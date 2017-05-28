﻿using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Excalibur.Models
{
    public class ConsoleModel
    {
        public string Alias { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Param { get; set; }
    }

    public class HamburgerMenuModel : ModelBase
    {
        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                DoNotification(nameof(Title));
            }
        }

        private ObservableCollection<string> content;
        public ObservableCollection<string> Content
        {
            get => content;
            set
            {
                content = value;
                DoNotification(nameof(Content));
            }
        }

        #region 按钮状态
        private bool startButtonState = true;
        public bool StartButtonState
        {
            get => startButtonState;
            set
            {
                startButtonState = value;
                DoNotification(nameof(StartButtonState));
            }
        }

        private bool stopButtonState;
        public bool StopButtonState
        {
            get => stopButtonState;
            set
            {
                stopButtonState = value;
                DoNotification(nameof(StopButtonState));
            }
        }

        private bool clearButtonState;
        public bool ClearButtonState
        {
            get => clearButtonState;
            set
            {
                clearButtonState = value;
                DoNotification(nameof(ClearButtonState));
            }
        }
        #endregion

        #region 按钮显示名称
        private string startButtonTitle;
        public string StartButtonTitle
        {
            get => startButtonTitle;
            set
            {
                startButtonTitle = value;
                DoNotification(nameof(StartButtonTitle));
            }
        }

        private string stopButtonTitle;
        public string StopButtonTitle
        {
            get => stopButtonTitle;
            set
            {
                stopButtonTitle = value;
                DoNotification(nameof(StopButtonTitle));
            }
        }

        private string clearButtonTitle;
        public string ClearButtonTitle
        {
            get => clearButtonTitle;
            set
            {
                clearButtonTitle = value;
                DoNotification(nameof(ClearButtonTitle));
            }
        }

        #endregion

        #region 按钮Command
        private ICommand consoleActionCommand;
        public ICommand ConsoleActionCommand
        {
            get => consoleActionCommand;
            set
            {
                consoleActionCommand = value;
                DoNotification(nameof(ConsoleActionCommand));
            }
            #endregion
        }

        public string Path { get; internal set; }

        internal HamburgerMenuModel Clone()
        {
            return new HamburgerMenuModel
            {
                Title = this.Title,
                Path = this.Path,
                StartButtonTitle = this.StartButtonTitle,
                StopButtonTitle = this.StopButtonTitle,
                ClearButtonTitle = this.ClearButtonTitle,
                ConsoleActionCommand = this.ConsoleActionCommand,
            };
        }
    }
}
