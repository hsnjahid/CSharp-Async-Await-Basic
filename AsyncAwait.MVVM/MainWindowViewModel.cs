using AsyncAwait.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncAwait.MVVM
{
  public class MainWindowViewModel : ViewModelBase
  {
    #region Fields
    #endregion

    #region Commands
    public RelayCommand GetNumberCommand { get; set; }

    private IWorker Worker { get; } = new Worker();

    public int Number { get; set; }

    public bool IsBusy { get; set; }
    #endregion

    #region Properties
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>  
    public MainWindowViewModel()
    {
      GetNumberCommand = new RelayCommand(async () => 
      {
        IsBusy = true;
        OnPropertyChanged(nameof(IsBusy));
        Number =  await Worker.GetRandomNumberAsync();
        OnPropertyChanged(nameof(Number));
        IsBusy = false;
        OnPropertyChanged(nameof(IsBusy));
      });
    }
    #endregion

    #region Methods
    #endregion

    #region Private Helpers
    #endregion
  }
}
