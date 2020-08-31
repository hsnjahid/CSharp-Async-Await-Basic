using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwaitInConsole
{
  class Program
  {
    /// <summary>
    /// Main function
    /// </summary>
    static async Task Main(string[] args)
    {
      // ---------- Thread -----------
      Log("Before new thread");
      new Thread(() => DoSomething()).Start();
      Log("After new thread");
      
      // wait for separate output
      Thread.Sleep(4000);
      Console.WriteLine("------------------------------------------");

      // ---------- Task -----------
      Log("Before new task");
      await DoSomethingAsync();
      Log("After new task");

      Console.WriteLine("------------------------------------------");
      Log("Application exiting....");
      Console.ReadLine();
    }

    /// <summary>
    /// Delays 3s
    /// </summary>
    static void DoSomething()
    {
      Log("Inside worker thread, waits for 3 seconds");
      Thread.Sleep(3000);
      Log("Exiting worker thread");
    }

    /// <summary>
    /// Delays 3s then change text
    /// </summary>
    static async Task DoSomethingAsync()
    {
      Log("Inside task, waits for 3 seconds");
      await Task.Delay(3000);
      Log("Exiting task");
    }

    /// <summary>
    /// Log message to console
    /// </summary>
    static void Log(string message)
    {
      var logMsg = string.Format("{0} | Current Thread : [{1}]",
        message, Thread.CurrentThread.ManagedThreadId);

      Console.WriteLine(logMsg);
    }
  }
}
