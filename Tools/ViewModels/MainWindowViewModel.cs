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
        public string ApplicationName
        {
            get
            {
                return Configurator.GetConfiguration("ApplicationName", "Tools");
            }
        }
        public string RibbonUI_Quit { get { return Resources.UI.RibbonUI_Quit; } }
        public string RibbonUI_Views { get { return Resources.UI.RibbonUI_Views; } }
        public string RibbonUI_ViewsGroup { get { return Resources.UI.RibbonUI_ViewsGroup; } }
        public string RibbonUI_AddIns { get { return Resources.UI.RibbonUI_AddIns; } }

        public string ContentRegion { get { return RegionNameConstants.ContentRegion; } }
        public string CenterRegion { get { return RegionNameConstants.CenterRegion; } }

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
