using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using Excalibur.Business;
using Excalibur.Command;
using Excalibur.Config;
using Excalibur.Models;
using Excalibur.Resources;
using MahApps.Metro.Controls;
using Utility.Config;

namespace Excalibur.Views
{
    /// <summary>
    /// Console.xaml 的交互逻辑
    /// </summary>
    public partial class Console : UserControl, IDisposable
    {
        HamburgerMenuModel mHamburgerMenuModel = new HamburgerMenuModel();
        Dictionary<string, ConsoleOperator> mConsoles = new Dictionary<string, ConsoleOperator>();

        public Console()
        {
            InitializeComponent();

            mHamburgerMenuModel.StartButtonTitle = UI.Console_StartButtonTitle;
            mHamburgerMenuModel.StopButtonTitle = UI.Console_StopButtonTitle;
            mHamburgerMenuModel.ClearButtonTitle = UI.Console_ClearButtonTitle;

            mHamburgerMenuModel.ConsoleActionCommand = new ConsoleActionCommand(ExecuteAction);

            ConsoleConfigEntity entity = new ConsoleConfigEntity();
            entity.OnConfigChanged += OnConfigChanged;
            XmlConfigurator.ConfigAndWatch(entity, new FileInfo("config/ConsoleConfig.xml"));
        }

        private void OnConfigChanged(ObservableCollection<ConsoleModel> consoles)
        {
            DispatcherAction(MenuItemCollection.Clear);
            foreach (var console in consoles)
            {
                var menuItem = new HamburgerMenuGlyphItem()
                {
                    Glyph = console.Alias,
                    Label = console.Name,
                    Tag = console,
                };
                DispatcherAction(MenuItemCollection.Add, menuItem);
            }
            if (MenuItemCollection.Count > 0 && Menu.SelectedIndex == -1)
            {
                Menu_ItemClick(null, new ItemClickEventArgs(MenuItemCollection[0]));
            }
        }

        private void DispatcherAction(Action<HamburgerMenuItem> action, HamburgerMenuGlyphItem menuItem)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                action(menuItem);
            }));
        }

        private void DispatcherAction(Action action)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                action();
            }));
        }

        private void Menu_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is HamburgerMenuGlyphItem item)
            {
                if (item.Tag is ConsoleModel model)
                {
                    if (!mConsoles.TryGetValue(model.Path, out ConsoleOperator console))
                    {
                        console = new ConsoleOperator(model, mHamburgerMenuModel.Clone(), this.Dispatcher);
                        mConsoles[model.Path] = console;
                    }

                    Menu.Content = console.HamburgerMenuModel;
                }
            }
        }

        private void ExecuteAction(object parameter)
        {
            if (Menu.Content is HamburgerMenuModel model)
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

        private void Menu_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        public void Dispose()
        {
            foreach (var console in mConsoles)
            {
                console.Value?.Dispose();
            }
        }
    }
}
