using CommonUtility.Lock;
using CommonUtility.Logging;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using Tools.Models;

namespace Tools.Config
{
    public class ToolsConfig
    {
        const string configFilePath = "ToolsConfig.xml";
        static Logger mLogger = new Logger(MethodBase.GetCurrentMethod().DeclaringType);
        static object mLocker = Locker.GetLocker(nameof(ToolsConfig));
        static ToolsConfig instance = null;

        public SortedSet<ViewInfo> Views = new SortedSet<ViewInfo>();
        public SortedSet<ModuleInfo> Modules = new SortedSet<ModuleInfo>();
        public ConfigurationSaying Saying;
        private ToolsConfig()
        {
            using (var stream = new FileStream(configFilePath, FileMode.Open))
            {
                var xmlSerializer = new XmlSerializer(typeof(Configuration));
                if (xmlSerializer.Deserialize(stream) is Configuration config)
                {
                    if (config.ContentPanelViews != null)
                    {
                        foreach (var view in config.ContentPanelViews)
                        {
                            Views.Add(new ViewInfo(view));
                        }
                    }
                    if (config.Modules != null)
                    {
                        foreach (var module in config.Modules)
                        {
                            Modules.Add(new ModuleInfo(module));
                        }
                    }
                    Saying = config.Saying;
                }
            }
        }

        public static ToolsConfig GetInstance()
        {
            if (instance == null)
            {
                lock (mLocker)
                {
                    if (instance == null)
                    {
                        instance = new ToolsConfig();
                    }
                }
            }

            return instance;
        }
    }
}
