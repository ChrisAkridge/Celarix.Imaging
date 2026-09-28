using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground.Rendering.v2
{
    public sealed class StripedFactoryOptions : FactoryOptions
    {
        public override LoadedImageKind Kind => LoadedImageKind.Striped;

        public int StripeIndex { get; }

        public StripedFactoryOptions(CancellationToken cancellationToken, int stripeIndex) : base(cancellationToken)
        {
            StripeIndex = stripeIndex;
        }

        public override string ToString()
        {
            return $"StripedFactoryOptions[StripeIndex={StripeIndex}]";
        }
    }
}
