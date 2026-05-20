using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground.Rendering.v2
{
    public abstract class FactoryOptions
    {
        public CancellationToken CancellationToken { get; protected set; }
        public abstract LoadedImageKind Kind { get; }

        protected FactoryOptions(CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;
        }
    }
}
