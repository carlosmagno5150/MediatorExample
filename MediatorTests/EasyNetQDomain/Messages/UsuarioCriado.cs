using EasyNetQ;
using MediatR;
using System;

namespace EasyNetQDomain.Messages
{
    [Queue("UsuarioCriado", ExchangeName = "UsuarioCriado_EX")]
    public class UsuarioCriado: IRequest<bool>
    {
        public UsuarioCriado(DateTime dtCriacao)
        {
            DtCriacao = dtCriacao;
        }

        public DateTime DtCriacao { get; set; }
    }
}