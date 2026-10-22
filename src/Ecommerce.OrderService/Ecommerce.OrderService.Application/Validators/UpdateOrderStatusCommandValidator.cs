using Ecommerce.OrderService.Application.Commands.UpdateOrderStatus;
using FluentValidation;

namespace Ecommerce.OrderService.Application.Validators;

public class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
{
    private static readonly string[] AllowedStatuses = { "Pending", "Confirmed", "Failed", "Cancelled" };

    public UpdateOrderStatusCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty().WithMessage("OrderId is required.");
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.")
            .Must(s => AllowedStatuses.Contains(s, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Status must be one of: Pending, Confirmed, Failed, Cancelled.");
    }
}

