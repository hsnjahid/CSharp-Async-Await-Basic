using System.Threading.Tasks;

namespace AsyncAwaitInLibrary
{
  public interface IWorker
  {
    /// <summary>
    /// Do some async work
    /// </summary>
    Task DoSomethingAsync();

    /// <summary>
    /// Return a random number async.
    /// </summary>
    Task<int> GetRandomNumberAsync();
  }
}
