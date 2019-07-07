namespace Tools.Models
{
    public class ViewInfo : RibbonButtonBaseInfo
    {
        public ViewInfo(ConfigurationContentPanelView view)
        {
            ID = view.ID;
            DisplayName = view.DisplayName;
            Assembly = view.Assembly;
            Name = view.Name;
            Icon = view.Icon;
            Type = view.Type;
        }
    }
}
