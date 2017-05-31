using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using Prism.Mvvm;
using Tools.Commands;
using Utility.Config;
using Utility.Prism;

namespace Tools.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly Timer mTimer;

        public MainWindowViewModel()
        {
            mTimer = new Timer(TimeSpan.FromSeconds(5).TotalMilliseconds);
            mTimer.Elapsed += (sender, e) =>
            {
                RaisePropertyChanged(nameof(UsedMemory));
            };
            mTimer.Start();
        }

        public string ApplicationName
        {
            get
            {
                return Configurator.GetConfiguration("ApplicationName", "Tools");
            }
        }
        public string RibbonUI_Quit => Resources.UI.RibbonUI_Quit;
        public string RibbonUI_Views => Resources.UI.RibbonUI_Views;
        public string RibbonUI_ViewsGroup => Resources.UI.RibbonUI_ViewsGroup;
        public string RibbonUI_AddIns => Resources.UI.RibbonUI_AddIns;
        public string RibbonUI_Saying => Resources.UI.RibbonUI_Saying;
        public string RibbonUI_UsedMemory => Resources.UI.RibbonUI_UsedMemory;

        public string Saying => "一二三四五六七";
        public double UsedMemory => Process.GetCurrentProcess().PrivateMemorySize64 / 1024;

        public string ContentRegion => RegionNameConstants.ContentRegion;
        public string CenterRegion => RegionNameConstants.CenterRegion;

        private RelayCommand quitCommand;
        public ICommand QuitCommand
        {
            get
            {
                if (quitCommand == null)
                {
                    quitCommand = new RelayCommand(Application.Current.Shutdown, () => { return true; });
                }
                return quitCommand;
            }
        }
    }
}
