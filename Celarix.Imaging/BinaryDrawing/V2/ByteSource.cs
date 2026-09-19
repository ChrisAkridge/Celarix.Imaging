using System;
using System.Collections.Generic;
using System.IO;

namespace Celarix.Imaging.BinaryDrawing.V2
{
    public sealed class ByteSource : IDisposable
    {
        private readonly IReadOnlyList<IStreamProvider> streamProviders;
        private int currentProviderIndex;
        private Stream currentStream;
        private long position;
        
        public long Position => position;
        public long Length { get; }
        public ByteSourceState State { get; private set; }
        public string CurrentStreamName => currentProviderIndex < streamProviders.Count ? streamProviders[currentProviderIndex].Name : null;

        public ByteSource(IReadOnlyList<IStreamProvider> streamProviders)
        {
            this.streamProviders = streamProviders ?? throw new ArgumentNullException(nameof(streamProviders));
            long totalLength = 0;
            foreach (var provider in this.streamProviders)
            {
                totalLength += provider.Length;
            }
            Length = totalLength;

            if (this.streamProviders.Count == 0)
            {
                State = ByteSourceState.EndOfStreams;
            }
            else
            {
                LoadCurrentStream();
                State = ByteSourceState.Default;
            }
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            if (State == ByteSourceState.EndOfStreams)
            {
                return 0;
            }
            
            // If we are previously sitting at a boundary and the caller requested more, we clear the boundary state.
            if (State == ByteSourceState.StreamBoundary)
            {
                State = ByteSourceState.Default;
            }

            int bytesRead = currentStream.Read(buffer, offset, count);
            position += bytesRead;

            if (bytesRead == 0 || currentStream.Position >= currentStream.Length)
            {
                // We reached the end of this stream. Advance so that CurrentStreamName reflects the next stream.
                AdvanceStream();
                State = currentStream != null 
                    ? ByteSourceState.StreamBoundary 
                    : ByteSourceState.EndOfStreams;
            }

            return bytesRead;
        }

        public void Seek(long offset, SeekOrigin origin)
        {
            if (streamProviders.Count == 0) return;

            // Calculate absolute target position
            long targetPosition = origin switch
            {
                SeekOrigin.Begin => offset,
                SeekOrigin.Current => position + offset,
                SeekOrigin.End => Length + offset,
                _ => throw new ArgumentException("Invalid origin", nameof(origin))
            };

            if (targetPosition < 0 || targetPosition > Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Seek position is out of bounds.");
            }

            // Find which stream this position falls into
            long accumulatedLength = 0;
            for (int i = 0; i < streamProviders.Count; i++)
            {
                var provider = streamProviders[i];
                if (targetPosition >= accumulatedLength && targetPosition < accumulatedLength + provider.Length ||
                    (targetPosition == Length && i == streamProviders.Count - 1))
                {
                    if (currentProviderIndex != i)
                    {
                        DisposeCurrentStreamIfNeeded();
                        currentProviderIndex = i;
                        currentStream = streamProviders[currentProviderIndex].GetStream();
                    }
                    
                    long offsetInStream = targetPosition - accumulatedLength;
                    if (currentStream.CanSeek)
                    {
                        currentStream.Seek(offsetInStream, SeekOrigin.Begin);
                    }
                    else
                    {
                        throw new NotSupportedException($"Stream from provider {provider.Name} does not support seeking.");
                    }
                    
                    position = targetPosition;
                    
                    State = (position == Length) ? ByteSourceState.EndOfStreams : ByteSourceState.Default;
                    return;
                }
                accumulatedLength += provider.Length;
            }
        }

        private void LoadCurrentStream()
        {
            if (currentProviderIndex < streamProviders.Count)
            {
                currentStream = streamProviders[currentProviderIndex].GetStream();
            }
            else
            {
                currentStream = null;
            }
        }

        private void AdvanceStream()
        {
            DisposeCurrentStreamIfNeeded();
            currentProviderIndex++;
            LoadCurrentStream();
        }

        private void DisposeCurrentStreamIfNeeded()
        {
            if (currentStream != null && currentProviderIndex < streamProviders.Count)
            {
                if (streamProviders[currentProviderIndex].DisposeOnAdvance)
                {
                    currentStream.Dispose();
                }
                currentStream = null;
            }
        }

        public void Dispose()
        {
            DisposeCurrentStreamIfNeeded();
        }
    }
}
