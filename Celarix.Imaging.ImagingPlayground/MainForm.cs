using Celarix.Imaging.ImagingPlayground.Operations;
using Celarix.Imaging.ImagingPlayground.Options;
using Celarix.Imaging.ImagingPlayground.Rendering.v2;
using Serilog;
using Serilog.Formatting.Display;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.PixelFormats;

namespace Celarix.Imaging.ImagingPlayground
{
    public partial class MainForm : Form
    {
        private MasterOptions options = new();
        private List<IOperation> operations = new();
        private IOperation? lastOperation = null;
        private CancellationTokenSource? cancellationTokenSource;
        private WinFormsTextBoxLogSink? uiSink;

        public MainForm()
        {
            InitializeComponent();
            options.CanvasMaxMemoryMBChanged += (s, newValue) =>
            {
                InfiniteCanvas?.SetSoftMemoryLimit(newValue * 1024L * 1024L);
                Log($"Canvas max memory set to {newValue} MB");
            };
        }

        private void ConfigureLogging()
        {
            // Configure file sink for Serilog
            var pcName = Environment.MachineName;
            var logFolderPath = pcName.Equals("STARFLOWER08", StringComparison.OrdinalIgnoreCase)
                ? @"E:\Documents\Files\Programming\Logs\ImagingPlayground"
                : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ImagingPlayground", "Logs");

            Directory.CreateDirectory(logFolderPath);
            var logFilePath = Path.Combine(logFolderPath, $"imaging-playground-.log");
            Serilog.Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.File(
                    logFilePath,
                    rollingInterval: RollingInterval.Day,
                    shared: true
                )
                .WriteTo.Sink(uiSink ?? throw new ArgumentNullException(nameof(uiSink), "Invalid program initialization; tried to configure logging before UI load"))
                .CreateLogger();
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
            const int defaultButtonCount = 4;
            var currentY = buttonMargin + ((23 + buttonMargin) * defaultButtonCount);
            foreach (var operation in operations)
            {
                var button = new Button
                {
                    Text = operation.Name,
                    Location = new System.Drawing.Point(buttonMargin, currentY),
                    Size = new System.Drawing.Size(250, 23)
                };
                button.Click += (s, args) => RunOperation(operation);
                SplitOperationsSecond.Panel1.Controls.Add(button);
                currentY += button.Height + buttonMargin;
            }

            // Logging setup
            uiSink = new WinFormsTextBoxLogSink(new MessageTemplateTextFormatter(
                "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"), TextLog);
            ConfigureLogging();
            Serilog.Log.Information("Logging registered. Application started.");
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
                SetImage,
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

        private void SetImage(Image<Rgba32> image)
        {
            if (InvokeRequired)
            {
                Invoke(new SetImageDelegate(SetImage), image);
                return;
            }

            InfiniteCanvas.LoadInMemoryImage(image);
        }

        private void ButtonOpenImage_Click(object sender, EventArgs e)
        {
            if (OFDMain.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    InfiniteCanvas.LoadSingleImage(OFDMain.FileName);
                    Log($"Loaded image: {OFDMain.FileName}");
                }
                catch (Exception ex)
                {
                    Log($"Error loading image: {ex.Message}");
                }
            }
        }

        private void InfiniteCanvas_Click(object sender, EventArgs e)
        {
            // Temporary for debugging
            InfiniteCanvas.Invalidate();
        }

        private void ButtonOpenZoomableCanvas_Click(object sender, EventArgs e)
        {
            if (FBDZoomableCanvas.ShowDialog() == DialogResult.OK)
            {
                var zoomableCanvasPath = FBDZoomableCanvas.SelectedPath;
                try
                {
                    var source = new DiskZoomableCanvasSource(zoomableCanvasPath);
                    InfiniteCanvas.LoadZoomableCanvas(source);
                    Log($"Loaded zoomable canvas from: {zoomableCanvasPath}");
                }
                catch (Exception ex)
                {
                    Log($"Error loading zoomable canvas: {ex.Message}");
                }
            }
        }

        private void ButtonOpenComputedZoomableCanvas_Click(object sender, EventArgs e)
        {
            using var dialog = new ComputedZoomableCanvasSelectorForm();
            if (dialog.ShowDialog(this) != DialogResult.OK || dialog.SelectedSource == null)
            {
                return;
            }

            InfiniteCanvas.LoadZoomableCanvas(dialog.SelectedSource);
            Log($"Loaded computed zoomable canvas: {dialog.SelectedSource.Name}");
        }

        private void LogFlushTimer_Tick(object sender, EventArgs e)
        {
            uiSink.Flush();
        }

        private void ButtonSetSingleFile_Click(object sender, EventArgs e)
        {
            if (OFDMain.ShowDialog() == DialogResult.OK)
            {
                options.Files = new Models.FileList([OFDMain.FileName]);
                MainProperties.Invalidate();
            }
        }
    }
}
