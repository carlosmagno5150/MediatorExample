using System;

namespace Project.Cqrs.Events
{
    public class Event : IEvent
    {
        public DateTime DtEvent { get; }

        public Event()
        {
            DtEvent = DateTime.Now;
        }
    }
}