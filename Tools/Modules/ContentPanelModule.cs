﻿using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using Tools.Business;
using Tools.Config;
using Tools.Constants;
using Tools.Views;

namespace Tools.Modules
{
    public class ContentPanelModule : IModule
    {
        public static IRegionManager RegionManager;
        public static IUnityContainer UnityContainer;
        public ContentPanelModule(IUnityContainer container, RegionManager regionManager)
        {
            RegionManager = regionManager;
            UnityContainer = container;
        }
        public void Initialize()
        {
            RegionManager.RegisterViewWithRegion(PrismRegionNameConstant.ContentRegion, typeof(RibbonUI));

            var views = ToolsConfig.GetInstance().Views;
            IRegion region = RegionManager.Regions[PrismRegionNameConstant.CenterRegion];
            RibbonButtonUtility.AddToRegion(UnityContainer, region, views);
        }
    }
}
