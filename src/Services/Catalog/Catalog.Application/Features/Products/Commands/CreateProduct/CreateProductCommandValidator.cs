using Catalog.Application.Features.Products.Commands.CreateProduct;
using FluentValidation;

namespace Catalog.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(100).WithMessage("Product name must not exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(x => x.AvailableStock)
            .GreaterThanOrEqualTo(0).WithMessage("Available stock cannot be negative");

        RuleFor(x => x.PictureUrl)
            .MaximumLength(500).WithMessage("Picture URL must not exceed 500 characters");
    }
}
