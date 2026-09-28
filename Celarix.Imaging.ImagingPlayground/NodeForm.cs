using Celarix.Imaging.ImagingPlayground.Nodes;
using ST.Library.UI.NodeEditor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Celarix.Imaging.ImagingPlayground
{
    public partial class NodeForm : Form
    {
        private readonly ViewerForm _viewerForm;
        private bool _dirty = false;

        public NodeForm()
        {
            InitializeComponent();

            _viewerForm = new ViewerForm();
            _viewerForm.FormClosing += (s, e) =>
            {
                if (_dirty)
                {
                    // Cancel the close and prompt the user to save changes
                    e.Cancel = true;

                    // TODO: show a dialog to ask the user if they want to save changes, discard changes, or cancel closing
                }
            };
            _viewerForm.FormClosed += (s, e) => Close();
            Instance.ViewerForm = _viewerForm;
        }

        protected void NodeForm_Load(object sender, EventArgs e)
        {
            _viewerForm.Show();

            NodePropertyGrid.Text = "Node_Property";
            NodeTreeView.LoadAssembly(typeof(NodeForm).Assembly.Location);
            NodeEditor.LoadAssembly(typeof(NodeForm).Assembly.Location);

            NodeEditor.ActiveChanged += (s, e) => NodePropertyGrid.SetNode(NodeEditor.ActiveNode);
            NodeEditor.OptionConnected += (s, e) => NodeEditor.ShowAlert(e.Status.ToString(),
                Color.White,
                e.Status == ConnectionStatus.Connected
                    ? Color.FromArgb(125, Color.Green)
                    : Color.FromArgb(125, Color.Red));
            NodeEditor.CanvasScaled += (s, e) => NodeEditor.ShowAlert(NodeEditor.CanvasScale.ToString("F2"),
                Color.White, Color.FromArgb(125, Color.Yellow));

            NodePropertyGrid.SetInfoKey("Celarix", "cakridge2@gmail.com", "https://github.com/ChrisAkridge", "ImagingPlayground");
            NodeTreeView.PropertyGrid.SetInfoKey("Celarix", "cakridge2@gmail.com", "https://github.com/ChrisAkridge", "ImagingPlayground");
        }

        protected void ButtonRunFlow_Click(object sender, EventArgs e)
        {
            var workflowRunner = new WorkflowRunner(NodeEditor.Nodes.ToArray());

            // Run the workflow asynchronously and handle any exceptions that occur
            _ = Task.Run(async () =>
            {
                try
                {
                    await workflowRunner.RunWorkflow(CancellationToken.None);
                }
                catch (Exception ex)
                {
                    // Show the exception message in a message box on the UI thread
                    Invoke(() =>
                    {
                        MessageBox.Show(this, $"An error occurred while running the workflow:\n{ex.Message}", "Workflow Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                }
            });
        }
    }
}
