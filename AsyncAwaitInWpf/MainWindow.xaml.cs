using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace AsyncAwaitInWpf
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private DispatcherTimer _timer = new DispatcherTimer();
    public bool IsConfigureAwaitCallingTask { get; set; } = true;
    public bool IsConfigureAwaitNesedTask { get; set; } = true;
    public bool IsConfigureAwaitDelayTask { get; set; } = true;
    public bool IsConfigureAwaitChildTask { get; set; } = true;
    public int Count { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
      _timer.Interval = TimeSpan.FromSeconds(1);
      _timer.Tick += (sender, e) => TextBlock_Counter.Text = Count++.ToString();
      _timer.Start();

      InitializeComponent();
      DataContext = this;
    }

    /// <summary>
    /// Blocks main thread than show log message.
    /// </summary>
    private void OnButtonClickHandleByMainThread(object sender, RoutedEventArgs e)
    {
      Reset(); // clear and reset
      Log("App froze for 5 seconds and border changed to Cyan.");
      Thread.Sleep(TimeSpan.FromSeconds(5));
      TryChangeBorderBrush();
    }

    /// <summary>
    /// Starts new thread which delays logging is interactive (does not block gui thread)
    /// </summary>
    private void OnButtonClickHandleByWorkerThread(object sender, RoutedEventArgs e)
    {
      Reset(); // clear and reset
      Log("Before worker thread");
      // start new thread
      new Thread(() =>
      {
        Log("Inside worker thread, wait for 5 seconds, then border changed to Cyan | gui shall be interactive");
        Thread.Sleep(TimeSpan.FromSeconds(5));
        TryChangeBorderBrush();
        Log("Leaving worker thread...");
      }).Start();

      Log("After worker thread | does not wait for worker to finish");
    }

    /// <summary>
    /// Start new task and asynchronous work (does not block gui thread)
    /// </summary>
    private async void OnButtonClickHandleByAsyncAwait(object sender, RoutedEventArgs e)
    {
      Reset(); // clear and reset

      Log("Before async method");
      // await
      await DoSomethingAsync().ConfigureAwait(IsConfigureAwaitCallingTask);

      Log("After async method...");

      TryChangeBorderBrush();
    }

    /// <summary>
    /// Start nested tasks and asynchronous work (does not block gui thread)
    /// </summary>
    private async void OnButtonClickHandleByNestedAsyncAwait(object sender, RoutedEventArgs e)
    {
      Reset(); // clear and reset

      Log("Before nested async method");
      // await
      await DoSomethingNestedAsync().ConfigureAwait(IsConfigureAwaitCallingTask);

      Log("After nested async method...");

      TryChangeBorderBrush();
    }

    /// <summary>
    /// Start a new task and wait until a random number is received.
    /// </summary>
    private async void OnButtonClickAwaitLibAsyncMethod(object sender, RoutedEventArgs e)
    {
      AsyncAwaitInLibrary.IWorker worker = new AsyncAwaitInLibrary.Worker();
      Log("Before DoSomethingAsync method from class lib");
      await Task.Run(async ()=>
      {
        Log("Inside task | will wait 5 seconds");
        await worker.DoSomethingAsync();
        Log("Now try to wait a random delay");
        var num = await worker.GetRandomNumberAsync();
        Log($"Awaited '{num}' ms");
        Log("Exiting task...");
      });

      Log("After DoSomethingAsync method...");
      TryChangeBorderBrush();
    }

    /// <summary>
    /// Produce dead lock.
    /// </summary>
    private void OnButtonClickProduceDeadlock(object sender, RoutedEventArgs e)
    {
      Log("Before dead lock set ConfigureAwait to false");
      var task = DoSomethingAsync();
      Log("Main thread wait to finish task");
      task.Wait();
    }

    // Do something asynchronously | nested task
    private async Task DoSomethingNestedAsync()
    {
      Log("Inside DoSomethingNestedAsync() method which runs synchronously | start new task");
      // new task await
      await Task.Run(async () =>
      {
        Log("Inside parent task");

        await Task.Run(() =>
        {
          Log("Inside child task will wait 5 seconds ");
          Thread.Sleep(TimeSpan.FromSeconds(5));
          Log("Exiting child task...");
        }).ConfigureAwait(IsConfigureAwaitChildTask);
        Log("Exiting parent task......");
      }).ConfigureAwait(IsConfigureAwaitNesedTask);
    }

    // Do something asynchronous
    private async Task DoSomethingAsync()
    {
      Log("Inside DoSomethingAsync() method runs synchronously| start new task will wait 5 seconds | gui shall be interactive");
      await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(IsConfigureAwaitDelayTask);
      Log("After async delay | apeared after 5 seconds");
    }

    // Clean logging text and reset border brush to white
    private void Reset()
    {
      // clear and reset
      LoggingTextBlock.Text = string.Empty;
      LoggingTextBlock.Background = Brushes.White;
      LoggingBorder.BorderBrush = Brushes.White;
    }

    // Try to change boder color of the logging text block. 
    private void TryChangeBorderBrush()
    {
      try
      {
        LoggingTextBlock.Background = Brushes.LightGoldenrodYellow;
        LoggingBorder.BorderBrush = Brushes.DarkCyan;
      }
      catch (Exception ex)
      {
        Log($"Error on changing background {Environment.NewLine}{ex.Message}", isError: true);
      }
    }

    // Entry log message.
    private void Log(string message, bool isError = false, int? givenId = null)
    {
      var logMsg = string.Format("{0}{1} [ {2} - {3}]",
        Environment.NewLine,
        message,
        givenId ?? Thread.CurrentThread.ManagedThreadId,
        Thread.CurrentThread.IsBackground? "Background" : "Foreground"
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
