using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground.Rendering.v2
{
    public sealed class SingleImageFactoryOptions : FactoryOptions
    {
        public override LoadedImageKind Kind => LoadedImageKind.SingleImage;

        public SingleImageFactoryOptions(CancellationToken cancellationToken) : base(cancellationToken)
        {
        }
    }
}
