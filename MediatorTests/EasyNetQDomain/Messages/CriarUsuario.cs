using EasyNetQ;
using MediatR;

namespace EasyNetQDomain.Messages
{
    [Queue("CriarUsuario", ExchangeName = "CriacaoUsuario_EX")]
    public class CriarUsuario: IRequest<bool>
    {
        public int Id { get; set; }
        public string Nome { get; set; }
    }
}