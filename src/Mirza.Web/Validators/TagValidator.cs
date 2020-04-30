using FluentValidation;
using Mirza.Web.Models;

namespace Mirza.Web.Validators
{
    public class TagValidator : AbstractValidator<Tag>
    {
        public TagValidator()
        {
            RuleFor(t => t.Value)
                .NotEmpty().WithMessage("Tag value cannot be empty")
                .MaximumLength(256).WithMessage("tag value cannot exceed 256 character");
        }
    }
}