using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace AsyncAwaitInWpf
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window, INotifyPropertyChanged
  {
    /// <summary>
    /// Gets current thread Id
    /// </summary>
    public bool _isContinueOnCapturedContext = true;

    public bool IsConfigureAwaitTask { get; set; }
    public bool IsConfigureAwaitNesedTask { get; set; }
    public bool IsConfigureAwaitDelayTask { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged(string name)
    {
      PropertyChanged(this, new PropertyChangedEventArgs(name));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
      InitializeComponent();
      DataContext = this;
    }

    /// <summary>
    /// On checked
    /// </summary>am>
    private void ConfigureAwait_Checked(object sender, RoutedEventArgs e)
    {
      _isContinueOnCapturedContext = true;
    }

    /// <summary>
    /// On unchecked
    /// </summary>
    private void ConfigureAwait_Unchecked(object sender, RoutedEventArgs e)
    {
      _isContinueOnCapturedContext = false;
    }

    /// <summary>
    /// On button click delays for 3s after log messages (blocks gui thread)
    /// </summary>
    private void OnButtonClickHandleByMainThread(object sender, RoutedEventArgs e)
    {
      Reset(); // clear and reset
      Log("App freezes for 3 seconds and border changed to red.");
      Thread.Sleep(3000);
      TryChangeBorderBrush();
    }

    /// <summary>
    /// On button click starts new thread and delays for 5s, logging is interactive (does not block gui thread)
    /// </summary>
    private void OnButtonClickHandleByWorkerThread(object sender, RoutedEventArgs e)
    {
      Reset(); // clear and reset
      Log("Before worker thread");
      // start new thread
      new Thread(() =>
      {
        Log("Inside worker thread, wait for 3 seconds, then border changed to red");
        Thread.Sleep(3000);
        TryChangeBorderBrush();
        Log("Leaving worker thread");
      }).Start();

      Log("After worker thread");
    }

    /// <summary>
    /// On button click start new task and asynchronous do something (does not block gui thread)
    /// </summary>
    private async void OnButtonClickHandleByAsyncAwait(object sender, RoutedEventArgs e)
    {
      Reset(); // clear and reset

      Log("Before parent task");
      // await
      await DoSomethingAsync();

      Log("After parent task");

      TryChangeBorderBrush();
    }


    /// <summary>
    /// On button click start nested tasks and asynchronous do something (does not block gui thread)
    /// </summary>
    private async void OnButtonClickHandleByNestedAsyncAwait(object sender, RoutedEventArgs e)
    {
      Reset(); // clear and reset

      Log("Before parent task");
      // await
      await DoSomethingNestedAsync();

      Log("After parent task");

      TryChangeBorderBrush();
    }

    private void OnButtonClickProduceDeadlock(object sender, RoutedEventArgs e)
    {
      Log("Before dead lock set ConfigureAwait to false");
      var task = DoSomethingAsync();
      Log("Main thread wait to finish task");
      task.Wait();
    }

    /// <summary>
    /// Do something asynchronously | nested task
    /// </summary>
    private async Task DoSomethingNestedAsync()
    {
      // new task await
      await Task.Run(async () =>
      {
        Log("Inside task");
        // await
        await DoSomethingAsync();

        Log("Exiting task");
      }).ConfigureAwait(IsConfigureAwaitNesedTask);
    }

    /// <summary>
    /// Do something asynchronous
    /// </summary>
    private async Task DoSomethingAsync()
    {
      Log("Before async delay for 3 seconds");
      await Task.Delay(3000).ConfigureAwait(IsConfigureAwaitDelayTask);
      Log("After async delay");
    }

    /// <summary>
    /// Clean logging text and reset border brush to white
    /// </summary>
    private void Reset()
    {
      // clear and reset
      LoggingTextBlock.Text = string.Empty;
      LoggingTextBlock.Background = Brushes.White;
      LoggingBorder.BorderBrush = Brushes.White;
    }

    /// <summary>
    /// Try to change boder color of the logging text block. 
    /// </summary>
    private void TryChangeBorderBrush()
    {
      try
      {
        LoggingTextBlock.Background = Brushes.LightGoldenrodYellow;
        LoggingBorder.BorderBrush = Brushes.DarkCyan;
      }
      catch (Exception ex)
      {
        Log($"Error on changing background\n{ex.Message}", isError: true);
      }
    }

    /// <summary>
    /// Log messages.
    /// </summary>
    private void Log(string message, bool isError = false, int? givenId = null)
    {
      var logMsg = string.Format("{0}{1} | Current Thread - Background : [{2}][{3}]",
        Environment.NewLine,
        message,
        Thread.CurrentThread.IsBackground,
        givenId.HasValue ? givenId.Value : Thread.CurrentThread.ManagedThreadId
        );

      // display message in GUI thread
      bool isInGuiThread = Application.Current.Dispatcher == System.Windows.Threading.Dispatcher.CurrentDispatcher;

      if (!isInGuiThread)
      {
        Application.Current.Dispatcher.Invoke(new Action(() =>
        {
          LoggingTextBlock.Text += logMsg;
          if(isError)
            LoggingBorder.BorderBrush = Brushes.OrangeRed;
        }));
      }
      else
      {
        LoggingTextBlock.Text += logMsg;
      }
    }


  }
}
