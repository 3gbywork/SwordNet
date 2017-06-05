using System;

namespace Tools.Models
{
    public class ModuleInfo : RibbonButtonBaseInfo
    {
        private ConfigurationModule module;

        public ModuleInfo(ConfigurationModule module)
        {
            this.module = module;
            Assembly = module.Assembly;
            Icon = module.Icon;
            Name = module.Name;
            Type = module.Type;
        }
    }
}
