using CommonUtility.Logging;
using CommonUtility.Config;
using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows;

namespace Tools
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        static Logger mlogger = new Logger(MethodBase.GetCurrentMethod().DeclaringType);
        static Bootstrapper strapper;
        public App()
        {
            //new DataReader().Load();
            var culture = new CultureInfo(Configurator.GetConfiguration("Language", "zh-Hans"));
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            Tools.Resources.UI.Culture = culture;

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                strapper?.Dispose();
            };
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            mlogger.Info($"Application started.");
            base.OnStartup(e);

            try
            {
                strapper = new Bootstrapper();
                strapper.Run();
            }
            catch (Exception ex)
            {
                mlogger.Fatal($"An error occurred, application exited due to: {ex}");
                MessageBox.Show(ex.Message);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            try
            {
                strapper?.Dispose();
            }
            catch (Exception ex)
            {
                mlogger.Error($"Error while disposing resources due to:{ex}");
            }

            mlogger.Info($"Application exited.");
        }
    }
}
