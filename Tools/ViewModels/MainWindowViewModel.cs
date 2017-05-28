using Prism.Mvvm;
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

        private bool isActive = true;
        public bool IsActive
        {
            get { return isActive; }
            set { SetProperty(ref isActive, value); }
        }
    }
}
