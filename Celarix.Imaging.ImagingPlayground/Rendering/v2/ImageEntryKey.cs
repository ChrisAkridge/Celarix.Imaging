using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground.Rendering.v2
{
    internal class ImageEntryKey : IEquatable<ImageEntryKey>
    {
        public required LoadedImageKind Kind { get; init; }
        public int? ZoomLevel { get; init; }
        public required int CanvasX { get; init; }
        public required int CanvasY { get; init; }

        public override bool Equals(object? obj)
        {
            if (obj == null || obj is not ImageEntryKey key)
            {
                return false;
            }

            return Equals(key);
        }

        public bool Equals(ImageEntryKey? other)
        {
            if (other == null)
            {
                return false;
            }

            return Kind == other.Kind
                && ZoomLevel == other.ZoomLevel
                && CanvasX == other.CanvasX
                && CanvasY == other.CanvasY;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Kind, ZoomLevel, CanvasX, CanvasY);
        }

        public override string ToString()
        {
            return $"ImageEntryKey[Kind={Kind},ZoomLevel={ZoomLevel?.ToString() ?? "(none)"},CanvasX={CanvasX},CanvasY={CanvasY}]";
        }
    }
}
