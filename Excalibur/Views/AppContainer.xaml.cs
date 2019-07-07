using CommonUtility.Config;
using CommonUtility.Logging;
using Excalibur.Config;
using Excalibur.Models;
using Excalibur.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using TheSeed;
using WinApi.Net.Geometry;
using WinApi.Net.User32;

namespace Excalibur.Views
{
    /// <summary>
    /// TaskManager.xaml 的交互逻辑
    /// </summary>
    public partial class AppContainer : UserControl, IDisposable, IView
    {
        static Logger mLogger = new Logger(MethodBase.GetCurrentMethod().DeclaringType);
        Process mProcess = null;
        IntPtr mParentHandle = IntPtr.Zero;
        bool mHasParent = false;

        public string ID { get; set; }

        public AppContainer()
        {
            UI.Culture = Thread.CurrentThread.CurrentUICulture;

            InitializeComponent();

            Panel.Loaded += OnPanelLoaded;
            Panel.SizeChanged += OnPanelSizeChanged;
            Panel.IsVisibleChanged += OnPanelIsVisibleChanged;

            Notice.Text = UI.Container_Notice;
        }

        private void OnPanelLoaded(object sender, RoutedEventArgs e)
        {
            var entity = new ContainerConfigEntity();
            entity.OnConfigChanged += OnConfigChanged;
            XmlConfigurator.ConfigAndWatch(entity, new FileInfo("config/ContainerConfig.xml"));
        }

        private void OnConfigChanged(ICollection<ContainerModel> containers)
        {
            ContainerModel model = null;
            foreach (var container in containers)
            {
                if (ID == container.ID)
                {
                    model = container;
                    break;
                }
            }

            if (!mHasParent && null != model)
            {
                var hwnd = (HwndSource)PresentationSource.FromVisual(Panel);
                if (hwnd != null && hwnd.Handle != IntPtr.Zero)
                {
                    mParentHandle = hwnd.Handle;
                    hwnd.AddHook(this.WndProc);
                    try
                    {
                        if (mProcess == null)
                        {
                            mProcess = GetProcess(model.ProcessName, model.FullName, model.Param);
                        }
                        if (mProcess != null && mProcess.MainWindowHandle != IntPtr.Zero)
                        {
                            User32Methods.SetWindowLongPtr(mProcess.MainWindowHandle, (int)WindowLongFlags.GWL_STYLE, (IntPtr)WindowStyles.WS_VISIBLE);
                            var result = User32Methods.SetParent(mProcess.MainWindowHandle, mParentHandle);

                            mHasParent = result == null ? false : result == IntPtr.Zero ? false : true;
                            if (mHasParent)
                            {
                                OnPanelSizeChanged(null, null);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        mLogger.Error($"{ID} -- Error while setting window parent, due to:{ex}");
                        this.Dispose();
                    }
                }
            }
        }

        private void OnPanelIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (mProcess != null && mProcess.MainWindowHandle != IntPtr.Zero)
                {
                    var isVisible = (bool)e.NewValue;
                    User32Methods.ShowWindow(mProcess.MainWindowHandle, isVisible ? ShowWindowCommands.SW_SHOW : ShowWindowCommands.SW_HIDE);
                }
            }
            catch (Exception ex)
            {
                mLogger.Error($"{ID} -- Error while changing window visible, due to:{ex}");
                Dispose();
            }
        }

        private void OnPanelSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Resize(mParentHandle, (int)Math.Round(Panel.ActualWidth, 0, MidpointRounding.AwayFromZero), (int)Math.Round(Panel.ActualHeight, 0, MidpointRounding.AwayFromZero));
        }

        private void Resize(IntPtr parentHandle, int width, int height)
        {
            try
            {
                if (Panel.IsVisible && parentHandle != IntPtr.Zero && mProcess != null && mProcess.MainWindowHandle != IntPtr.Zero)
                {
                    var curPoint = PointToScreen(new System.Windows.Point(0, 0));
                    int offsetLeft = 0;
                    int offsetTop = 0;
                    if (User32Methods.GetWindowRect(parentHandle, out Rectangle rect))
                    {
                        offsetLeft = (int)Math.Round(curPoint.X, 0) - rect.Left - 9;
                        offsetTop = (int)Math.Round(curPoint.Y, 0) - rect.Top - 30;
                        User32Methods.MoveWindow(mProcess.MainWindowHandle, offsetLeft, offsetTop, width + 28, height + 28, true);
                    }
                }
            }
            catch (Exception ex)
            {
                mLogger.Error($"{ID} -- Error while resizing window, due to:{ex}");
                this.Dispose();
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            handled = false;
            WM wMsg = (WM)msg;
            switch (wMsg)
            {
                case WM.PAINT:
                    OnPanelSizeChanged(null, null);
                    break;
            }

            return IntPtr.Zero;
        }

        private Process GetProcess(string processName, string fullName, string param)
        {
            Process result = null;
            try
            {
                var processes = Process.GetProcessesByName(processName);
                foreach (var process in processes)
                {
                    if (process.MainWindowHandle != IntPtr.Zero)
                    {
                        return process;
                    }
                }

                mLogger.Info($"{ID} -- Creat new process: {processName} full name: {fullName} param: {param}.");
                result = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = fullName,
                        Arguments = param,
                        WindowStyle = ProcessWindowStyle.Minimized,
                        UseShellExecute = false,
                        //Verb = "runas",
                    }
                };
                result.Start();

                // 等待窗口加载完成
                int retryCount = 50;
                while (result != null && result.MainWindowHandle == IntPtr.Zero)
                {
                    result.WaitForInputIdle(100);
                    retryCount--;
                }
            }
            catch (Exception ex)
            {
                mLogger.Error($"{ID} -- Error while getting {processName} process, due to:{ex}");
                if (ex is Win32Exception win32ex)
                {
                    if (win32ex.NativeErrorCode.Equals(740))
                    {
                        MessageBox.Show(UI.Console_RunAsAdminError, win32ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                throw;
            }

            return result;
        }

        public void Dispose()
        {
            try
            {
                if (mProcess != null)
                {
                    using (mProcess)
                    {
                        if (!mProcess.HasExited && !mProcess.CloseMainWindow())
                        {
                            if (!mProcess.HasExited)
                            {
                                mLogger.Info($"{ID} -- Killing process.");
                                mProcess.Kill();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mLogger.Warn($"{ID} -- Exception while disposing process, due to:{ex}");
            }
            finally
            {
                mProcess = null;
            }
        }
    }
}
