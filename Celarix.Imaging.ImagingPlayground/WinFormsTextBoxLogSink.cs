using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground
{
    internal sealed class WinFormsTextBoxLogSink : ILogEventSink, IDisposable
    {
        private const int MaxTextBoxLines = 10000;

        private readonly ConcurrentQueue<string> pendingLines = new();
        private readonly ITextFormatter formatter;
        private readonly int maxQueuedLines;

        private readonly TextBox textBox;

        public WinFormsTextBoxLogSink(ITextFormatter formatter, TextBox textBox, int maxQueuedLines = 1000)
        {
            this.formatter = formatter;
            this.maxQueuedLines = maxQueuedLines;
            this.textBox = textBox;
        }

        public void Emit(LogEvent logEvent)
        {
            if (pendingLines.Count >= maxQueuedLines) { return; }

            using var writer = new StringWriter();
            formatter.Format(logEvent, writer);
            pendingLines.Enqueue(writer.ToString());
        }

        public void Flush()
        {
            var incomingLines = new StringBuilder();
            var count = 0;

            while (count < MaxTextBoxLines && pendingLines.TryDequeue(out var line))
            {
                incomingLines.Append(line);

                count += 1;
            }

            if (incomingLines.Length == 0)
            {
                return;
            }

            textBox.AppendText(incomingLines.ToString());

            var lines = textBox.Lines;
            if (lines.Length > MaxTextBoxLines)
            {
                textBox.Lines = lines
                    .Skip(lines.Length - MaxTextBoxLines)
                    .ToArray();
            }

            textBox.SelectionStart = textBox.TextLength;
            textBox.ScrollToCaret();
        }

        public void Dispose()
        {
            // No unmanaged resources to dispose, but implement IDisposable to allow for future enhancements and to follow the standard pattern for log sinks.
        }
    }
}
