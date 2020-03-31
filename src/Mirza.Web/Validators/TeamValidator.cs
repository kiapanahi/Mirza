using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Mirza.Web.Models;

namespace Mirza.Web.Validators
{
    [SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "<Pending>")]
    public class TeamValidator : AbstractValidator<Team>
    {
        public TeamValidator()
        {
            RuleFor(t => t.Name)
                .NotEmpty().WithMessage("Name must be a non-empty value")
                .MaximumLength(50).WithMessage("Name must be at most 50 characters");
        }
    }
}
