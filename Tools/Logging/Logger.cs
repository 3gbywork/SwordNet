using Prism.Logging;
using System.Reflection;
using LoggerUtil = Utility.Logging.Logger;

namespace Tools.Logging
{
    class Logger : ILoggerFacade
    {
        static LoggerUtil mlogger = new LoggerUtil(MethodBase.GetCurrentMethod().DeclaringType);
        public void Log(string message, Category category, Priority priority)
        {
            string msg = $"{priority} {message}";
            switch (category)
            {
                case Category.Debug:
                    mlogger.Debug(msg);
                    break;
                case Category.Exception:
                    mlogger.Error(msg);
                    break;
                default:
                case Category.Info:
                    mlogger.Info(msg);
                    break;
                case Category.Warn:
                    mlogger.Warn(msg);
                    break;
            }
        }
    }
}
