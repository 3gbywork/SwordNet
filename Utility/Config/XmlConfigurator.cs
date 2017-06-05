using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml;
using Utility.Logging;

namespace Utility.Config
{
    public class XmlConfigurator : IDisposable
    {
        const int TimeoutMillsecond = 500;

        FileInfo mConfigFile;
        FileSystemWatcher mWatcher;
        Timer mTimer;
        static Logger mLogger = new Logger(MethodBase.GetCurrentMethod().DeclaringType);

        IXmlConfig mXmlConfigEntity;

        static readonly Dictionary<string, XmlConfigurator> mConfigurators = new Dictionary<string, XmlConfigurator>();

        public static void ConfigAndWatch(IXmlConfig configEntity, FileInfo configFile)
        {
            mLogger.Debug($"Config and watch file:{configFile}");

            if (configFile == null)
            {
                mLogger.Debug($"Configure called with null 'configFile' parameter");
                return;
            }
            if (configEntity == null)
            {
                mLogger.Debug($"Configure called with null 'configEntity' parameter");
                return;
            }

            InternalConfigure(configEntity, configFile);
            try
            {
                lock (mConfigurators)
                {
                    if (mConfigurators.TryGetValue(configFile.FullName, out XmlConfigurator handler))
                    {
                        if (handler != null)
                        {
                            handler.Dispose();
                        }
                    }

                    mConfigurators[configFile.FullName] = new XmlConfigurator(configEntity, configFile);
                }
            }
            catch (Exception ex)
            {
                mLogger.Error($"Failed to initialize configuration file watcher for the file:{configFile.FullName}, due to:{ex}");
            }
        }

        private XmlConfigurator(IXmlConfig configEntity, FileInfo configFile)
        {
            this.mConfigFile = configFile;
            this.mXmlConfigEntity = configEntity;

            this.mWatcher = new FileSystemWatcher()
            {
                Path = mConfigFile.DirectoryName,
                Filter = mConfigFile.Name,
                NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.FileName,
            };

            this.mWatcher.Changed += new FileSystemEventHandler(ConfigFileChangedHandler);
            this.mWatcher.Created += new FileSystemEventHandler(ConfigFileChangedHandler);
            this.mWatcher.Deleted += new FileSystemEventHandler(ConfigFileChangedHandler);
            this.mWatcher.Renamed += new RenamedEventHandler(ConfigFileRenamedHandler);

            this.mWatcher.EnableRaisingEvents = true;

            this.mTimer = new Timer(OnConfigFileChanged, null, Timeout.Infinite, Timeout.Infinite);
        }

        private void OnConfigFileChanged(object state)
        {
            InternalConfigure(mXmlConfigEntity, mConfigFile);
        }

        static private void InternalConfigure(IXmlConfig configEntity, FileInfo configFile)
        {
            mLogger.Debug($"Configuring file:{configFile.FullName}");

            if (configFile == null)
            {
                mLogger.Debug($"Configure called with null 'configFile' parameter");
                return;
            }

            if (File.Exists(configFile.FullName))
            {
                FileStream fileStream = null;
                for (int retry = 5; --retry >= 0;)
                {
                    try
                    {
                        fileStream = configFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
                        break;
                    }
                    catch (IOException ex)
                    {
                        if (retry == 0)
                        {
                            mLogger.Error($"Failed to open XML config file:{configFile.FullName}, due to:{ex}");

                            // The stream cannot be valid
                            fileStream = null;
                        }
                        Thread.Sleep(250);
                    }
                }

                if (fileStream != null)
                {
                    try
                    {
                        // Load the configuration from the stream
                        InternalConfigure(configEntity, fileStream);
                    }
                    finally
                    {
                        // Force the file closed whatever happens
                        fileStream.Close();
                    }
                }
            }
            else
            {
                mLogger.Debug($"Config file {configFile.FullName} not found. Configuration unchanged.");
            }
        }

        static private void InternalConfigure(IXmlConfig configEntity, FileStream fileStream)
        {
            if (fileStream == null)
            {
                mLogger.Debug($"Configure called with null 'fileStream' parameter");
                return;
            }
            if (configEntity == null)
            {
                mLogger.Debug($"Configure called with null 'configEntity' parameter");
                return;
            }

            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings()
                {
                    DtdProcessing = DtdProcessing.Parse,
                };

                using (XmlReader xmlReader = XmlReader.Create(fileStream, settings))
                {
                    xmlDoc.Load(xmlReader);
                }
            }
            catch (Exception ex)
            {
                mLogger.Error($"Error while loading XML configuration, due to:{ex}");
                xmlDoc = null;
            }

            if (xmlDoc != null)
            {
                mLogger.Debug($"Loading XML configuration.");
                configEntity.Config(xmlDoc.DocumentElement);
            }
        }

        private void ConfigFileRenamedHandler(object sender, RenamedEventArgs e)
        {
            mLogger.Debug($"ConfigFileRenamedHandler, file path:{e.OldFullPath} => {e.FullPath}, file name:{e.OldName} => {e.Name}, change type:{e.ChangeType}");
            mTimer.Change(TimeoutMillsecond, Timeout.Infinite);
        }

        private void ConfigFileChangedHandler(object sender, FileSystemEventArgs e)
        {
            mLogger.Debug($"ConfigFileChangedHandler, file path:{e.FullPath}, file name:{e.Name}, change type:{e.ChangeType}");
            mTimer.Change(TimeoutMillsecond, Timeout.Infinite);
        }

        public void Dispose()
        {
            if (mWatcher != null)
            {
                mWatcher.EnableRaisingEvents = false;
                mWatcher.Dispose();
            }
            if (mTimer != null)
            {
                mTimer.Dispose();
            }
        }
    }
}
