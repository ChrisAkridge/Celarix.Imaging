using Celarix.Imaging.ImagingPlayground.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground.Operations
{
    internal sealed class OperationRunOptions
    {
        public MasterOptions MasterOptions { get; }
        public IProgress<int> Progress { get; }
        public LoggingDelegate Logger { get; }
        public SetBitmapDelegate SetImage { get; }
        public CancellationToken CancellationToken { get; }

        public OperationRunOptions(MasterOptions masterOptions,
            IProgress<int> progress,
            LoggingDelegate logger,
            SetBitmapDelegate setImage,
            CancellationToken cancellationToken)
        {
            MasterOptions = masterOptions ?? throw new ArgumentNullException(nameof(masterOptions));
            Progress = progress ?? throw new ArgumentNullException(nameof(progress));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            SetImage = setImage ?? throw new ArgumentNullException(nameof(setImage));
            CancellationToken = cancellationToken;
        }
    }
}
