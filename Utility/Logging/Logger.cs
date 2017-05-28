using log4net;
using log4net.Config;
using System;
using System.Reflection;

namespace Utility.Logging
{
    public class Logger
    {
        private ILog mlogger = null;
        private string mName = null;

        private Logger()
        {
            XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.config"));
        }
        public Logger(string name) : this()
        {
            this.mName = name;
            mlogger = LogManager.GetLogger(Assembly.GetCallingAssembly(), name);
        }

        public Logger(Type type) : this(type.FullName)
        {
        }

        public void Debug(string format, params object[] args)
        {
            mlogger.DebugFormat(format, args);
        }

        public void Debug(IFormatProvider provider, string format, params object[] args)
        {
            mlogger.DebugFormat(provider, format, args);
        }

        public void Error(string format, params object[] args)
        {
            mlogger.ErrorFormat(format, args);
        }

        public void Error(IFormatProvider provider, string format, params object[] args)
        {
            mlogger.ErrorFormat(provider, format, args);
        }

        public void Fatal(string format, params object[] args)
        {
            mlogger.FatalFormat(format, args);
        }

        public void Fatal(IFormatProvider provider, string format, params object[] args)
        {
            mlogger.FatalFormat(provider, format, args);
        }

        public void Info(string format, params object[] args)
        {
            mlogger.InfoFormat(format, args);
        }

        public void Info(IFormatProvider provider, string format, params object[] args)
        {
            mlogger.InfoFormat(provider, format, args);
        }

        public void Warn(string format, params object[] args)
        {
            mlogger.WarnFormat(format, args);
        }

        public void Warn(IFormatProvider provider, string format, params object[] args)
        {
            mlogger.WarnFormat(provider, format, args);
        }

        public void Log(Level level, object message, Exception exception=null)
        {
            switch (level)
            {
                case Level.Debug:
                    mlogger.Debug(message, exception);
                    break;
                case Level.Error:
                    mlogger.Error(message, exception);
                    break;
                case Level.Fatal:
                    mlogger.Fatal(message, exception);
                    break;
                default:
                case Level.Info:
                    mlogger.Info(message, exception);
                    break;
                case Level.Warn:
                    mlogger.Warn(message, exception);
                    break;
            }
        }
    }
}
