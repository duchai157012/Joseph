using Catalog.Application.Features.Products.Commands.CreateProduct;
using FluentValidation.TestHelper;

namespace Catalog.UnitTests.Application;

public class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator;

    public CreateProductCommandValidatorTests()
    {
        _validator = new CreateProductCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldNotHaveErrors()
    {
        var command = new CreateProductCommand
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            PictureUrl = "http://test.com/image.jpg",
            AvailableStock = 10
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyName_ShouldHaveError()
    {
        var command = new CreateProductCommand
        {
            Name = "",
            Price = 99.99m,
            AvailableStock = 10
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Product name is required");
    }

    [Fact]
    public void Validate_WithNegativePrice_ShouldHaveError()
    {
        var command = new CreateProductCommand
        {
            Name = "Test",
            Price = -10m,
            AvailableStock = 10
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Price)
            .WithErrorMessage("Price must be greater than 0");
    }

    [Fact]
    public void Validate_WithNegativeStock_ShouldHaveError()
    {
        var command = new CreateProductCommand
        {
            Name = "Test",
            Price = 99.99m,
            AvailableStock = -5
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AvailableStock)
            .WithErrorMessage("Available stock cannot be negative");
    }
}
