using CommonUtility.Logging;
using Prism.Regions;
using System;
using System.Reflection;
using System.Windows.Input;
using Tools.Models;

namespace Tools.Commands
{
    class ContentPanelCommand : ICommand
    {
        private string regionName;
        static Logger mLogger = new Logger(MethodBase.GetCurrentMethod().DeclaringType);
        private IRegionManager regionManager;

        public ContentPanelCommand(IRegionManager regionManager, string centerRegion)
        {
            this.regionManager = regionManager;
            this.regionName = centerRegion;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            try
            {
                var region = regionManager.Regions[regionName];
                if (region != null)
                {
                    if (parameter is IRibbonButtonInfo info)
                    {
                        var view = region.GetView(info.Name);
                        if (view != null)
                        {
                            region.Activate(view);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("An error occurred while activate view:{0}, due to:{1}", parameter, ex);
            }
        }
    }
}
