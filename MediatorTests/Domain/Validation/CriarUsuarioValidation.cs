using Domain.Commands;
using FluentValidation;

namespace Domain.Validation
{
    public class CriarUsuarioValidation : AbstractValidator<CriarUsuario>
    {
        public CriarUsuarioValidation()
        {
            RuleFor(x => x.Id)
                .NotEmpty();
            RuleFor(x => x.Nome)
                .NotEmpty();
        }
    }
}