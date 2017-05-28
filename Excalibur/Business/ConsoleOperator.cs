using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Threading;
using Excalibur.Models;
using Utility.Logging;

namespace Excalibur.Business
{
    /// <summary>
    /// 对控制台程序封装，重定向控制台程序输出
    /// </summary>
    class ConsoleOperator : IDisposable
    {
        static Logger mLogger = new Logger(MethodBase.GetCurrentMethod().DeclaringType);

        private ConsoleModel model;
        private Dispatcher dispatcher;
        private HamburgerMenuModel hamburgerMenuModel;
        ProcessOperator mProcessOperator;

        public ConsoleOperator(ConsoleModel model, HamburgerMenuModel hamburgerMenuModel, Dispatcher dispatcher)
        {
            this.model = model;
            this.dispatcher = dispatcher;
            this.HamburgerMenuModel = hamburgerMenuModel;

            this.HamburgerMenuModel.Title = model.Name;
            this.HamburgerMenuModel.Path = model.Path;
            this.HamburgerMenuModel.Content = Content;

            this.mProcessOperator = new ProcessOperator(
                ProcessOperator.GetProcessStartInfo(model.Path, model.Param, true));
            mProcessOperator.OutputDataReceived += (message) =>
            {
                this.dispatcher.Invoke(new Action(() =>
                {
                    if (Content.Count >= 1000)
                    {
                        Content.RemoveAt(0);
                    }
                    Content.Add(message);
                    HasContent = true;
                }));
            };
        }

        private bool isRunning;
        public bool IsRunning
        {
            get => isRunning;
            set
            {
                if (!bool.Equals(isRunning, value))
                {
                    isRunning = value;
                    this.HamburgerMenuModel.StartButtonState = !value;
                    this.HamburgerMenuModel.StopButtonState = value;
                }
            }
        }

        private bool hasContent;
        public bool HasContent
        {
            get => hasContent;
            set
            {
                if (!bool.Equals(hasContent, value))
                {
                    hasContent = value;
                    this.HamburgerMenuModel.ClearButtonState = value;
                }
            }
        }

        public ObservableCollection<string> Content = new ObservableCollection<string>();

        public HamburgerMenuModel HamburgerMenuModel { get => hamburgerMenuModel; private set => hamburgerMenuModel = value; }

        internal void TryRun()
        {
            try
            {
                if (mProcessOperator != null)
                {
                    IsRunning = mProcessOperator.Start();
                }
            }
            catch (Exception ex)
            {
                mLogger.Error($"Error while starting process:{model?.Path}, due to:{ex}");
            }
        }

        internal void TryStop()
        {
            if (HasContent)
            {
                TryClear();
            }
            try
            {
                if (mProcessOperator != null)
                {
                    IsRunning = !mProcessOperator.Stop();
                }
            }
            catch (Exception ex)
            {
                mLogger.Error($"Error while stoping process:{model?.Path}, due to:{ex}");
            }
        }

        internal void TryClear()
        {
            HamburgerMenuModel.Content.Clear();
            HasContent = false;
        }

        public void Dispose()
        {
            mProcessOperator?.Dispose();
            Content.Clear();
        }
    }
}
