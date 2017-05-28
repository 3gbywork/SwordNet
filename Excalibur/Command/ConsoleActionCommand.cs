using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Excalibur.Command
{
    class ConsoleActionCommand : ICommand
    {
        private Action<object> executeAction;

        public ConsoleActionCommand(Action<object> executeAction)
        {
            this.executeAction = executeAction;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (this.executeAction != null)
            {
                executeAction.Invoke(parameter);
            }
        }
    }
}
