using System.Reflection;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using Tools.Business;
using Tools.Config;
using Tools.Views;
using Utility.Logging;
using Utility.Prism;

namespace Tools.Modules
{
    public class ContentPanelModule : IModule
    {
        static Logger mLogger = new Logger(MethodBase.GetCurrentMethod().DeclaringType);
        public static IRegionManager RegionManager;
        public static IUnityContainer UnityContainer;
        public ContentPanelModule(IUnityContainer container, RegionManager regionManager)
        {
            RegionManager = regionManager;
            UnityContainer = container;
        }
        public void Initialize()
        {
            RegionManager.RegisterViewWithRegion(RegionNameConstants.ContentRegion, typeof(RibbonUI));

            var views = ToolsConfig.GetInstance().Views;
            IRegion region = RegionManager.Regions[RegionNameConstants.CenterRegion];
            RibbonButtonUtility.AddToRegion(UnityContainer, region, views);
        }
    }
}
