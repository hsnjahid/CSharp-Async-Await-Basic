using System.Diagnostics;
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
    private string _text;

    public MainWindow()
    {
      InitializeComponent();
    }

    /// <summary>
    /// On button click delays for 1s after that changes text (freezes gui)
    /// </summary>
    private void OnButtonClickChangeTextNormally(object sender, RoutedEventArgs e)
    {
      DisplayTextBlock.Text = "";
      Thread.Sleep(1000);
      _text = "Hello world";
      DisplayTextBlock.Text = _text;
    }

    /// <summary>
    /// On button click starts new thread and delays for 1s after that changes text (does not freezes gui)
    /// </summary>
    private void OnButtonClickChangeTextThreaded(object sender, RoutedEventArgs e)
    {
      // clear text
      DisplayTextBlock.Text = "";
      Trace.WriteLine($"Before thread | Current thread id : { Thread.CurrentThread.ManagedThreadId}");
      
      // start new thread
      new Thread(() =>
      {
        Trace.WriteLine($"Inside thread | Current thread id : { Thread.CurrentThread.ManagedThreadId}");
        Thread.Sleep(1000);

        // invoke ui thread
        Application.Current.Dispatcher.Invoke(() =>
        {
          DisplayTextBlock.Text = "Hello world";
        });

        Trace.WriteLine($"Thread finished | Current thread id : { Thread.CurrentThread.ManagedThreadId}");

      }).Start();

      Trace.WriteLine($"After thread | Current thread id : { Thread.CurrentThread.ManagedThreadId}");
    }

    /// <summary>
    /// On button click changes text async (does not freezes gui)
    /// </summary>
    private async void OnButtonClickChangeTextAwaited(object sender, RoutedEventArgs e)
    {
      // clear text
      DisplayTextBlock.Text = "";
      Trace.WriteLine($"Before task | Current thread id : { Thread.CurrentThread.ManagedThreadId}");
      // await
      await DoSomethingAsync();
      Trace.WriteLine($"After task | Current thread id : { Thread.CurrentThread.ManagedThreadId}");
      // changes text 
      DisplayTextBlock.Text = _text;
    }

    /// <summary>
    /// Do something asynchronous (does not freezes gui as well)
    /// </summary>
    private async Task DoSomethingAsync()
    {
      Trace.WriteLine($"Inside task | Current thread id : { Thread.CurrentThread.ManagedThreadId}");

      // wait 1s
      await Task.Delay(1000);

      _text = "Hello world";
      Trace.WriteLine($"Task finished | Current thread id : { Thread.CurrentThread.ManagedThreadId}");
    }
  }
}
