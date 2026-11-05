using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace Celarix.Imaging.ByteViewObservatory;

internal sealed class MainForm : Form
{
    private readonly WebView2 webView;
    private WebViewBridge? bridge;

    public MainForm()
    {
        Text = "ByteView Observatory";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(1200, 800);

        webView = new WebView2
        {
            Dock = DockStyle.Fill
        };

        Controls.Add(webView);
    }

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        try
        {
            await webView.EnsureCoreWebView2Async();

            string webRoot = Path.Combine(AppContext.BaseDirectory, "wwwroot");
            webView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                "byteview.local",
                webRoot,
                CoreWebView2HostResourceAccessKind.DenyCors);

            bridge = new WebViewBridge(webView.CoreWebView2);
            bridge.MessageReceived += Bridge_MessageReceived;

            webView.Source = new Uri("https://byteview.local/index.html");
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                this,
                $"ByteView Observatory could not initialize its WebView2 surface.\n\n{exception.Message}",
                "ByteView Observatory",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && bridge is not null)
        {
            bridge.MessageReceived -= Bridge_MessageReceived;
            bridge.Dispose();
            bridge = null;
        }

        base.Dispose(disposing);
    }

    private void Bridge_MessageReceived(object? sender, WebViewMessage message)
    {
        if (message.Type == "page.ready")
        {
            bridge?.SendHello();
        }
    }
}
