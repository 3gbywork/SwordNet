﻿using Excalibur.Resources;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Utility.Logging;
using WinApi.Net.User32;

namespace Excalibur.Views
{
    /// <summary>
    /// TaskManager.xaml 的交互逻辑
    /// </summary>
    public partial class TaskManager : UserControl, IDisposable
    {
        static Logger mLogger = new Logger(MethodBase.GetCurrentMethod().DeclaringType);
        Process mProcess = null;
        IntPtr mParentHandle = IntPtr.Zero;
        bool mHasParent = false;

        public TaskManager()
        {
            InitializeComponent();

            mProcess = GetTaskManagerProcess();

            Panel.Loaded += OnPanelLoaded;
            Panel.SizeChanged += OnPanelSizeChanged;
            Panel.IsVisibleChanged += OnPanelIsVisibleChanged;
        }

        private void OnPanelLoaded(object sender, RoutedEventArgs e)
        {
            if (!mHasParent)
            {
                var hwnd = (HwndSource)PresentationSource.FromVisual(Panel);
                if (hwnd != null && hwnd.Handle != IntPtr.Zero)
                {
                    mParentHandle = hwnd.Handle;
                    hwnd.AddHook(WndProc);
                    try
                    {
                        if (mProcess != null && mProcess.MainWindowHandle != IntPtr.Zero)
                        {
                            NativeMethods.SetWindowLongPtr(mProcess.MainWindowHandle, (int)WindowLongFlags.GWL_STYLE, (IntPtr)WindowStyles.WS_VISIBLE);
                            var result = NativeMethods.SetParent(mProcess.MainWindowHandle, mParentHandle);

                            mHasParent = result == null ? false : result == IntPtr.Zero ? false : true;
                            if (mHasParent)
                            {
                                OnPanelSizeChanged(null, null);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        mLogger.Error($"Error while setting window parent, due to:{ex}");
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
                    NativeMethods.ShowWindow(mProcess.MainWindowHandle, isVisible ? ShowWindowCommands.SW_SHOW : ShowWindowCommands.SW_HIDE);
                }
            }
            catch (Exception ex)
            {
                mLogger.Error($"Error while changing window visible, due to:{ex}");
                this.Dispose();
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
                    if (NativeMethods.GetWindowRect(parentHandle, out Rectangle rect))
                    {
                        offsetLeft = (int)Math.Round(curPoint.X, 0) - rect.Left - 9;
                        offsetTop = (int)Math.Round(curPoint.Y, 0) - rect.Top - 30;
                        NativeMethods.MoveWindow(mProcess.MainWindowHandle, offsetLeft, offsetTop, width + 28, height + 28, true);
                    }
                }
            }
            catch (Exception ex)
            {
                mLogger.Error($"Error while resizing window, due to:{ex}");
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

        private Process GetTaskManagerProcess()
        {
            Process result = null;
            try
            {
                var processes = Process.GetProcessesByName("taskmgr");
                foreach (var process in processes)
                {
                    if (process.MainWindowHandle != IntPtr.Zero)
                    {
                        return process;
                    }
                }

                mLogger.Info("Creat new process.");
                result = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = @"C:\Windows\System32\Taskmgr.exe",
                        WindowStyle = ProcessWindowStyle.Minimized,
                        UseShellExecute = false,
                        Verb = "runas",
                    }
                };
                result.Start();
            }
            catch (Exception ex)
            {
                mLogger.Error($"Error while getting taskmgr process, due to:{ex}");
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
                                mLogger.Info($"Killing task manager process.");
                                mProcess.Kill();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mLogger.Warn($"Exception while disposing process, due to:{ex}");
            }
            finally
            {
                mProcess = null;
            }
        }
    }
}
