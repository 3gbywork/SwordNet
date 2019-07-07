using System;
using System.Collections;
using System.Windows.Controls;
using Tools.Business;
using Tools.Models;
using Tools.Modules;
using Xceed.Wpf.AvalonDock.Layout;

namespace Tools.ContentPanel
{
    /// <summary>
    /// AvalonDock.xaml 的交互逻辑
    /// </summary>
    public partial class AvalonDock : UserControl, IContentPanel
    {
        public AvalonDock()
        {
            InitializeComponent();

            RibbonButtonUtility.AddRibbonButtonAction += RibbonButtonUtility_AddRibbonButtonAction;
        }

        public void Add(object item)
        {
            ItemsPanel.Children.Add((LayoutAnchorable)item);
        }

        public IEnumerator GetEnumerator()
        {
            return ItemsPanel.Children.GetEnumerator();
        }

        public object GetView(IRibbonButtonInfo info)
        {
            return RibbonButtonUtility.GetView(typeof(AvalonDock).AssemblyQualifiedName, ContentPanelModule.UnityContainer, info);
        }

        public object NewItem(IRibbonButtonInfo info, object view)
        {
            return new LayoutAnchorable
            {
                Title = info.DisplayName,
                Content = view,
                CanClose = false,
                //CanAutoHide = false,
                //CanFloat = false,
                //CanHide = false,
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
                ContentPanelUtility.AddViewToContentPanel<LayoutAnchorable>(info, this, v => v.Title.Equals(info.DisplayName, StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
