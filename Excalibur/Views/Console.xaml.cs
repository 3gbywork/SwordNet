using CommonUtility.Command;
using CommonUtility.Config;
using Excalibur.Business;
using Excalibur.Config;
using Excalibur.Models;
using Excalibur.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Windows.Controls;

namespace Excalibur.Views
{
    /// <summary>
    /// Console.xaml 的交互逻辑
    /// </summary>
    public partial class Console : UserControl, IDisposable
    {
        MenuModel mMenuModel = new MenuModel();
        Dictionary<string, ConsoleOperator> mConsoles = new Dictionary<string, ConsoleOperator>();

        public Console()
        {
            UI.Culture = Thread.CurrentThread.CurrentUICulture;

            InitializeComponent();

            mMenuModel.StartButtonTitle = UI.Console_StartButtonTitle;
            mMenuModel.StopButtonTitle = UI.Console_StopButtonTitle;
            mMenuModel.ClearButtonTitle = UI.Console_ClearButtonTitle;

            mMenuModel.ConsoleActionCommand = new RelayCommand<object>(ExecuteAction);

            ConsoleConfigEntity entity = new ConsoleConfigEntity();
            entity.OnConfigChanged += OnConfigChanged;
            XmlConfigurator.ConfigAndWatch(entity, new FileInfo("config/ConsoleConfig.xml"));
        }

        private void OnConfigChanged(ICollection<ConsoleModel> consoles)
        {
            if (consoles == null)
            {
                return;
            }

            this.Dispatcher.Invoke(new Action(() =>
            {
                var collection = new ObservableCollection<ListBoxItem>();
                foreach (var console in consoles)
                {
                    collection.Add(GetItem(console));
                }
                Menu.ItemsSource = collection;
                if (collection.Count > 0 && Menu.SelectedIndex == -1)
                {
                    Menu.SelectedIndex = 0;
                }
            }));
        }

        private ListBoxItem GetItem(ConsoleModel console)
        {
            return new ListBoxItem()
            {
                Content = console.Name,
                Tag = console,
            };
        }

        private void Menu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listbox)
            {
                if (listbox.SelectedItem is ListBoxItem item)
                {
                    if (item.Tag is ConsoleModel model)
                    {
                        if (!mConsoles.TryGetValue(model.Path, out ConsoleOperator console))
                        {
                            console = new ConsoleOperator(model, mMenuModel.Clone(), this.Dispatcher);
                            mConsoles[model.Path] = console;
                        }
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            ConsoleGrid.DataContext = console.MenuModel;
                        }));
                    }
                }
            }
        }

        private void ExecuteAction(object parameter)
        {
            if (ConsoleGrid.DataContext is MenuModel model)
            {
                if (mConsoles.TryGetValue(model.Path, out ConsoleOperator console))
                {
                    switch (parameter.ToString().ToLower())
                    {
                        case "start":
                            console.TryRun();
                            break;
                        case "stop":
                            console.TryStop();
                            break;
                        case "clear":
                            console.TryClear();
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public void Dispose()
        {
            foreach (var console in mConsoles)
            {
                console.Value?.Dispose();
            }
            mConsoles.Clear();
            mConsoles = null;
        }
    }
}
