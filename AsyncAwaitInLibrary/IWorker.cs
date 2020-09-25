// <copyright>
// Technical Software Engineering Plazotta 2020
// </copyright>
//
// <author>
// TSEP / Hossain
// </author>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncAwaitInLibrary
{
  interface IWorker
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
