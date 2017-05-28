using System;
using System.Windows.Input;

namespace Tools.Commands
{
    interface IRelayCommand : ICommand
    {
        event EventHandler Executed;
        event EventHandler Executing;
    }
}
