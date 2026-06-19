using System.Text.RegularExpressions;

namespace SyncClipboard.Core.Utilities;

public static partial class StringHelper
{
    /// <summary>
    /// URL 正则表达式，匹配 http/https 链接
    /// 支持域名、端口、路径、查询参数和锚点
    /// </summary>
    [GeneratedRegex(
        @"http(s)?://[a-zA-Z0-9\-_]+(\.[a-zA-Z0-9\-_]+)+(:[0-9]{1,5})?(/[a-zA-Z0-9\-_.?=&%#]*)*",
        RegexOptions.IgnoreCase)]
    public static partial Regex UrlRegex();

    public static string GetHostFromUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return url ?? string.Empty;

        if (Uri.TryCreate(url, UriKind.Absolute, out var uri) && !string.IsNullOrEmpty(uri.Host))
            return uri.Host;

        if (Uri.TryCreate("http://" + url, UriKind.Absolute, out uri) && !string.IsNullOrEmpty(uri.Host))
            return uri.Host;

        var idx = url.IndexOf(':');
        if (idx > 0)
            return url[..idx];

        return url;
    }
}
