using System;
using System.Reflection;
using Tools.Models;
using Utility.Logging;

namespace Tools.Business
{
    class ContentPanelUtility
    {
        static Logger mLogger = new Logger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void AddViewToContentPanel<T>(IRibbonButtonInfo viewInfo, IContentPanel panel, Func<T, bool> predicate)
        {
            if (panel.IsLoaded())
            {
                bool shouldCreate = true;
                var enumerator = panel.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        if (predicate((T)enumerator.Current))
                        {
                            shouldCreate = false;
                            break;
                        }
                    }
                }
                finally
                {
                    enumerator = null;
                }
                if (shouldCreate)
                {
                    try
                    {
                        var view = panel.GetView(viewInfo);
                        if (view != null)
                        {
                            panel.Add(panel.NewItem(viewInfo, view));
                        }
                    }
                    catch (Exception ex)
                    {
                        mLogger.Warn($"Error while invoking addin:{viewInfo.Name}, type:{viewInfo.Type}, assembly:{viewInfo.Assembly}, due to:{ex}");
                    }
                }
            }
        }
    }
}
