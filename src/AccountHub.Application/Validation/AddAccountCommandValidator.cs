using AccountHub.Application.ApiClients;
using AccountHub.Application.CQRS.Commands.Account.AddAccount;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace AccountHub.Application.Validation
{
    public class AddAccountCommandValidator : AbstractValidator<AddAccountCommand>
    {
        private readonly IGeoServiceApiClient apiClient;
        private readonly long maxAvatarLenght;
        public AddAccountCommandValidator(IConfiguration config, IGeoServiceApiClient apiClient)
        {
            this.apiClient = apiClient;
            maxAvatarLenght = long.Parse(config["Kestrel:MaxAvatarLength"]!);

            RuleFor(x => x.Username)
                .Matches(@"^[a-zA-Z0-9_8$&.@#]+$")
                .WithErrorCode("INVALID_USERNAME_CHARACTERS")
                .WithMessage("It does not contain spaces or special characters, except for the following: _, 8, $, &, ., @, #.")
                .Length(2, 45)
                .WithErrorCode("INVALID_USERNAME_LENGTH")
                .WithMessage("The value must not exceed 45 characters.");
            RuleFor(x => x.Email)
                .Must(IsValidEmail)
                .WithErrorCode("INVALID_EMAIL_FORMAT")
                .WithMessage("The email address does not match the valid email format.");
            RuleFor(x => x.Password)
                .Matches(@"^\S*$")
                .WithErrorCode("INVALID_PASSWORD")
                .WithMessage("The password must not contain spaces.")
                .Length(10, 50)
                .WithErrorCode("INVALID_PASSWORD_LENGTH")
                .WithMessage("The value must be no more than 50 characters and no less than 10.");
            RuleFor(x => x.Birthdate)
                .Must(b => CalculateAge(b) >= 13)
                .WithErrorCode("MINIMUM_AGE_REQUIREMENT_NOT_MET")
                .WithMessage("The age must not be less than 13 years old.");
            RuleFor(x => x.Birthdate)
                .Must(b => CalculateAge(b) <= 100)
                .WithErrorCode("MAXIMUM_AGE_EXCEEDED")
                .WithMessage("The age cannot be more than 100 years old.");
            RuleFor(x => x.Avatar)
                .Must(a => a == null || a?.Length < maxAvatarLenght)
                .WithErrorCode("MAXIMUM_FILE_SIZE_EXCEEDED")
                .WithMessage("The maximum file size is more than 5 KB.");
            RuleFor(x => x.Avatar)
                .Must(a => a == null || Regex.IsMatch(Path.GetExtension(a.FileName.ToLower()), @"\.(png|jpe?g)$"))
                .WithErrorCode("INVALID_FILE_EXTENSION")
                .WithMessage("Invalid file extension. Only .png, .jpeg, and .jpg are allowed. ");
            RuleFor(x => x.Agree)
                .Must(a => a == true)
                .WithErrorCode("TERMS_NOT_ACCEPTED")
                .WithMessage("You must accept the terms and conditions to complete the registration.");
            RuleFor(x => x.Country)
                .MustAsync(IsAvailableCountry)
                .WithErrorCode("INVALID_COUNTRY")
                .WithMessage("There is no country with that name.");
        }

        private async Task<bool> IsAvailableCountry(string input, CancellationToken cancellationToken)
        {
            var result = await apiClient.GetAvailableCountries();
            return result.Contains(input);
        }

        private static int CalculateAge(DateOnly date)
        {
            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            var age = currentDate.Year - date.Year;
            if (date > currentDate.AddYears(-age)) age--;
            return Math.Abs(age);
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new MailAddress(email);
            }
            catch
            {
                return false;
            }

            var parts = email.Split('@');
            if (parts.Length != 2)
                return false;

            var localPart = parts[0];

            var regex = new Regex(@"^[a-zA-Z0-9.]+$");
            return regex.IsMatch(localPart);
        }
    }
}
