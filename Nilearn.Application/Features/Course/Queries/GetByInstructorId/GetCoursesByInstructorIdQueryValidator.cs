using FluentValidation;


namespace Nilearn.Application.Features.Course.Queries.GetByInstructorId
{
    internal class GetCoursesByInstructorIdQueryValidator : AbstractValidator<GetCoursesByInstructorIdQuery>
    {
        public GetCoursesByInstructorIdQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("Instructor ID must be provided.");
        }
    }
}
