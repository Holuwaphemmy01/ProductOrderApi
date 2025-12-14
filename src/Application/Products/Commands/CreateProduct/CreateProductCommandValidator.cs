using FluentValidation;
using ProductOrderAPI.Application.Products.Commands.CreateProduct;

namespace ProductOrderAPI.Application.Products.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("Name is required.");

        RuleFor(command => command.Description)
            .NotEmpty().WithMessage("Description is required.");

        RuleFor(command => command.Price.Amount)
            .GreaterThan(0).WithMessage("Price amount must be greater than zero.");

        RuleFor(command => command.Price.Currency)
            .NotEmpty().WithMessage("Currency is required.")
            .Length(3).WithMessage("Currency must be 3 characters long (e.g., USD).");

        RuleFor(command => command.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative.");
    }
}
