using Topshelf;

namespace AMQPOverWebSocketProxy
{
    public interface IService
    {
        bool Start(HostControl hostControl);
        void Stop();
    }
}