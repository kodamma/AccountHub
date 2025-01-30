using AccountHub.Application.CQRS.Commands.Account.AddAccount;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace AccountHub.Application.Validation
{
    public class AddAccountCommandValidator : AbstractValidator<AddAccountCommand>
    {
        private readonly long maxAvatarLenght;
        public AddAccountCommandValidator(IConfiguration config)
        {
            maxAvatarLenght = long.Parse(config["Kestrel:MaxAvatarLength"]!);

            RuleFor(x => x.Password)
                .Matches(@"^\S*$")
                .WithMessage("The password must not contain spaces.");
            RuleFor(x => x.Birthdate)
                .Must(b => CalculateAge(b) >= 13)
                .WithMessage("The age must not be less than 13 years old.");
            RuleFor(x => x.Birthdate)
                .Must(b => CalculateAge(b) <= 100)
                .WithMessage("The age cannot be more than 100 years old.");
            RuleFor(x => x.Avatar)
                .Must(a => a == null || a?.Length < maxAvatarLenght)
                .WithMessage("The maximum file size is more than 5 KB");
            RuleFor(x => x.Avatar)
                .Must(a => a == null || Regex.IsMatch(Path.GetExtension(a.FileName.ToLower()), @"\.(png|jpe?g)$"))
                .WithMessage("Invalid file extension. Only .png, .jpeg, and .jpg are allowed. Please select a file with a valid extension.");
        }

        private static int CalculateAge(DateOnly date)
        {
            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            var age = currentDate.Year - date.Year;
            if (date > currentDate.AddYears(-age)) age--;
            return Math.Abs(age);
        }
    }
}
