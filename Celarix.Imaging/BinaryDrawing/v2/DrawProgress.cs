using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.BinaryDrawing.v2
{
    public sealed class DrawProgress
    {
        public DrawProgressPhase Phase { get; }
        public int PercentComplete { get; }
        public string Message { get; }

        public DrawProgress(DrawProgressPhase phase, int percentComplete, string message)
        {
            Phase = phase;
            PercentComplete = percentComplete;
            Message = message;
        }
    }
}
