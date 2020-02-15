
using Project.Cqrs.DomainNotification;
using Project.Cqrs.Events;

namespace Domain.Events
{
    public class UsuarioCriado: DomainNotification
    {
        public UsuarioCriado(string key, string value) : base(key, value)
        {
        }
    }
}
