﻿using System;
using System.Windows.Input;

namespace Xbim.InformationSpecifications.UI.mvvm
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
            return canExecute == null || canExecute(parameter);
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
