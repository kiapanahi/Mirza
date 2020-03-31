using FluentValidation.TestHelper;
using Mirza.Web.Models;
using Mirza.Web.Validators;
using Xunit;

namespace Mirza.Web.UnitTests.ModelTests
{
    public class TeamTests
    {
        private readonly TeamValidator _teamValidator;
        public TeamTests()
        {
            _teamValidator = new TeamValidator();
        }

        [Fact]
        public void Name_Should_Have_Validation_Error_When_Null()
        {
            var t = new Team { Name = null };
            _teamValidator.TestValidate(t)
                .ShouldHaveValidationErrorFor(t => t.Name)
                .WithErrorMessage("Name must be a non-empty value")
                .WithSeverity(FluentValidation.Severity.Error);
        }

        [Fact]
        public void Name_Should_Have_Validation_Error_When_EmptyString()
        {
            var t = new Team { Name = string.Empty };
            _teamValidator.TestValidate(t)
                .ShouldHaveValidationErrorFor(t => t.Name)
                .WithErrorMessage("Name must be a non-empty value")
                .WithSeverity(FluentValidation.Severity.Error);
        }

        [Fact]
        public void Name_Should_Have_Validation_Error_When_MoreThan50Characters()
        {
            var t = new Team { Name = new string('a', 55) };
            _teamValidator.TestValidate(t)
                .ShouldHaveValidationErrorFor(t => t.Name)
                .WithErrorMessage("Name must be at most 50 characters")
                .WithSeverity(FluentValidation.Severity.Error);
        }

        [Fact]
        public void Name_Should_Be_Valid()
        {
            var t = new Team { Name = "sample_name" };
            _teamValidator.TestValidate(t)
                .ShouldNotHaveValidationErrorFor(t => t.Name);
        }

        [Fact]
        public void Model_Should_Be_Valid()
        {
            var t = new Team { Name = "sample_name" };
            _teamValidator.TestValidate(t)
                .ShouldNotHaveAnyValidationErrors();
        }
    }
}
