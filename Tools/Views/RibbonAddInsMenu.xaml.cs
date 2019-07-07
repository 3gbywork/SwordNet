using System.ComponentModel;
using System.Windows.Controls;
using Tools.Business;
using Tools.Commands;
using Tools.Config;

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

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                var modules = ToolsConfig.GetInstance().Modules;
                var command = new AddInCommand();
                AddInsContainer.ItemsSource = RibbonButtonUtility.GenerateRibbonButtons(command, modules);
            }
        }
    }
}
