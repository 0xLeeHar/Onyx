using FluentValidation;

namespace Onyx.Services.Products.Domain.Requests;

/// <summary>
/// An HTTP request model for creating a new product.
/// </summary>
public record CreateProductRequest(string Name, string Description, int PriceInMinorUnits, string Colour);

// NOTE: Validation is handles in the HTTP request pipeline and FluentValidation will return problem details for failures. 
public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Products must have a name")
            .Length(10, 100)
            .WithMessage("The name must be between 10 and 100 characters");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Products must have a description")
            .Length(10, 500)
            .WithMessage("The description must be between 10 and 500 characters");
        
        RuleFor(x => x.PriceInMinorUnits)
            .GreaterThanOrEqualTo(0)
            .WithMessage("The price must be greater than 0");
        
        RuleFor(x => x.Colour)
            .NotEmpty()
            .WithMessage("The product must have a colour")
            .Length(1, 50)
            .WithMessage("Product descriptions must be between 1 and 50 characters");
    }
}