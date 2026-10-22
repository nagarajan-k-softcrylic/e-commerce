using Ecommerce.InventoryService.Application.Commands.UpdateProduct;
using FluentValidation;

namespace Ecommerce.InventoryService.Application.Validators;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Product ID is required.");
        RuleFor(x => x.ProductCode).NotEmpty().WithMessage("Product code is required.");
        RuleFor(x => x.ProductName).NotEmpty().WithMessage("Product name is required.");
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than 0.");
        RuleFor(x => x.AvailableQuantity).GreaterThanOrEqualTo(0).WithMessage("Available quantity cannot be negative.");
    }
}

