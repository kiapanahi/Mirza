using System;
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
            var model = new MirzaUser { FirstName = null };
            _validator.TestValidate(model)
                .ShouldHaveValidationErrorFor(u => u.FirstName)
                .WithErrorMessage("FirstName must be a non-empty value")
                .WithSeverity(FluentValidation.Severity.Error);
        }

        [Fact]
        public void FirstName_Should_Have_Validation_Error_When_EmptyString()
        {
            var model = new MirzaUser { FirstName = string.Empty };
            _validator.TestValidate(model)
                .ShouldHaveValidationErrorFor(u => u.FirstName)
                .WithErrorMessage("FirstName must be a non-empty value")
                .WithSeverity(FluentValidation.Severity.Error);
        }

        [Fact]
        public void FirstName_Should_Have_Validation_Error_When_MoreThan40Characters()
        {
            var model = new MirzaUser { FirstName = new string('a', 50) };
            _validator.TestValidate(model)
                .ShouldHaveValidationErrorFor(u => u.FirstName)
                .WithErrorMessage("FirstName must be at most 40 characters long")
                .WithSeverity(FluentValidation.Severity.Error);
        }

        [Fact]
        public void FirstName_Should_Be_Valid()
        {
            var model = new MirzaUser { FirstName = new string('a', 20) };
            _validator.TestValidate(model)
                .ShouldNotHaveValidationErrorFor(u => u.FirstName);
        }

        [Fact]
        public void LastName_Should_Have_Validation_Error_When_Null()
        {
            var model = new MirzaUser { LastName = null };
            _validator.TestValidate(model)
                .ShouldHaveValidationErrorFor(u => u.LastName)
                .WithErrorMessage("LastName must be a non-empty value")
                .WithSeverity(FluentValidation.Severity.Error);
        }

        [Fact]
        public void LastName_Should_Have_Validation_Error_When_EmptyString()
        {
            var model = new MirzaUser { LastName = string.Empty };
            _validator.TestValidate(model)
                .ShouldHaveValidationErrorFor(u => u.LastName)
                .WithErrorMessage("LastName must be a non-empty value")
                .WithSeverity(FluentValidation.Severity.Error);
        }

        [Fact]
        public void LastName_Should_Have_Validation_Error_When_MoreThan50Characters()
        {
            var model = new MirzaUser { LastName = new string('a', 55) };
            _validator.TestValidate(model)
                .ShouldHaveValidationErrorFor(u => u.LastName)
                .WithErrorMessage("LastName must be at most 50 characters long")
                .WithSeverity(FluentValidation.Severity.Error);
        }

        [Fact]
        public void LastName_Should_Be_Valid()
        {
            var model = new MirzaUser { LastName = new string('a', 45) };
            _validator.TestValidate(model)
                .ShouldNotHaveValidationErrorFor(u => u.LastName);
        }

        [Fact]
        public void Email_Should_Have_Validation_Error_When_Null()
        {
            var model = new MirzaUser { Email = null };
            _validator.TestValidate(model)
                .ShouldHaveValidationErrorFor(u => u.Email)
                .WithErrorMessage("Email must be a non-empty value")
                .WithSeverity(FluentValidation.Severity.Error);
        }

        [Fact]
        public void Email_Should_Have_Validation_Error_When_EmptyString()
        {
            var model = new MirzaUser { Email = string.Empty };
            _validator.TestValidate(model)
                .ShouldHaveValidationErrorFor(u => u.Email)
                .WithErrorMessage("Email must be a non-empty value")
                .WithSeverity(FluentValidation.Severity.Error);
        }

        [Fact]
        public void Email_Should_Have_Validation_Error_When_NotValidEmail()
        {
            var model = new MirzaUser { Email = Guid.NewGuid().ToString() };
            _validator.TestValidate(model)
                .ShouldHaveValidationErrorFor(u => u.Email)
                .WithErrorMessage("Email field must abide by the simple email structure. i.e. NAME@DOMAIN.TLD")
                .WithSeverity(FluentValidation.Severity.Error);
        }

        [Fact]
        public void Email_Should_Be_Valid()
        {
            var model = new MirzaUser { Email = "sample@example.com" };
            _validator.TestValidate(model)
                .ShouldNotHaveValidationErrorFor(u => u.Email);
        }

        [Fact]
        public void Model_Should_be_Valid()
        {
            var u = new MirzaUser
            {
                FirstName = "sample_first_name",
                LastName = "sample_last_name",
                Email = "sample@example.com"
            };
            _validator.TestValidate(u)
                .ShouldNotHaveAnyValidationErrors();
        }
    }
}
