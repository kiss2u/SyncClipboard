using System.ComponentModel;

namespace SyncClipboard.WinUI3.Views;

/// <summary>
/// 紧凑列表代理，作为 XAML DataTemplate 中的绑定源
/// </summary>
public class CompactListProxy : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private int _maxLines = 0;

    /// <summary>紧凑模式下的最大行数：1 表示单行，0 表示无限制</summary>
    public int MaxLines => _maxLines;

    /// <summary>是否为紧凑模式（MaxLines == 1）</summary>
    public bool IsCompact => _maxLines == 1;

    public void SetMaxLines(int maxLines)
    {
        if (_maxLines != maxLines)
        {
            _maxLines = maxLines;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MaxLines)));
        }
    }
}