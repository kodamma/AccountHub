using AccountHub.Application.CQRS.Commands.Account.AddAccount;
using AccountHub.Application.Validation;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Configuration;

namespace AccountHub.Application.Test.Validation
{
    [TestClass]
    public class AddAccountCommandValidatorTest
    {
        private IConfiguration root = null!;
        private AddAccountCommandValidator validator;
        private AddAccountCommand command;
        public AddAccountCommandValidatorTest()
        {
            IEnumerable<KeyValuePair<string, string>> pairs = [
                    new KeyValuePair<string, string>("Kestrel:MaxAvatarLength", "5120")];
            root = new ConfigurationBuilder()
                .AddInMemoryCollection(pairs).Build();

            validator = new AddAccountCommandValidator(root);
            command = new AddAccountCommand()
            {
                Username = "user1",
                Email = "user@mail.ru",
                Password = "password",
                Birthdate = new DateOnly(2012, 1, 1),
                Avatar = null
            };
        }

        [TestMethod]
        public void AddAccountCommandValidator_Must_Success()
        {
            var result = validator.TestValidate(command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void AddAccountCommandValidator_Must_Fail()
        {
            command.Password = "pass word";
            command.Birthdate = new DateOnly(2012, 2, DateOnly.FromDateTime(DateTime.Today).Day + 1);

            var result = validator.TestValidate(command);

            Assert.IsFalse(result.IsValid);
            result.ShouldHaveValidationErrorFor(x => x.Password);
            result.ShouldHaveValidationErrorFor(x => x.Birthdate);
            Assert.AreEqual(12, CalculateAge(command.Birthdate));
        }

        [TestMethod]
        public void AddAccountCommandValidator_Over100Years_Fail()
        {
            command.Birthdate = new DateOnly(1924, 1, DateOnly.FromDateTime(DateTime.Today).Day);

            var result = validator.TestValidate(command);

            Assert.IsFalse(result.IsValid);
            result.ShouldHaveValidationErrorFor(x => x.Birthdate);
            Assert.AreEqual(101, CalculateAge(command.Birthdate));
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
