using System.Windows.Controls;
using Tools.Business;
using Tools.Commands;
using Tools.Config;
using Tools.Modules;
using Utility.Prism;

namespace Tools.Views
{
    /// <summary>
    /// RibbonViewsMenu.xaml 的交互逻辑
    /// </summary>
    public partial class RibbonViewsMenu : UserControl
    {
        public RibbonViewsMenu()
        {
            InitializeComponent();

            var views = ToolsConfig.GetInstance().Views;
            var command = new ContentPanelCommand(ContentPanelModule.RegionManager, RegionNameConstants.CenterRegion);
            ViewsContainer.ItemsSource = RibbonButtonUtility.GenerateRibbonButtons(command, views);
        }
    }
}
