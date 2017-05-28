using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using Tools.Models;
using Utility.Logging;

namespace Tools.Config
{
    class ToolsConfig
    {
        const string configFilePath = "ToolsConfig.xml";
        static Logger mLogger = new Logger(MethodBase.GetCurrentMethod().DeclaringType);
        static object mLocker = Utility.Lock.Locker.GetLocker(nameof(ToolsConfig));
        static ToolsConfig instance = null;

        public SortedSet<ViewInfo> Views = new SortedSet<ViewInfo>();
        public SortedSet<ModuleInfo> Modules = new SortedSet<ModuleInfo>();
        private ToolsConfig()
        {
            Views = LoadViewsFromConfigFile();
            Modules = LoadModelsFromConfigFile();
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

        private SortedSet<ViewInfo> LoadViewsFromConfigFile()
        {
            SortedSet<ViewInfo> result = new SortedSet<ViewInfo>();
            if (File.Exists(configFilePath))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(configFilePath);

                var nodes = xmlDoc.SelectNodes("Configuration/ContentPanelViews/ContentPanelView");
                foreach (XmlElement node in nodes)
                {
                    try
                    {
                        if (node.HasAttributes && node.Attributes.Count == 4)
                        {
                            ViewInfo viewInfo = new ViewInfo
                            {
                                Name = node.Attributes["name"].Value,
                                Icon = node.Attributes["icon"].Value,
                                Type = node.Attributes["type"].Value,
                                Assembly = node.Attributes["assembly"].Value,
                            };

                            result.Add(viewInfo);
                        }
                    }
                    catch (Exception ex)
                    {
                        mLogger.Log(Level.Warn, "An error occurred while loading views from config file. due to:", ex);
                    }
                }
            }

            return result;
        }
        private SortedSet<ModuleInfo> LoadModelsFromConfigFile()
        {
            SortedSet<ModuleInfo> result = new SortedSet<ModuleInfo>();
            if (File.Exists(configFilePath))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(configFilePath);

                var nodes = xmlDoc.SelectNodes("Configuration/Modules/Module");
                foreach (XmlElement node in nodes)
                {
                    try
                    {
                        if (node.HasAttributes && node.Attributes.Count == 4)
                        {
                            ModuleInfo modelInfo = new ModuleInfo
                            {
                                Name = node.Attributes["name"].Value,
                                Icon = node.Attributes["icon"].Value,
                                Type = node.Attributes["type"].Value,
                                Assembly = node.Attributes["assembly"].Value,
                            };

                            result.Add(modelInfo);
                        }
                    }
                    catch (Exception ex)
                    {
                        mLogger.Log(Level.Warn, "An error occurred while loading models from config file. due to:", ex);
                    }
                }
            }

            return result;
        }
    }
}
