using MediatR;
using System;

namespace Custom.Cqrs.Events
{
    public interface IEvent : INotification
    {
    }

    public abstract class Event : IEvent
    {
        protected Event()
        {
            DtEvent = DateTime.Now;
        }

        protected DateTime DtEvent { get; }

    }
}