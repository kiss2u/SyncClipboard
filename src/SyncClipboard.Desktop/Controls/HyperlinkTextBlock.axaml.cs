using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Input;
using Avalonia.Media;
using SyncClipboard.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SyncClipboard.Desktop.Controls;

/// <summary>
/// 增强的文本显示控件，自动检测并渲染超链接
/// 支持文本选择、链接点击、鼠标悬浮手型光标
/// </summary>
public partial class HyperlinkTextBlock : UserControl
{
    // 存储链接信息：起始位置、结束位置、URL
    private readonly List<LinkInfo> _linkInfos = [];

    private class LinkInfo
    {
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public string Url { get; set; } = string.Empty;
    }

    /// <summary>
    /// 需要解析链接的文本依赖属性
    /// </summary>
    public static readonly StyledProperty<string> LinkTextProperty =
        AvaloniaProperty.Register<HyperlinkTextBlock, string>(nameof(LinkText));

    /// <summary>
    /// 需要解析链接的文本
    /// </summary>
    public string LinkText
    {
        get => GetValue(LinkTextProperty);
        set => SetValue(LinkTextProperty, value);
    }

    /// <summary>
    /// 链接前景色依赖属性
    /// </summary>
    public static readonly StyledProperty<IBrush> LinkForegroundProperty =
        AvaloniaProperty.Register<HyperlinkTextBlock, IBrush>(nameof(LinkForeground), Brushes.Blue);

    /// <summary>
    /// 链接前景色
    /// </summary>
    public IBrush LinkForeground
    {
        get => GetValue(LinkForegroundProperty);
        set => SetValue(LinkForegroundProperty, value);
    }

    /// <summary>
    /// 是否显示链接下划线依赖属性
    /// </summary>
    public static readonly StyledProperty<bool> ShowLinkUnderlineProperty =
        AvaloniaProperty.Register<HyperlinkTextBlock, bool>(nameof(ShowLinkUnderline), true);

    /// <summary>
    /// 是否显示链接下划线
    /// </summary>
    public bool ShowLinkUnderline
    {
        get => GetValue(ShowLinkUnderlineProperty);
        set => SetValue(ShowLinkUnderlineProperty, value);
    }

    /// <summary>
    /// 文本换行依赖属性
    /// </summary>
    public static readonly StyledProperty<TextWrapping> TextWrappingProperty =
        AvaloniaProperty.Register<HyperlinkTextBlock, TextWrapping>(nameof(TextWrapping), TextWrapping.Wrap);

    /// <summary>
    /// 文本换行
    /// </summary>
    public TextWrapping TextWrapping
    {
        get => GetValue(TextWrappingProperty);
        set => SetValue(TextWrappingProperty, value);
    }

    public HyperlinkTextBlock()
    {
        InitializeComponent();

        // 监听 Tapped 事件处理链接点击
        _TextBlock.Tapped += OnTapped;
        // 监听 PointerMoved 事件处理鼠标悬浮光标
        _TextBlock.PointerMoved += OnPointerMoved;
        // 监听 PointerExited 事件恢复默认光标
        _TextBlock.PointerExited += OnPointerExited;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        // 当 LinkText 属性变化时，重新解析并渲染链接
        if (change.Property == LinkTextProperty)
        {
            ParseAndRenderLinks();
        }
        // 当链接样式属性变化时，重新渲染
        else if (change.Property == LinkForegroundProperty || change.Property == ShowLinkUnderlineProperty)
        {
            if (!string.IsNullOrEmpty(LinkText))
            {
                ParseAndRenderLinks();
            }
        }
        // 当 TextWrapping 属性变化时，更新内部控件
        else if (change.Property == TextWrappingProperty)
        {
            _TextBlock.TextWrapping = TextWrapping;
        }
    }

    /// <summary>
    /// 解析文本中的 URL 并渲染为带链接样式的 Inlines
    /// </summary>
    private void ParseAndRenderLinks()
    {
        // 清除之前的选择状态
        _TextBlock.SelectionStart = 0;
        _TextBlock.SelectionEnd = 0;

        _TextBlock.Inlines?.Clear();
        _linkInfos.Clear();

        var text = LinkText;
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        int currentIndex = 0;
        var matches = StringHelper.UrlRegex().Matches(text);

        foreach (Match match in matches)
        {
            // 添加链接前的普通文本
            if (match.Index > currentIndex)
            {
                var plainText = text[currentIndex..match.Index];
                _TextBlock.Inlines?.Add(new Run(plainText));
                currentIndex = match.Index;
            }

            // 添加链接文本（使用 Underline 或 Span）
            var linkText = match.Value;
            Inline linkInline;
            if (ShowLinkUnderline)
            {
                var linkUnderline = new Underline();
                linkUnderline.Inlines.Add(new Run(linkText));
                linkUnderline.Foreground = LinkForeground;
                linkInline = linkUnderline;
            }
            else
            {
                var linkSpan = new Span();
                linkSpan.Inlines.Add(new Run(linkText));
                linkSpan.Foreground = LinkForeground;
                linkInline = linkSpan;
            }

            _TextBlock.Inlines?.Add(linkInline);

            // 记录链接信息
            _linkInfos.Add(new LinkInfo
            {
                StartIndex = currentIndex,
                EndIndex = currentIndex + linkText.Length,
                Url = linkText
            });

            currentIndex += linkText.Length;
        }

        // 添加剩余的普通文本
        if (currentIndex < text.Length)
        {
            var remainingText = text[currentIndex..];
            _TextBlock.Inlines?.Add(new Run(remainingText));
        }
    }

    /// <summary>
    /// 处理点击事件，检测是否点击了链接
    /// </summary>
    private void OnTapped(object? sender, TappedEventArgs e)
    {
        if (_linkInfos.Count == 0)
            return;

        var point = e.GetPosition(_TextBlock);
        var textLayout = _TextBlock.TextLayout;
        if (textLayout == null)
            return;

        var hitTestResult = textLayout.HitTestPoint(point);
        var characterIndex = hitTestResult.TextPosition;

        // 检查点击位置是否在某个链接范围内
        foreach (var linkInfo in _linkInfos)
        {
            if (characterIndex >= linkInfo.StartIndex && characterIndex < linkInfo.EndIndex)
            {
                _ = OpenUrlAsync(linkInfo.Url);
                e.Handled = true;
                return;
            }
        }
    }

    /// <summary>
    /// 处理鼠标移动事件，检测是否悬浮在链接上
    /// </summary>
    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        var point = e.GetPosition(_TextBlock);
        var textLayout = _TextBlock.TextLayout;
        if (textLayout == null)
        {
            _TextBlock.Cursor = Cursor.Default;
            return;
        }

        var hitTestResult = textLayout.HitTestPoint(point);
        var characterIndex = hitTestResult.TextPosition;

        // 检查鼠标位置是否在某个链接范围内
        bool isOverLink = false;
        foreach (var linkInfo in _linkInfos)
        {
            if (characterIndex >= linkInfo.StartIndex && characterIndex < linkInfo.EndIndex)
            {
                isOverLink = true;
                break;
            }
        }

        // 设置光标：链接上显示手型，普通文字显示 caret
        _TextBlock.Cursor = isOverLink
            ? new Cursor(StandardCursorType.Hand)
            : new Cursor(StandardCursorType.Ibeam);
    }

    /// <summary>
    /// 处理鼠标离开事件，恢复默认光标
    /// </summary>
    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        _TextBlock.Cursor = Cursor.Default;
    }

    /// <summary>
    /// 打开 URL
    /// </summary>
    private async Task OpenUrlAsync(string url)
    {
        try
        {
            if (TopLevel.GetTopLevel(this) is { } topLevel)
            {
                await topLevel.Launcher.LaunchUriAsync(new Uri(url));
            }
        }
        catch (Exception)
        {
            // 忽略打开 URL 失败的情况
        }
    }
}