using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground.Operations
{
    internal interface IOperation
    {
        string Name { get; }
        Task RunAsync(OperationRunOptions options);
    }
}
