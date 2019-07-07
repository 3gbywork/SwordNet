namespace Tools.Models
{
    public class ModuleInfo : RibbonButtonBaseInfo
    {
        public ModuleInfo(ConfigurationModule module)
        {
            ID = module.ID;
            DisplayName = module.DisplayName;
            Assembly = module.Assembly;
            Icon = module.Icon;
            Name = module.Name;
            Type = module.Type;
        }
    }
}
