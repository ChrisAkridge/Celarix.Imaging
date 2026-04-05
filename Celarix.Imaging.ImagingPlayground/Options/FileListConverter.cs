using Celarix.Imaging.ImagingPlayground.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground.Options
{
    public sealed class FileListConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext? context, [NotNullWhen(true)] Type? destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is FileList fileList)
            {
                return $"({fileList.FilePaths.Count:N0} file(s))";
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
