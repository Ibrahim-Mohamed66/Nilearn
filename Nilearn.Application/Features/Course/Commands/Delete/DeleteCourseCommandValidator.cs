using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nilearn.Application.Features.Course.Commands.Delete
{
    internal class DeleteCourseCommandValidator : AbstractValidator<DeleteCourseCommand>
    {
        public DeleteCourseCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Course ID must be greater than 0.");
        }
    }
}
