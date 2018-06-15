using System;
using System.Collections.Generic;
using System.Text;

namespace Microex.All.EventBus
{
    public interface IEventBus
    {
        void Subscribe<T, TH>(Func<TH> handler)
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;
        void Unsubscribe<T, TH>()
            where TH : IIntegrationEventHandler<T>
            where T : IntegrationEvent;

        void Publish(IntegrationEvent @event);
    }
}
