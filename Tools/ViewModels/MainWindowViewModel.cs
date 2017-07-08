using CommonUtility.Command;
using CommonUtility.Config;
using Prism.Mvvm;
using System;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using Tools.Constants;
using Tools.Saying;

namespace Tools.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        // 用于更新程序占用内存的计时器
        private readonly Timer mUsedMemoryTimer;
        private readonly int mUsedMemoryTimerSeconds = 5;
        // 用于更新名言名句的计时器
        private readonly Timer mSayingTimer;
        private readonly int mSayingTimerSeconds = 30;

        public MainWindowViewModel()
        {
            mUsedMemoryTimer = new Timer(TimeSpan.FromSeconds(mUsedMemoryTimerSeconds).TotalMilliseconds);
            mUsedMemoryTimer.Elapsed += (sender, e) =>
            {
                RaisePropertyChanged(nameof(UsedMemory));
            };
            mUsedMemoryTimer.Start();

            mSayingTimer = new Timer(TimeSpan.FromSeconds(mSayingTimerSeconds).TotalMilliseconds);
            mSayingTimer.Elapsed += (sender, e) =>
            {
                RaisePropertyChanged(nameof(Saying));
            };
            mSayingTimer.Start();
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

        public string Saying => SayingManager.GetNextSaying();
        public double UsedMemory => Process.GetCurrentProcess().PrivateMemorySize64 / 1024;

        public string ContentRegion => PrismRegionNameConstant.ContentRegion;
        public string CenterRegion => PrismRegionNameConstant.CenterRegion;

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
