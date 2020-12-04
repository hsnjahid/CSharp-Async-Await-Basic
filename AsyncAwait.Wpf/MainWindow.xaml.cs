using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace AsyncAwait.Wpf
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    #region Fields
    private DispatcherTimer _timer = new DispatcherTimer();
    private CancellationTokenSource _tokenSource = null;
    #endregion

    #region Properties
    public bool IsConfigureAwaitForParent { get; set; } = true;
    public bool IsConfigureAwaitForChild { get; set; } = false;
    public int Count { get; private set; }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
      InitializeComponent();
      _timer.Interval = TimeSpan.FromSeconds(1);
      _timer.Tick += (sender, e) => TextBlock_Counter.Text = Count++.ToString();
      _timer.Start();
      DataContext = this;
    }

    #region Button Handlers
    // blocks main thread than show log message.
    private void ButtonMainHandled_Click(object sender, RoutedEventArgs e)
    {
      Reset(); // clear and reset
      Log("App froze for 5 seconds and border brush changed.");
      Thread.Sleep(TimeSpan.FromSeconds(5));
      TryChangeLoggingPane();
    }

    // starts new thread which delays logging is interactive (does not block gui thread)
    private void ButtonWorkerHandled_Click(object sender, RoutedEventArgs e)
    {
      Reset(); // clear and reset
      Log("Before worker thread");
      // start new thread
      new Thread(() =>
      {
        Log("Inside worker thread -> wait for 5 seconds, then try to change border brush --> App remains interactive");
        Thread.Sleep(TimeSpan.FromSeconds(5));
        TryChangeLoggingPane();
        Log("Leaving worker thread...");
      }).Start();

      Log("After worker thread -> does not wait for worker to finish");
    }

    // starts a new task and do asynchronous work (does not block gui thread)
    private async void ButtonTaskHandled_ClickAsync(object sender, RoutedEventArgs e)
    {
      Reset(); // clear and reset

      Log("Before async method");
      await DoSomethingAsync().ConfigureAwait(IsConfigureAwaitForParent); // await
      Log("After async method...");

      TryChangeLoggingPane();
    }

    // start a new task and wait until a random number is received.
    private async void ButtonClassLibHandled_ClickAsync(object sender, RoutedEventArgs e)
    {
      Reset();
      // create worker 
      AsyncAwait.Lib.IWorker worker = new AsyncAwait.Lib.Worker();

      Log("Before new task.");
      await Task.Run(async () =>
      {
        Log("Before async method from class library.");
        await worker.DoSomethingAsync();
        Log("After async method, now try to get a random number");
        var num = await worker.GetNumberAsync();
        Log($"Required '{num}' ms to get the value");
      });
      Log("After the task.");
      TryChangeLoggingPane();
    }

    // produce dead lock.
    private void ButtonDeadlock_Click(object sender, RoutedEventArgs e)
    {
      Log("Before dead lock set ConfigureAwait to false");
      var task = DoSomethingAsync();
      Log("Main thread wait to finish task");
      task.Wait();
    }
    #endregion

    // delay calling thread for five seconds asynchronous
    private async Task DoSomethingAsync()
    {
      Log("Inside async method -> runs on calling thread, starts new task will wait five seconds -> App remains interactive");
      await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(IsConfigureAwaitForChild);
      Log("After async task delay -> waited five seconds");
    }

    // reset logging pane
    private void Reset()
    {
      // clear and reset
      Border_Logging.BorderBrush = Brushes.White;
      TextBlock_Logging.Background = Brushes.White;
      TextBlock_Logging.Text = string.Empty;
    }

    // try to change border color and background of the logging text block.
    private void TryChangeLoggingPane()
    {
      try
      {
        TextBlock_Logging.Background = Brushes.LightGoldenrodYellow;
        Border_Logging.BorderBrush = Brushes.DarkCyan;
      }
      catch (Exception ex)
      {
        Log($"Error on changing logging pane \'{ex.Message}\'", isError: true);
      }
    }

    // log a message.
    private void Log(string message, bool isError = false, int? givenId = null)
    {
      // message
      var logMsg = string.Format("{0}{1} [{2}][{3}]",
        Environment.NewLine,
        message,
        Thread.CurrentThread.IsBackground ? "B" : "F",
        givenId ?? Thread.CurrentThread.ManagedThreadId
        );

      // display message in GUI thread
      bool isGuiThread = Application.Current.Dispatcher == Dispatcher.CurrentDispatcher;

      if (!isGuiThread)
      {
        Application.Current.Dispatcher.Invoke(new Action(() =>
        {
          TextBlock_Logging.Text += logMsg;

          // change border brush in error case
          if (isError)
          {
            TextBlock_Logging.Background = Brushes.LightPink;
            Border_Logging.BorderBrush = Brushes.OrangeRed;
          }
        }));
      }
      else
      {
        TextBlock_Logging.Text += logMsg;
      }
    }

    #region IProgress
    // start a new async task and report running progress
    private async void ButtonStart_Click(object sender, RoutedEventArgs e)
    {
      _tokenSource = new CancellationTokenSource();

      var token = _tokenSource.Token;

      var progress = new Progress<int>(value =>
      {
        ProgressBar_Worker.Value = value;
        TextBlock_Progress.Text = $"{value}%";
      });

      try
      {
        await Task.Run(() => LoopThroughNumbers(500, progress, token));
      }
      catch (OperationCanceledException)
      {
        TextBlock_Progress.Text = "Canelled";
      }
      finally
      {
        _tokenSource.Dispose();
      }
    }

    // cancel running task
    private void ButtonCancel_Click(object sender, RoutedEventArgs e)
    {
      if (_tokenSource != null)
      {
        _tokenSource.Cancel();
      }
    }

    // loop throgh numbers and wait in each iteration
    private void LoopThroughNumbers(int count, IProgress<int> progress, CancellationToken token)
    {
      Enumerable.Range(1, count).ToList().ForEach(idx =>
      {
        Thread.Sleep(10);
        var parcentageComplete = (idx * 100) / count;
        progress.Report(parcentageComplete);

        // check in each iteration
        if (token.IsCancellationRequested)
        {
          token.ThrowIfCancellationRequested();
        }
      });
    }
    #endregion
  }
}
