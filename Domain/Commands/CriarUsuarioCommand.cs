using Domain.Model;
using Domain.Validation;
using MediatR;

namespace Domain.Commands
{
    public class CriarUsuarioCommand: IRequest<ResponseResult>
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        public CriarUsuarioCommand(int id, string nome)
        {
            Id = id;
            Nome = nome;
        }
    }
}