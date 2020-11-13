using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AsyncAwaitWithMVVM
{
  /// <summary>
  /// This class represents a basic command that runs an action
  /// </summary>
  public class RelayCommand : ICommand
  {
    #region Fields
    private readonly Action _execute;
    private readonly Func<bool> _canExecute;
    private bool _canExecuteCache = true;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="RelayCommand"/> class.
    /// </summary>  
    public RelayCommand(Action execute, Func<bool> canExecute = null)
    {
      _execute = execute;
      _canExecute = canExecute;
    }
    #endregion

    #region Event
    /// <summary>
    /// The event thats fired when the <see cref="CanExecute(object)"/> value has changed
    /// </summary>
    public event EventHandler CanExecuteChanged = (sender, e) => { };
    #endregion

    #region ICommand
    /// <summary>
    /// A relay command can execute or not
    /// </summary>
    public bool CanExecute(object parameter)
    {
      if (_canExecute != null)
      {
        bool canExecute = _canExecute();

        if (_canExecuteCache != canExecute)
        {
          _canExecuteCache = canExecute;

          if (CanExecuteChanged != null)
          {
            CanExecuteChanged(this, new EventArgs());
          }
        }
      }

      return _canExecuteCache;
    }

    /// <summary>
    /// Executes the commands Action
    /// </summary>
    public void Execute(object parameter)
    {
      _execute();
    }
    #endregion
  }
  public class TaskRelayCommand : ICommand
  {
    #region Fields
    private Action<Task> _task;

    private Func<bool> _canExecute;
    private bool _canExecuteCache = true;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="RelayCommand"/> class.
    /// </summary>  

    public TaskRelayCommand(Action<Task> execute, Func<bool> canExecute = null)
    {
      _task = execute;
      _canExecute = canExecute;
    }
    #endregion

    #region Event
    /// <summary>
    /// The event thats fired when the <see cref="CanExecute(object)"/> value has changed
    /// </summary>
    public event EventHandler CanExecuteChanged = (sender, e) => { };
    #endregion

    #region ICommand
    /// <summary>
    /// A relay command can execute or not
    /// </summary>
    public bool CanExecute(object parameter)
    {
      if (_canExecute != null)
      {
        bool canExecute = _canExecute();

        if (_canExecuteCache != canExecute)
        {
          _canExecuteCache = canExecute;

          if (CanExecuteChanged != null)
          {
            CanExecuteChanged(this, new EventArgs());
          }
        }
      }

      return _canExecuteCache;
    }

    /// <summary>
    /// Executes the commands Action
    /// </summary>
    public void Execute(object parameter)
    {
        //_task();
    }
    #endregion
  }
}
