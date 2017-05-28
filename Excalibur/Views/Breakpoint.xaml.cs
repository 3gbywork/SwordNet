using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Controls;
using Excalibur.Config;
using Excalibur.Models;
using Excalibur.Resources;
using MahApps.Metro.Controls;
using Utility.Config;
using Utility.Logging;
using Operator = Excalibur.Business.BreakpointOperator;

namespace Excalibur.Views
{
    /// <summary>
    /// Breakpoint.xaml 的交互逻辑
    /// </summary>
    public partial class Breakpoint : UserControl, IDisposable
    {
        FileSystemWatcher mWatcher;
        ObservableCollection<BreakpointModel> mBreaks;
        static Logger mLogger = new Logger(MethodBase.GetCurrentMethod().DeclaringType);

        public Breakpoint()
        {
            UI.Culture = Thread.CurrentThread.CurrentUICulture;

            InitializeComponent();

            FileName.Header = UI.Breakpoint_FileName;
            Description.Header = UI.Breakpoint_Description;

            var entity = new BreakpointConfigEntity();
            entity.OnConfigChanged += OnConfigChanged;
            XmlConfigurator.ConfigAndWatch(entity, new FileInfo("config/BreakpointConfig.xml"));

            mWatcher = new FileSystemWatcher()
            {
                Path = @"C:\",
                IncludeSubdirectories = false,
                NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName,
                EnableRaisingEvents = true,
            };
            mWatcher.Created += OnFileChanged;
            mWatcher.Deleted += OnFileChanged;
            mWatcher.Renamed += OnFileRenamed;
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            mLogger.Debug($"File:{e.OldFullPath} {e.ChangeType} to {e.FullPath}");
            UpdataToggleSwitchState(e.OldFullPath, false);
            UpdataToggleSwitchState(e.FullPath, true);
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            mLogger.Debug($"File:{e.FullPath} change type:{e.ChangeType}");
            UpdataToggleSwitchState(e.FullPath, e.ChangeType == WatcherChangeTypes.Created);
        }

        private void UpdataToggleSwitchState(string fullPath, bool isChecked)
        {
            var point = mBreaks.FirstOrDefault(p => p.FileName.Equals(fullPath, StringComparison.OrdinalIgnoreCase));
            if (point != null)
            {
                point.IsChecked = isChecked;
            }
        }

        private void OnConfigChanged(ObservableCollection<BreakpointModel> breaks)
        {
            mBreaks = breaks;

            foreach (var point in mBreaks)
            {
                try
                {
                    if (File.Exists(point.FileName) || Directory.Exists(point.FileName))
                    {
                        point.IsChecked = true;
                    }
                }
                catch (Exception ex)
                {
                    mLogger.Warn($"Error while checking file or directory exists, due to:{ex}");
                }
            }

            Dispatcher.BeginInvoke(new Action(() =>
            {
                DataGrid.ItemsSource = mBreaks;
            }));
        }

        private void PreCheckedChanged(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                string filename = toggleSwitch.Tag.ToString();
                if (!string.IsNullOrEmpty(filename))
                {
                    mWatcher.EnableRaisingEvents = false;
                    bool result = false;
                    if (toggleSwitch.IsChecked == false)
                    {
                        result = Operator.Create(filename);
                    }
                    else
                    {
                        result = Operator.Delete(filename);
                    }
                    mWatcher.EnableRaisingEvents = true;

                    mLogger.Debug($"File:{filename}, check state:{toggleSwitch.IsChecked}, result:{result}");
                    if (result)
                    {
                        toggleSwitch.IsChecked = !toggleSwitch.IsChecked;
                    }
                    e.Handled = true;
                }
            }
        }

        public void Dispose()
        {
            mWatcher?.Dispose();
            mBreaks?.Clear();
            mBreaks = null;
        }
    }
}
