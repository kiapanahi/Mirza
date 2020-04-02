using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Mirza.Web.Models;

namespace Mirza.Web.Validators
{
    [SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "<Pending>")]
    public class WorkLogValidator : AbstractValidator<WorkLog>
    {
        public WorkLogValidator()
        {
            RuleFor(w => w.EntryDate)
                .NotNull().WithMessage("EntryDate is mandatory");

            RuleFor(w => w.StartTime)
                .NotEmpty().WithMessage("StartTime is mandatory");

            RuleFor(w => w.EndTime)
                .NotEmpty().WithMessage("EndTime is mandatory");

            RuleFor(w => w.Description)
                .MaximumLength(500).WithMessage("Description field must be less than 500 characters long");

            RuleFor(w => w.Details)
                .MaximumLength(500).WithMessage("Details field must be less than 500 characters long");
        }
    }
}
