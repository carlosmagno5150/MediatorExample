using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Cqrs.Events
{
    public interface IEventLogService
    {
        IEnumerable<IEvent> GetUnpublishedEvents();

        Task MarkEventAsPublishedAsync(IEvent @event);
    }
}