using System;
using System.Windows.Input;

namespace MemoryGame.Wpf
{
    public class RelayCommand : ICommand // ICommand implementation for simple command binding
    {
        private readonly Action<object?> _execute; //Action to execute

        public RelayCommand(Action<object?> execute) //Constructor accepting the action to execute
        {
            _execute = execute;
        }

        public bool CanExecute(object? parameter) => true; //Always executable
        public void Execute(object? parameter) => _execute(parameter); //Invoke the action

        public event EventHandler? CanExecuteChanged; //CanExecuteChanged event (not used here, but required by ICommand)
    }
}