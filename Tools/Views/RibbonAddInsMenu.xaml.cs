using System.Windows.Controls;
using Tools.Business;
using Tools.Commands;
using Tools.Config;
using Tools.Modules;
using Utility.Prism;

namespace Tools.Views
{
    /// <summary>
    /// RibbonAddInsMenu.xaml 的交互逻辑
    /// </summary>
    public partial class RibbonAddInsMenu : UserControl
    {
        public RibbonAddInsMenu()
        {
            InitializeComponent();

            var modules = ToolsConfig.GetInstance().Modules;
            var command = new AddInCommand();
            AddInsContainer.ItemsSource = RibbonButtonUtility.GenerateRibbonButtons(command, modules);
        }
    }
}
