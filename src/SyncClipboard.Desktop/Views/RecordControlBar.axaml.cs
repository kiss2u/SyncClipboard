using Avalonia;
using Avalonia.Controls;
using SyncClipboard.Core.ViewModels;
using SyncClipboard.Core.ViewModels.Sub;

namespace SyncClipboard.Desktop.Views;

public partial class RecordControlBar : UserControl
{
    public HistoryViewModel? ViewModel
    {
        get => GetValue(ViewModelProperty) as HistoryViewModel;
        set => SetValue(ViewModelProperty, value);
    }

    public static readonly StyledProperty<HistoryViewModel?> ViewModelProperty =
        AvaloniaProperty.Register<RecordControlBar, HistoryViewModel?>(nameof(ViewModel));

    public HistoryRecordVM? Record
    {
        get => GetValue(RecordProperty) as HistoryRecordVM;
        set => SetValue(RecordProperty, value);
    }

    public static readonly StyledProperty<HistoryRecordVM?> RecordProperty =
        AvaloniaProperty.Register<RecordControlBar, HistoryRecordVM?>(nameof(Record));

    public RecordControlBar()
    {
        InitializeComponent();
    }

    private void DownloadButtonClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (Record != null && ViewModel != null)
        {
            ViewModel.DownloadRemoteProfileCommand.Execute(Record);
        }
    }

    private void CancelDownloadButtonClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (Record != null && ViewModel != null)
        {
            ViewModel.CancelDownloadCommand.Execute(Record);
        }
    }

    private void UploadButtonClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (Record != null && ViewModel != null)
        {
            ViewModel.UploadLocalHistoryCommand.Execute(Record);
        }
    }

    private void CancelUploadButtonClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (Record != null && ViewModel != null)
        {
            ViewModel.CancelUploadCommand.Execute(Record);
        }
    }

    private void CopyButtonClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (Record != null && ViewModel != null)
        {
            _ = ViewModel.CopyToClipboard(Record, false, System.Threading.CancellationToken.None);
        }
    }

    private void PasteButtonClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (Record != null && ViewModel != null)
        {
            _ = ViewModel.CopyToClipboard(Record, true, System.Threading.CancellationToken.None);
        }
    }

    private void StarButtonClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (Record != null && ViewModel != null)
        {
            ViewModel.ChangeStarStatus(Record);
        }
    }

    private void DeleteButtonClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (Record != null && ViewModel != null)
        {
            ViewModel.DeleteItem(Record);
        }
    }
}