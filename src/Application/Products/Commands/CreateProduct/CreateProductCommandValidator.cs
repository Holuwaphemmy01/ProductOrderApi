using FluentValidation;
using ProductOrderAPI.Application.Products.Commands.CreateProduct;

namespace ProductOrderAPI.Application.Products.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.");

        RuleFor(x => x.Price.Amount)
            .GreaterThan(0).WithMessage("Price amount must be greater than zero.");

        RuleFor(x => x.Price.Currency)
            .NotEmpty().WithMessage("Currency is required.");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative.");
    }
}
