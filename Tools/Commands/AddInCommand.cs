using System;
using System.Windows.Input;
using Tools.Business;
using Tools.Models;

namespace Tools.Commands
{
    class AddInCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (parameter == null)
            {
                return true;
            }

            var param = parameter as IRibbonButtonInfo;
            return param != null;
        }

        public void Execute(object parameter)
        {
            RibbonButtonUtility.Execute(parameter);
        }
    }
}
