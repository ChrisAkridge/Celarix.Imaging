using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground
{
    internal sealed class ProgressBarProgress : IProgress<int>
    {
        private ProgressBar progressBar;

        public ProgressBarProgress(ProgressBar progressBar)
        {
            this.progressBar = progressBar ?? throw new ArgumentNullException(nameof(progressBar));
        }

        public void Report(int value)
        {
            if (progressBar.InvokeRequired)
            {
                progressBar.Invoke(new Action(() => progressBar.Value = value));
            }
            else
            {
                progressBar.Value = value;
            }
        }
    }
}
