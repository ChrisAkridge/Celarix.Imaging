using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Celarix.Imaging.Utilities
{
    internal static class BinaryJobHelpers
    {
        public static void WriteLengthPrefixedString(BinaryWriter writer, string value)
        {
           var bytes = Encoding.UTF8.GetBytes(value);
           writer.Write(bytes.Length);
           writer.Write(bytes);
        }

        public static string ReadLengthPrefixedString(BinaryReader reader)
        {
           var length = reader.ReadInt32();
           var bytes = reader.ReadBytes(length);
           return Encoding.UTF8.GetString(bytes);
        }
    }
}
