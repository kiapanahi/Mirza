using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Mirza.Web.Models;

namespace Mirza.Web.Validators
{
    [SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "<Pending>")]
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(p => p.FirstName)
                .NotEmpty().WithMessage("FirstName must be a non-empty value")
                .MaximumLength(40).WithMessage("FirstName must be at most 40 characters long");

            RuleFor(p => p.LastName)
                .NotEmpty().WithMessage("LastName must be a non-empty value")
                .MaximumLength(50).WithMessage("LastName must be at most 50 characters long");

            RuleFor(p => p.Email)
                .NotEmpty().WithMessage("Email must be a non-empty value")
                .EmailAddress().WithMessage("Email field must abide by the simple email structure. i.e. NAME@DOMAIN.TLD");
        }
    }
}
