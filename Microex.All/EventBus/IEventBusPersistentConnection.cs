using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;

namespace Microex.All.EventBus
{
    public interface IEventBusPersistentConnection<TModel>
        : IEventBusPersistentConnection,IDisposable
    {
        TModel CreateModel();
    }

    public interface IEventBusPersistentConnection
    {
        bool IsConnected { get; }

        bool TryConnect();
    }
}
