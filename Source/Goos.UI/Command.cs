using System;
using System.Windows.Input;

namespace Goos.UI
{
    public class Command : ICommand
    {
        private readonly Action execute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }


        public Command(Action execute)
        {
            this.execute = execute;
        }


        public void Execute(object parameter)
        {
            execute();
        }


        public bool CanExecute(object parameter)
        {
            return true;
        }
    }
}
