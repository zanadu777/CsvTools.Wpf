using System;
using System.Windows.Input;

namespace CsvTools.Wpf.Wpf;

public class DelegateCommand : ICommand
{
  private readonly Action<object> execute;
  private readonly Func<object, bool> canExecute;

  public DelegateCommand(Action<object> execute, Func<object, bool> canExecute = null)
  {
    this.execute = execute;
    this.canExecute = canExecute ?? (_ => true);
  }

  public bool CanExecute(object parameter)
  {
    return canExecute(parameter);
  }

  public void Execute(object parameter)
  {
    execute(parameter);
  }

  public event EventHandler CanExecuteChanged;
}