using FluentValidation;
using ProdutoTechfin.Application.Commands.Products.Create;

namespace ProdutoTechfin.Application.Validators.Products
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Nome é obrigatório.")
                .MaximumLength(200).WithMessage("Nome não pode exceder 200 caracteres.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Descrição não pode exceder 1000 caracteres.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Preço não pode ser negativo.");

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Estoque não pode ser negativo.");
        }
    }
}
