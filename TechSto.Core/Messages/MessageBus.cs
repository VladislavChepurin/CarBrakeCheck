using System.Collections.Concurrent;

namespace TechSto.Core.Messages
{
    public class MessageBus : IMessageBus
    {
        private readonly ConcurrentDictionary<Type, List<Delegate>> _handlers = new();

        public void Publish<TMessage>(TMessage message)
        {
            if (_handlers.TryGetValue(typeof(TMessage), out var handlers))
            {
                foreach (var handler in handlers.ToArray())
                {
                    ((Action<TMessage>)handler)?.Invoke(message);
                }
            }
        }

        public IDisposable Subscribe<TMessage>(Action<TMessage> handler)
        {
            var type = typeof(TMessage);

            var handlers = _handlers.GetOrAdd(type, _ => new List<Delegate>());

            lock (handlers)
            {
                handlers.Add(handler);
            }

            return new Subscription(() =>
            {
                lock (handlers)
                {
                    handlers.Remove(handler);
                }
            });
        }

        private class Subscription : IDisposable
        {
            private readonly Action _dispose;

            public Subscription(Action dispose)
            {
                _dispose = dispose;
            }

            public void Dispose()
            {
                _dispose();
            }
        }
    }
}