namespace NetIRC.Messages
{
    public interface IServerMessage
    {
        void TriggerEvent(EventHub eventHub);
    }
}
