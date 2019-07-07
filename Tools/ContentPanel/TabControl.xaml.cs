using System.Collections;
using System.Windows.Controls;
using Tools.Business;
using Tools.Models;
using Tools.Modules;

namespace Tools.ContentPanel
{
    /// <summary>
    /// TabControl.xaml 的交互逻辑
    /// </summary>
    public partial class TabControl : UserControl, IContentPanel
    {
        public TabControl()
        {
            InitializeComponent();

            RibbonButtonUtility.AddRibbonButtonAction += RibbonButtonUtility_AddRibbonButtonAction;
        }

        public void Add(object item)
        {
            ItemsPanel.Items.Add(item);
        }

        public IEnumerator GetEnumerator()
        {
            return ItemsPanel.Items.SourceCollection.GetEnumerator();
        }

        public object GetView(IRibbonButtonInfo info)
        {
            return RibbonButtonUtility.GetView(typeof(TabControl).AssemblyQualifiedName, ContentPanelModule.UnityContainer, info);
        }

        public object NewItem(IRibbonButtonInfo info, object view)
        {
            return new TabItem()
            {
                Header = info.DisplayName,
                Content = view,
            };
        }

        bool IContentPanel.IsLoaded()
        {
            return this.IsLoaded;
        }

        private void RibbonButtonUtility_AddRibbonButtonAction(object parameter)
        {
            if (parameter is IRibbonButtonInfo info)
            {
                ContentPanelUtility.AddViewToContentPanel<TabItem>(info, this, v => v.Header.Equals(info.DisplayName));
            }
        }
    }
}
