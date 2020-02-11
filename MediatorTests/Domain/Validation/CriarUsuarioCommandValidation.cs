
using Domain.Commands;
using FluentValidation;
using FluentValidation.Results;

namespace Domain.Validation
{
    public class CriarUsuarioCommandValidation : AbstractValidator<CriarUsuarioCommand>
    {
        public CriarUsuarioCommandValidation()
        {
            RuleFor(x => x.Id)
                .NotEmpty();
            RuleFor(x => x.Nome)
                .NotEmpty();
        }
    }

}