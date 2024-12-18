using FluentValidation;

namespace Ordering.Application.Features.V1.Orders;

public class CreateOrUpdateValidator : AbstractValidator<CreateOrUpdateCommand>
{
    public CreateOrUpdateValidator()
    {
        RuleFor(p=>p.FirstName)
            .NotEmpty().WithMessage("First name cannot be empty")
            .NotNull()
            .MaximumLength(150).WithMessage("First name cannot exceed 150 characters");
        
        RuleFor(p=>p.LastName)
            .NotEmpty().WithMessage("Last name cannot be empty")
            .NotNull()
            .MaximumLength(150).WithMessage("Last name cannot exceed 150 characters");
        
        RuleFor(p=>p.EmailAddress)
            .NotEmpty().WithMessage("Email address cannot be empty")
            .EmailAddress().WithMessage("Invalid email address");
        
        RuleFor(p=>p.TotalPrice)
            .NotEmpty().WithMessage("Total price cannot be empty")
            .GreaterThan(0).WithMessage("Invalid total price");
    }
}