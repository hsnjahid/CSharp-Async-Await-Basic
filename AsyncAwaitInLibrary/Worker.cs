using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwaitInLibrary
{
  public class Worker : IWorker
  {
    public async Task DoSomethingAsync()
    {
      await Task.Run(() =>
      {
        Thread.Sleep(5000);
      }).ConfigureAwait(false);
    }

    public async Task<int> GetRandomNumberAsync()
    {
      int num = 999; 
      await Task.Run(() =>
      {
        Thread.Sleep(5000);
        // get a random number between 90K to 99.9K
        num = new Random().Next(90000, 99999);
      }).ConfigureAwait(false);

      return num;
    }
  }
}
