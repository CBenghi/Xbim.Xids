using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Xbim.IDS.UI.mvvm
{
	public class ActionCommand : ICommand
    {
        readonly Action<object> execute;
        readonly Predicate<object> canExecute;

        public ActionCommand(Action<object> executeDelegate, Predicate<object> canExecuteDelegate = null)
        {
            execute = executeDelegate;
            canExecute = canExecuteDelegate;
        }

        bool ICommand.CanExecute(object parameter)
        {
            return canExecute == null ? true : canExecute(parameter);
        }

        event EventHandler ICommand.CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        void ICommand.Execute(object parameter)
        {
            execute(parameter);
        }
    }
}
