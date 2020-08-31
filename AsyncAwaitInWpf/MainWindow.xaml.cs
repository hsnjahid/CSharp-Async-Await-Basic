using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace AsyncAwaitInWpf
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    /// <summary>
    /// Gets current thread Id
    /// </summary>
    public int CurrentId => Thread.CurrentThread.ManagedThreadId;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
      InitializeComponent();
    }

    /// <summary>
    /// On button click delays for 3s after log messages (blocks gui thread)
    /// </summary>
    private void OnButtonClickHandleByMainThread(object sender, RoutedEventArgs e)
    {
      LoggingTextBlock.Text = string.Empty;
      Log("Freezes application for 3 seconds.");
      Thread.Sleep(3000);
      Log("Exiting Click handler");
    }

    /// <summary>
    /// On button click starts new thread and delays for 5s, logging is interactive (does not block gui thread)
    /// </summary>
    private void OnButtonClickHandleByWorkerThread(object sender, RoutedEventArgs e)
    {
      // clear text
      LoggingTextBlock.Text = string.Empty;

      Log("Before new thread");

      // start new thread
      new Thread(() =>
      {
        Log("Inside new worker thread, wait for 3 seconds");
        Thread.Sleep(3000);
        Log("Exiting worker thread");
      }).Start();

      Log("After new thread");
    }

    /// <summary>
    /// On button click start new task and asynchronous do something (does not block gui thread)
    /// </summary>
    private async void OnButtonClickHandleByAsyncAwait(object sender, RoutedEventArgs e)
    {
      // clear text
      LoggingTextBlock.Text = string.Empty;

      Log("Before task");
      // await
      await DoSomethingAsync();

      Log("After task");
    }


    /// <summary>
    /// On button click start nested tasks and asynchronous do something (does not block gui thread)
    /// </summary>
    private async void OnButtonClickHandleByNestedAsyncAwait(object sender, RoutedEventArgs e)
    {
      // clear text
      LoggingTextBlock.Text = string.Empty;

      Log("Before parent task");

      // await
      await Task.Run(async () =>
      {
        Log("Inside parent task, wait for 3 seconds");
        await Task.Delay(3000);
        // await
        await DoSomethingAsync();

        Log("Exiting parent task");

      });

      Log("After parent task");
    }

    /// <summary>
    /// Do something asynchronous (does not freezes gui as well)
    /// </summary>
    private async Task DoSomethingAsync()
    {
      await Task.Run(async () =>
      {
        Log("Inside task, wait for 3 seconds");
        await Task.Delay(3000);
        Log("Exiting task");
      });
    }

    /// <summary>
    /// Log messages.
    /// </summary>
    private void Log(string message, int? givenId = null)
    {
      var logMsg = string.Format("{0}{1} | Current Thread : [{2}]",
        Environment.NewLine,
        message,
        givenId.HasValue ? givenId.Value : Thread.CurrentThread.ManagedThreadId
        );

      // display message in GUI thread
      bool isInGuiThread = Application.Current.Dispatcher == System.Windows.Threading.Dispatcher.CurrentDispatcher;

      if (!isInGuiThread)
      {
        Application.Current.Dispatcher.Invoke(new Action(() => LoggingTextBlock.Text += logMsg));
      }
      else
      {
        LoggingTextBlock.Text += logMsg;
      }
    }
  }
}
