using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwaitInConsole
{
  class Program
  {
    static string _text;
    static async Task Main(string[] args)
    {
      // before task
      Console.WriteLine("Before task | Text = \"{0}\" | Current thread id: [{1}]", 
        _text, Thread.CurrentThread.ManagedThreadId);

      await DoSomething();

      // after task
      Console.WriteLine("After task | Text = \"{0}\" | Current thread id: [{1}]",
        _text, Thread.CurrentThread.ManagedThreadId);

      Console.WriteLine("Finished....");
      Console.ReadLine();
    }

    /// <summary>
    /// Delays 50ms then change text
    /// </summary>
    static async Task DoSomething()
    {
      await Task.Delay(50);
      _text = "Hello world";
    }
  }
}
