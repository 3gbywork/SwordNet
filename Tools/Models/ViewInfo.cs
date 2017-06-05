using System;

namespace Tools.Models
{
    public class ViewInfo : RibbonButtonBaseInfo
    {
        private ConfigurationContentPanelView view;

        public ViewInfo(ConfigurationContentPanelView view)
        {
            this.view = view;
            Assembly = view.Assembly;
            Name = view.Name;
            Icon = view.Icon;
            Type = view.Type;
        }
    }
}
