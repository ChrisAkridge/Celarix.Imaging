using Celarix.Imaging.ImagingPlayground.Rendering.v2;
using Celarix.Imaging.ImagingPlayground.Rendering.v2.Computed;
using System.Reflection;

namespace Celarix.Imaging.ImagingPlayground
{
    internal sealed class ComputedZoomableCanvasSelectorForm : Form
    {
        private readonly ListBox ListComputedCanvases;
        private readonly Button ButtonOK;
        private readonly Button ButtonCancel;

        public IZoomableCanvasSource? SelectedSource
            => ListComputedCanvases.SelectedItem is ComputedCanvasListItem item
                ? item.Source
                : null;

        public ComputedZoomableCanvasSelectorForm()
        {
            Text = "Open Computed Zoomable Canvas";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            ClientSize = new Size(420, 320);

            ListComputedCanvases = new ListBox
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                DisplayMember = nameof(ComputedCanvasListItem.Name),
                Location = new Point(12, 12),
                Size = new Size(396, 264)
            };
            ButtonOK = new Button
            {
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                DialogResult = DialogResult.OK,
                Enabled = false,
                Location = new Point(252, 286),
                Size = new Size(75, 23),
                Text = "OK",
                UseVisualStyleBackColor = true
            };

            ButtonCancel = new Button
            {
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                DialogResult = DialogResult.Cancel,
                Location = new Point(333, 286),
                Size = new Size(75, 23),
                Text = "Cancel",
                UseVisualStyleBackColor = true
            };

            ListComputedCanvases.SelectedIndexChanged += (_, _) =>
            {
                ButtonOK.Enabled = ListComputedCanvases.SelectedItem != null;
            };
            ListComputedCanvases.DoubleClick += (_, _) =>
            {
                if (ListComputedCanvases.SelectedItem != null)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
            };

            AcceptButton = ButtonOK;
            CancelButton = ButtonCancel;
            Controls.Add(ListComputedCanvases);
            Controls.Add(ButtonOK);
            Controls.Add(ButtonCancel);

            LoadComputedCanvasSources();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.None)
            {
                DialogResult = DialogResult.Cancel;
            }

            base.OnFormClosing(e);
        }

        private void LoadComputedCanvasSources()
        {
            var computedNamespace = typeof(TestZoomableCanvasSource).Namespace;
            var items = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(GetLoadableTypes)
                .Where(t => t.Namespace == computedNamespace
                    && typeof(IZoomableCanvasSource).IsAssignableFrom(t)
                    && !t.IsInterface
                    && !t.IsAbstract
                    && t.GetConstructor(Type.EmptyTypes) != null)
                .Select(CreateListItem)
                .OfType<ComputedCanvasListItem>()
                .OrderBy(item => item.Name)
                .ToArray();

            ListComputedCanvases.Items.AddRange(items);
        }

        private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.OfType<Type>();
            }
        }

        private static ComputedCanvasListItem? CreateListItem(Type sourceType)
        {
            if (Activator.CreateInstance(sourceType) is not IZoomableCanvasSource source)
            {
                return null;
            }

            return new ComputedCanvasListItem(source.Name, source);
        }

        private sealed class ComputedCanvasListItem
        {
            public string Name { get; }
            public IZoomableCanvasSource Source { get; }

            public ComputedCanvasListItem(string name, IZoomableCanvasSource source)
            {
                Name = name;
                Source = source;
            }
        }
    }
}
