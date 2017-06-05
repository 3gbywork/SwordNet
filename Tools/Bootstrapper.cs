using System;
using System.Windows;
using Microsoft.Practices.ServiceLocation;
using Prism.Logging;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Unity;
using Tools.Business;
using Tools.Logging;
using Tools.ViewModels;
using Tools.Views;
using Tools.Saying;

namespace Tools
{
    class Bootstrapper : UnityBootstrapper, IDisposable
    {

        protected override DependencyObject CreateShell()
        {
            return Container.TryResolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            return new ConfigurationModuleCatalog();
        }

        protected override ILoggerFacade CreateLogger()
        {
            return new Logger();
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            ViewModelLocationProvider.Register<RibbonUI, MainWindowViewModel>();
        }

        public void Dispose()
        {
            RibbonButtonUtility.Dispose();
            SayingManager.UpdateSayingsIndex();
        }

        //protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
        //{
        //    var mappings = base.ConfigureRegionAdapterMappings();
        //    mappings.RegisterMapping(typeof(RibbonTabItem), ServiceLocator.Current.GetInstance<RibbonGroupBoxAdapter>());

        //    return mappings;
        //}

        //protected override void ConfigureModuleCatalog()
        //{
        //    //ModuleCatalog.AddModule(GetPanelModuleInfo(typeof(AvalonDockModule), true));
        //    //ModuleCatalog.AddModule(GetPanelModuleInfo(typeof(TabControlModule)));

        //    ModuleCatalog.AddModule(GetPanelModuleInfo(typeof(ContentControlModule), true));
        //}

        //private ModuleInfo GetPanelModuleInfo(Type moduleType, bool initWhenStartup = false)
        //{
        //    return new ModuleInfo
        //    {
        //        ModuleName = moduleType.Name,
        //        ModuleType = moduleType.AssemblyQualifiedName,
        //        InitializationMode = initWhenStartup ? InitializationMode.WhenAvailable : InitializationMode.OnDemand,
        //    };
        //}
    }
}
