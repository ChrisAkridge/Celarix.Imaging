using Celarix.Imaging.ImagingPlayground.Operations;
using Celarix.Imaging.ImagingPlayground.Options;

namespace Celarix.Imaging.ImagingPlayground
{
    public partial class MainForm : Form
    {
        private MasterOptions options = new();
        private List<IOperation> operations = new();
        private IOperation? lastOperation = null;
        private CancellationTokenSource? cancellationTokenSource;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            MainProperties.SelectedObject = options;

            // Load operations
            var operationTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(IOperation).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .ToList();
            foreach (var operationType in operationTypes)
            {
                if (Activator.CreateInstance(operationType) is IOperation operation)
                {
                    operations.Add(operation);
                }
            }

            // Create buttons for operations
            const int buttonMargin = 10;
            const int defaultButtonCount = 1;
            var currentY = buttonMargin + (23 * defaultButtonCount);
            foreach (var operation in operations)
            {
                var button = new Button
                {
                    Text = operation.Name,
                    Location = new Point(buttonMargin, currentY),
                    Size = new Size(250, 23)
                };
                button.Click += (s, args) => RunOperation(operation);
                SplitOperationsSecond.Panel1.Controls.Add(button);
                currentY += button.Height + buttonMargin;
            }
        }

        private void RunOperation(IOperation operation)
        {
            lastOperation = operation;
            cancellationTokenSource = new CancellationTokenSource();
            ButtonRerun.Enabled = false;
            ButtonCancel.Enabled = true;

            var options = new OperationRunOptions(
                this.options,
                new ProgressBarProgress(ProgressMain),
                Log,
                SetBitmap,
                cancellationTokenSource.Token
            );

            Task.Run(async () =>
            {
                try
                {
                    await operation.RunAsync(options);
                }
                catch (OperationCanceledException)
                {
                    Log("Operation cancelled.");
                }
                catch (Exception ex)
                {
                    Log($"Error: {ex.Message}");
                }
                finally
                {
                    if (InvokeRequired)
                    {
                        Invoke(new Action(() =>
                        {
                            ButtonRerun.Enabled = true;
                            ButtonCancel.Enabled = false;
                            cancellationTokenSource = null;
                        }));
                    }
                    else
                    {
                        ButtonRerun.Enabled = true;
                        ButtonCancel.Enabled = false;
                        cancellationTokenSource = null;
                    }
                }
            });
        }

        private void Log(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new LoggingDelegate(Log), message);
                return;
            }

            string line = $"[{DateTime.Now:HH:mm:ss}] {message}";
            TextLog.AppendText(line + Environment.NewLine);
        }

        private void SetBitmap(Bitmap bitmap)
        {
            if (InvokeRequired)
            {
                Invoke(new SetBitmapDelegate(SetBitmap), bitmap);
                return;
            }

            ImageMain.Image = bitmap;
        }

        private void ButtonOpenImage_Click(object sender, EventArgs e)
        {
            if (OFDMain.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var bitmap = (Bitmap)Image.FromFile(OFDMain.FileName);
                    SetBitmap(bitmap);
                    Log($"Loaded image: {OFDMain.FileName}");
                }
                catch (Exception ex)
                {
                    Log($"Error loading image: {ex.Message}");
                }
            }
        }
    }
}
