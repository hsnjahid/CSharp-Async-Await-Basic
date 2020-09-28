using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwaitInLibrary
{
  public class Worker : IWorker
  {
    async Task IWorker.DoSomethingAsync()
    {
      await Task.Run(() =>
      {
        Thread.Sleep(5000);
      }).ConfigureAwait(false);
    }

    async Task<int> IWorker.GetRandomNumberAsync()
    {
      int num = int.MaxValue; 
      await Task.Run(() =>
      {
        // get a random number between 3K to 5K
        num = new Random().Next(3000, 5000);
        Thread.Sleep(num);
      }).ConfigureAwait(false);

      return num;
    }
  }
}
