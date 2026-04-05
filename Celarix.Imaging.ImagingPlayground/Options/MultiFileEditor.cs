using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground.Options
{
    public sealed class MultiFileEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
        {
            using var dialog = new MultiFileSelectorForm();
            return dialog.ShowDialog() == DialogResult.OK ? new Models.FileList(dialog.FilePaths) : value;
        }
    }
}
