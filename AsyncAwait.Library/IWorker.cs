using System.Threading.Tasks;

namespace AsyncAwait.Lib
{
  public interface IWorker
  {
    /// <summary>
    /// Do some work async.
    /// </summary>
    Task DoSomethingAsync();

    /// <summary>
    /// Return a random number async.
    /// </summary>
    Task<int> GetRandomNumberAsync();
  }
}
