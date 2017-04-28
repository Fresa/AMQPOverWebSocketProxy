using System;
using Common.Logging;
using Common.Logging.Loggers;
using SuperSocket.SocketBase.Logging;

namespace AMQPOverWebSocketProxy.Logging
{
    public class Logger : ILog
    {
        private readonly ILogger _logger;

        public Logger(string logger)
        {
            _logger = LoggerFactory.Create(logger);
        }

        public void Debug(object message)
        {
            _logger.Debug(message.ToString());
        }

        public void Debug(object message, Exception exception)
        {
            _logger.Debug(message + ". " + exception.Message);
        }

        public void DebugFormat(string format, object arg0)
        {
            _logger.Debug(format, arg0);
        }

        public void DebugFormat(string format, params object[] args)
        {
            _logger.Debug(format, args);
        }

        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            _logger.Debug(string.Format(provider, format, args));
        }

        public void DebugFormat(string format, object arg0, object arg1)
        {
            _logger.Debug(format, arg0, arg1);
        }

        public void DebugFormat(string format, object arg0, object arg1, object arg2)
        {
            _logger.Debug(format, arg0, arg1, arg2);
        }

        public void Error(object message)
        {
            _logger.Error(message.ToString());
        }

        public void Error(object message, Exception exception)
        {
            _logger.Error(message.ToString(), exception);
        }

        public void ErrorFormat(string format, object arg0)
        {
            _logger.Error(format, arg0);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            _logger.Error(format, args);
        }

        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            _logger.Error(string.Format(provider, format, args));
        }

        public void ErrorFormat(string format, object arg0, object arg1)
        {
            _logger.Error(format, arg0, arg1);
        }

        public void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
            _logger.Error(format, arg0, arg1, arg2);
        }

        public void Fatal(object message)
        {
            _logger.Fatal(message.ToString(), null);
        }

        public void Fatal(object message, Exception exception)
        {
            _logger.Fatal(message.ToString(), exception);
        }

        public void FatalFormat(string format, object arg0)
        {
            _logger.Fatal(null, format, arg0);
        }

        public void FatalFormat(string format, params object[] args)
        {
            _logger.Fatal(null, format, args);
        }

        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            _logger.Fatal(null, string.Format(provider, format, args));
        }

        public void FatalFormat(string format, object arg0, object arg1)
        {
            _logger.Fatal(null, format, arg0, arg1);
        }

        public void FatalFormat(string format, object arg0, object arg1, object arg2)
        {
            _logger.Fatal(null, format, arg0, arg1, arg2);
        }

        public void Info(object message)
        {
            _logger.Info(message.ToString());
        }

        public void Info(object message, Exception exception)
        {
            _logger.Info(message + ". " + exception.Message);
        }

        public void InfoFormat(string format, object arg0)
        {
            _logger.Info(format, arg0);
        }

        public void InfoFormat(string format, params object[] args)
        {
            _logger.Info(format, args);
        }

        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            _logger.Info(string.Format(provider, format, args));
        }

        public void InfoFormat(string format, object arg0, object arg1)
        {
            _logger.Info(format, arg0, arg1);
        }

        public void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
            _logger.Info(format, arg0, arg1, arg2);
        }

        public void Warn(object message)
        {
            _logger.Warning(message.ToString(), null);
        }

        public void Warn(object message, Exception exception)
        {
            _logger.Warning(message.ToString(), exception);
        }

        public void WarnFormat(string format, object arg0)
        {
            _logger.Warning(null, format, arg0);
        }

        public void WarnFormat(string format, params object[] args)
        {
            _logger.Warning(null, format, args);
        }

        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            _logger.Warning(null, string.Format(provider, format, args));
        }

        public void WarnFormat(string format, object arg0, object arg1)
        {
            _logger.Warning(null, format, arg0, arg1);
        }

        public void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
            _logger.Warning(null, format, arg0, arg1, arg2);
        }

        public bool IsDebugEnabled => true;
        public bool IsErrorEnabled => true;
        public bool IsFatalEnabled => true;
        public bool IsInfoEnabled => true;
        public bool IsWarnEnabled => true;
    }
}