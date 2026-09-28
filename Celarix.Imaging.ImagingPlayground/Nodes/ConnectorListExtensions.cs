using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground.Nodes
{
    internal static class ConnectorListExtensions
    {
        public static ValueConnector<T> Required<T>(this IEnumerable<ValueConnector> connectors, string name)
        {
            return connectors
                .OfType<ValueConnector<T>>()
                .SingleOrDefault(c => c.Name == name)
                ?? throw new ArgumentException($"Missing required connector '{name}' of type {typeof(T).Name}");
        }

        public static ValueConnector<T>? Optional<T>(this IEnumerable<ValueConnector> connectors, string name)
        {
            return connectors
                .OfType<ValueConnector<T>>()
                .SingleOrDefault(c => c.Name == name);
        }
    }
}
