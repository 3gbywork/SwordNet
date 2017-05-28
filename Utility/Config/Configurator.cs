using System;
using System.Configuration;
using System.Reflection;
using Utility.Convert;
using Utility.Logging;

namespace Utility.Config
{
    public class Configurator
    {
        static Logger mlogger = new Logger(MethodBase.GetCurrentMethod().DeclaringType);

        public static T GetConfiguration<T>(string key, T defaultValue)
        {
            return GetConfiguration<T>(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None), key, defaultValue);
        }

        public static T GetConfiguration<T>(Configuration configuration, string key, T defaultValue)
        {
            T result = defaultValue;
            try
            {
                string value = configuration.AppSettings.Settings[key].Value;
                result = Converter.TryParse(value, defaultValue);
            }
            catch (Exception ex)
            {
                mlogger.Warn($"Get {key} configuration failed & use default value {defaultValue}, error {ex}");
            }

            return result;
        }
    }
}
