using CommonUtility.Extension;
using CommonUtility.Logging;
using CommonUtility.Security;
using Fluent;
using Microsoft.Practices.Unity;
using Prism.Regions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Input;
using Tools.Models;

namespace Tools.Business
{
    /// <summary>
    /// 将插件加载到Ribbon菜单
    /// 响应按钮点击事件，将插件加载到内容控件
    /// </summary>
    class RibbonButtonUtility
    {
        static Logger mLogger = new Logger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void AddToRegion<T>(IUnityContainer container, IRegion region, SortedSet<T> ribbonButtonInfos) where T : IRibbonButtonInfo
        {
            if (container == null || region == null || ribbonButtonInfos.Count == 0)
            {
                mLogger.Info($"Can't add to region, container:{container == null}, region:{region == null}, button count:{ribbonButtonInfos.Count}");
                return;
            }

            Assembly assembly = null;
            foreach (var info in ribbonButtonInfos)
            {
                try
                {
                    if (assembly == null || !info.Assembly.Equals(assembly.FullName, StringComparison.OrdinalIgnoreCase))
                    {
                        assembly = Assembly.Load(info.Assembly);
                    }
                    var view = container.Resolve(assembly.GetType(info.Type));
                    if (view == null)
                    {
                        mLogger.Warn("Can't resolve view by type:{0}, so remove it.", info.Type);
                        ribbonButtonInfos.Remove(info);
                        continue;
                    }
                    region.Add(view, info.Name);
                }
                catch (Exception ex)
                {
                    mLogger.Warn("An error occurred while adding view:{0} to region:{1}, due to:{2}", info.Name, region.Name, ex);
                    ribbonButtonInfos.Remove(info);
                }
            }
            assembly = null;
        }

        static public IEnumerable GenerateRibbonButtons<T>(ICommand command, SortedSet<T> ribbonButtonInfos) where T : IRibbonButtonInfo
        {
            List<Button> buttons = new List<Button>();

            if (command == null || ribbonButtonInfos.Count == 0)
            {
                mLogger.Info($"Can't generate ribbon buttons, command:{command == null}, button count:{ribbonButtonInfos.Count}");
                return buttons;
            }

            foreach (var info in ribbonButtonInfos)
            {
                try
                {
                    var iconPath = string.Format("pack://application:,,,/{0};component/{1}",
                    info.Assembly.Substring(0, info.Assembly.IndexOf(',')), info.Icon);
                    Button ribbonButton = new Button()
                    {
                        Header = info.Name,
                        Icon = iconPath,
                        LargeIcon = iconPath,
                        Command = command,
                        CommandParameter = info,
                    };

                    buttons.Add(ribbonButton);
                }
                catch (Exception ex)
                {
                    mLogger.Error($"Error while generating button:{info.Name}, due to:{ex}");
                }
            }

            return buttons;
        }

        #region 用于将插件View添加到content panel
        public delegate void ExecuteHandler(object parameter);
        public static event ExecuteHandler AddRibbonButtonAction;
        public static void Execute(object parameter)
        {
            if (AddRibbonButtonAction != null)
            {
                AddRibbonButtonAction.Invoke(parameter);
            }
        }
        #endregion

        static Dictionary<string, object> views = new Dictionary<string, object>();
        public static object GetView(string assemblyQualifiedName, IUnityContainer container, IRibbonButtonInfo buttonInfo)
        {
            if (container == null || buttonInfo == null)
            {
                mLogger.Warn($"Can't resolve view, container:{container == null}, buttonInfo:{buttonInfo == null}");
                return null;
            }

            var viewId = $"{assemblyQualifiedName},{buttonInfo.Type}".ToGuid().ToString("B");
            if (views.TryGetValue(viewId, out object view))
            {
                return view;
            }

            var assembly = Assembly.Load(buttonInfo.Assembly);
            view = container.Resolve(assembly.GetType(buttonInfo.Type));
            if (view != null)
            {
                views[viewId] = view;
            }
            return view;
        }

        public static void Dispose()
        {
            foreach (var view in views)
            {
                if (view.Value is IDisposable disposable)
                {
                    try
                    {
                        disposable.Dispose();
                    }
                    catch (Exception ex)
                    {
                        mLogger.Error($"Error while disposing view resources, due to:{ex}");
                    }
                }
            }
        }
    }
}
