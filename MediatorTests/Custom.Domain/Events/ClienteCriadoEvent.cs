using Custom.Cqrs.Events;

namespace Custom.Domain.Events
{
    public class ClienteCriado : Event
    {
        public ClienteCriado(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; }
        public string Name { get; }
    }
}