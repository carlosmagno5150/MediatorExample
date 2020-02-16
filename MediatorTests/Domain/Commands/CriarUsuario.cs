using Domain.Validation;
using FluentValidation;
using Project.Cqrs.Commands;

namespace Domain.Commands
{
    // ReSharper disable IdentifierTypo
    public class CriarUsuario : ValidatableCommand

    {
        public int Id { get; set; }
        public string Nome { get; set; }

        public CriarUsuario(int id, string nome)
        {
            Id = id;
            Nome = nome;
        }

        protected override IValidator Validator => new CriarUsuarioValidation();
    }
}