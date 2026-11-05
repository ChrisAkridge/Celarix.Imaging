using System.Globalization;
using System.Text.Json;
using Microsoft.Web.WebView2.Core;

namespace Celarix.Imaging.ByteViewObservatory;

internal sealed record WebViewMessage(
    string Type,
    string? RequestId,
    JsonElement Payload);

internal sealed class WebViewBridge : IDisposable
{
    private readonly CoreWebView2 webView;

    public WebViewBridge(CoreWebView2 webView)
    {
        this.webView = webView;
        this.webView.WebMessageReceived += HandleWebMessageReceived;
    }

    public event EventHandler<WebViewMessage>? MessageReceived;

    public void SendHello()
    {
        Send(
            "host.hello",
            new
            {
                application = "ByteView Observatory",
                protocolVersion = 1
            });
    }

    public void SendOffset(ulong byteOffset, byte bitIndex, string? requestId = null)
    {
        Send(
            "host.offset",
            new
            {
                byteOffset = byteOffset.ToString(CultureInfo.InvariantCulture),
                bitIndex
            },
            requestId);
    }

    public void SendProgress(
        string requestId,
        ulong completed,
        ulong total,
        bool isComplete)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(requestId);

        Send(
            "host.progress",
            new
            {
                completed = completed.ToString(CultureInfo.InvariantCulture),
                total = total.ToString(CultureInfo.InvariantCulture),
                isComplete
            },
            requestId);
    }

    public void SendError(string message, string? requestId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        Send("host.error", new { message }, requestId);
    }

    public void Dispose()
    {
        webView.WebMessageReceived -= HandleWebMessageReceived;
    }

    private void Send(string type, object payload, string? requestId = null)
    {
        string message = JsonSerializer.Serialize(new
        {
            type,
            requestId,
            payload
        });

        webView.PostWebMessageAsJson(message);
    }

    private void HandleWebMessageReceived(
        object? sender,
        CoreWebView2WebMessageReceivedEventArgs eventArgs)
    {
        try
        {
            using JsonDocument document = JsonDocument.Parse(eventArgs.WebMessageAsJson);
            JsonElement root = document.RootElement;

            if (root.ValueKind != JsonValueKind.Object ||
                !root.TryGetProperty("type", out JsonElement typeElement) ||
                typeElement.ValueKind != JsonValueKind.String)
            {
                return;
            }

            string? type = typeElement.GetString();
            if (string.IsNullOrWhiteSpace(type))
            {
                return;
            }

            string? requestId = null;
            if (root.TryGetProperty("requestId", out JsonElement requestIdElement) &&
                requestIdElement.ValueKind == JsonValueKind.String)
            {
                requestId = requestIdElement.GetString();
            }

            JsonElement payload = root.TryGetProperty("payload", out JsonElement payloadElement)
                ? payloadElement.Clone()
                : default;

            MessageReceived?.Invoke(this, new WebViewMessage(type, requestId, payload));
        }
        catch (JsonException)
        {
            SendError("The page sent an invalid bridge message.");
        }
    }
}
