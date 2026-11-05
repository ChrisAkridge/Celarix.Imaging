using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;

namespace Celarix.Imaging.ByteViewObservatory;

internal sealed class MainForm : Form
{
    private readonly WebView2 webView;

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

        await webView.EnsureCoreWebView2Async();
        webView.NavigateToString(
            "<!doctype html>" +
            "<html><head><meta charset=\"utf-8\"><title>ByteView Observatory</title></head>" +
            "<body style=\"margin:0;background:#171717;\"></body></html>");
    }
}
