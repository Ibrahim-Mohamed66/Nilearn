using FluentValidation;

namespace Nilearn.Application.Features.Payments.Queries.GetStudentPayments;

public sealed class GetStudentPaymentsQueryValidator : AbstractValidator<GetStudentPaymentsQuery>
{
    public GetStudentPaymentsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100)
            .WithMessage("Page size must not exceed 100.");
    }
}
