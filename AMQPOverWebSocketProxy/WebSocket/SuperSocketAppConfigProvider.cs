using System.Configuration;
using SuperSocket.SocketBase.Config;

namespace AMQPOverWebSocketProxy.WebSocket
{
    public class SuperSocketAppConfigProvider : ISuperSocketConfigurationProvider
    {
        public IConfigurationSource Get()
        {
            var obj = ConfigurationManager.GetSection("superSocket") ?? ConfigurationManager.GetSection("socketServer");
            if (obj == null)
            {
                throw new ConfigurationErrorsException("Missing 'superSocket' or 'socketServer' configuration section.");
            }
            var config = obj as IConfigurationSource;
            if (config == null)
            {
                throw new ConfigurationErrorsException("Invalid 'superSocket' or 'socketServer' configuration section.");
            }

            return config;
        }
    }
}