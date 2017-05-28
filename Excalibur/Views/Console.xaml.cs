using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Excalibur.Config;
using Excalibur.Models;

namespace Excalibur.Views
{
    /// <summary>
    /// Console.xaml 的交互逻辑
    /// </summary>
    public partial class Console : UserControl
    {
        public Console()
        {
            InitializeComponent();

            ConsoleConfigEntity entity = new ConsoleConfigEntity();
            entity.OnConfigChanged += OnConfigChanged;
        }

        private void OnConfigChanged(ObservableCollection<ConsoleModel> consoles)
        {
            throw new NotImplementedException();
        }
    }
}
