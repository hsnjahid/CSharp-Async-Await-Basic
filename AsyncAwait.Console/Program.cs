using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwait.Con
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
      Thread.Sleep(1200);
      Console.WriteLine("------------------------------------------");

      // ---------- Task -----------
      Log("Before new task");
      await DoSomethingAsync();
      Log("After new task");


      Console.WriteLine("------------------------------------------");
      Log("Application exiting....");
      Console.ReadLine();
    }

    // Wait by blocking calling thread.
    private static void DoSomething()
    {
      Log("Inside worker thread, waits for a second");
      Thread.Sleep(1000);
      Log("Exiting worker thread");
    }

    // Wait asynchronously without blocking calling thread.
    private static async Task DoSomethingAsync()
    {
      Log("Inside task, waits for a second");
      await Task.Delay(1000);
      Log("Exiting task");
    }

    // Log message to console
    private static void Log(string message)
    {
      var logMsg = string.Format("{0} | Current Thread : [{1}]",
        message, Thread.CurrentThread.ManagedThreadId);

      Console.WriteLine(logMsg);
    }
  }
}
