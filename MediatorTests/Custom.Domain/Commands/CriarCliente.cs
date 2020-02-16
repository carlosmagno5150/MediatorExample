using Custom.Cqrs.Commands;
using Custom.Domain.ValueObjects;

namespace Custom.Domain.Commands
{
    public class CriarCliente : Command
    {
        public CriarCliente(string nome, string cpf, string email)
        {
            Nome = nome;
            Cpf = new CPF { Numero = cpf };
            Email = new Email { Endereco = email };
        }

        public string Nome { get; set; }
        public CPF Cpf { get; set; }
        public Email Email { get; set; }
    }
}