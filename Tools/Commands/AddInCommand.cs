using System;
using System.Windows.Input;
using Tools.Business;

namespace Tools.Commands
{
    class AddInCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            RibbonButtonUtility.Execute(parameter);
        }
    }
}
