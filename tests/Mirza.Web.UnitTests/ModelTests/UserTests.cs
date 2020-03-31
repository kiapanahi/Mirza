using FluentValidation.TestHelper;
using Mirza.Web.Models;
using Mirza.Web.Validators;
using Xunit;

namespace Mirza.Web.UnitTests.ModelTests
{
    public class UserTests
    {
        private readonly UserValidator _validator;
        public UserTests()
        {
            _validator = new UserValidator();
        }


        [Fact]
        public void FirstName_Should_Have_Validation_Error_When_Null()
        {
            var model = new User { FirstName = null };
            _validator.TestValidate(model)
                .ShouldHaveValidationErrorFor(u => u.FirstName)
                .WithErrorMessage("FirstName must be a non-empty value")
                .WithSeverity(FluentValidation.Severity.Error);
        }

        [Fact]
        public void FirstName_Should_Have_Validation_Error_When_EmptyString()
        {
            var model = new User { FirstName = string.Empty };
            _validator.TestValidate(model)
                .ShouldHaveValidationErrorFor(u => u.FirstName)
                .WithErrorMessage("FirstName must be a non-empty value")
                .WithSeverity(FluentValidation.Severity.Error);
        }

        [Fact]
        public void FirstName_Should_Have_Validation_Error_When_MoreThan40Characters()
        {
            var model = new User { FirstName = new string('a', 50) };
            _validator.TestValidate(model)
                .ShouldHaveValidationErrorFor(u => u.FirstName)
                .WithErrorMessage("FirstName must be at most 40 characters long")
                .WithSeverity(FluentValidation.Severity.Error);
        }

        [Fact]
        public void FirstName_Should_Be_Valid()
        {
            var model = new User { FirstName = new string('a', 20) };
            _validator.TestValidate(model)
                .ShouldNotHaveValidationErrorFor(u => u.FirstName);
        }

        [Fact]
        public void LastName_Should_Have_Validation_Error_When_Null()
        {
            var model = new User { LastName = null };
            _validator.TestValidate(model)
                .ShouldHaveValidationErrorFor(u => u.LastName)
                .WithErrorMessage("LastName must be a non-empty value")
                .WithSeverity(FluentValidation.Severity.Error);
        }

        [Fact]
        public void LastName_Should_Have_Validation_Error_When_EmptyString()
        {
            var model = new User { LastName = string.Empty };
            _validator.TestValidate(model)
                .ShouldHaveValidationErrorFor(u => u.LastName)
                .WithErrorMessage("LastName must be a non-empty value")
                .WithSeverity(FluentValidation.Severity.Error);
        }

        [Fact]
        public void LastName_Should_Have_Validation_Error_When_MoreThan50Characters()
        {
            var model = new User { LastName = new string('a', 55) };
            _validator.TestValidate(model)
                .ShouldHaveValidationErrorFor(u => u.LastName)
                .WithErrorMessage("LastName must be at most 50 characters long")
                .WithSeverity(FluentValidation.Severity.Error);
        }

        [Fact]
        public void LastName_Should_Be_Valid()
        {
            var model = new User { LastName = new string('a', 45) };
            _validator.TestValidate(model)
                .ShouldNotHaveValidationErrorFor(u => u.LastName);
        }

    }
}
