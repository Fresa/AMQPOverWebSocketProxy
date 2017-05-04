using Topshelf;

namespace AMQPOverWebSocketProxy
{
    public sealed class Service : IService
    {
        public bool Start(HostControl hostControl)
        {
            return true;
        }

        public void Stop()
        {
        }
    }
}