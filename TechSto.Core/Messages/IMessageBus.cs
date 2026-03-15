namespace TechSto.Core.Messages
{
    public interface IMessageBus
    {
        void Publish<TMessage>(TMessage message);

        IDisposable Subscribe<TMessage>(Action<TMessage> handler);
    }
}
