using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using SyncClipboard.Core;
using SyncClipboard.Core.Models;
using SyncClipboard.Core.Utilities;
using SyncClipboard.Core.ViewModels;
using SyncClipboard.Core.ViewModels.Sub;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

namespace SyncClipboard.Desktop.Controls;

/// <summary>
/// 预览面板控件，用于显示剪贴板记录的预览内容
/// </summary>
public sealed partial class PreviewPanel : UserControl
{
    private (uint width, uint height) _currentPreviewImageSize = (0, 0);

    /// <summary>
    /// ViewModel依赖属性
    /// </summary>
    public static readonly StyledProperty<HistoryViewModel> ViewModelProperty =
        AvaloniaProperty.Register<PreviewPanel, HistoryViewModel>(nameof(ViewModel));

    /// <summary>
    /// SelectedItem依赖属性
    /// </summary>
    public static readonly StyledProperty<HistoryRecordVM> SelectedItemProperty =
        AvaloniaProperty.Register<PreviewPanel, HistoryRecordVM>(nameof(SelectedItem));

    public HistoryViewModel ViewModel
    {
        get => GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public HistoryRecordVM SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public PreviewPanel()
    {
        InitializeComponent();

        // 监听属性变化
        PropertyChanged += OnPropertyChanged;
    }

    private void OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == ViewModelProperty)
        {
            if (e.NewValue is HistoryViewModel viewModel)
            {
                // 监听ViewModel的ShowPreviewPanel变化
                viewModel.PropertyChanged += (s, args) =>
                {
                    if (args.PropertyName == nameof(HistoryViewModel.ShowPreviewPanel))
                    {
                        // 打开预览面板时，立即更新图片预览内容
                        if (viewModel.ShowPreviewPanel)
                        {
                            _ = UpdatePreviewImageContent();
                        }
                    }
                    else if (args.PropertyName == nameof(HistoryViewModel.EnableSyncHistory))
                    {
                        UpdateSyncedTextVisibility();
                    }
                };
            }
        }
        else if (e.Property == SelectedItemProperty)
        {
            // 取消订阅旧选中项的事件
            if (e.OldValue is HistoryRecordVM oldRecord)
            {
                oldRecord.PropertyChanged -= SelectedItem_PropertyChanged;
            }

            // 订阅新选中项的事件
            if (e.NewValue is HistoryRecordVM newRecord)
            {
                newRecord.PropertyChanged += SelectedItem_PropertyChanged;
            }

            _ = UpdatePreviewImageContent();
            UpdateSyncedTextVisibility();
        }
    }

    private void SelectedItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(HistoryRecordVM.PreviewImage))
        {
            _ = UpdatePreviewImageContent();
        }
    }

    private void PreviewPanel_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        // 预览面板大小变化时，重新计算图片 Stretch
        if (_currentPreviewImageSize.width > 0 && _currentPreviewImageSize.height > 0)
        {
            UpdatePreviewImageStretch();
        }
    }

    /// <summary>
    /// 只处理图片预览的异步加载，文字预览通过绑定自动更新
    /// </summary>
    private async Task UpdatePreviewImageContent()
    {
        if (ViewModel?.ShowPreviewPanel != true)
            return;

        // 清空之前的图片预览
        _PreviewImage.Source = null;
        _currentPreviewImageSize = (0, 0);

        // 检查是否选中了记录
        if (SelectedItem is not HistoryRecordVM record)
            return;

        // 只有图片类型且有预览图片时才异步加载
        if (record.Type != ProfileType.Image || record.PreviewImage == null)
            return;

        try
        {
            var (width, height) = await GetImageDimensions(record.PreviewImage);

            // 检查当前选中项是否还是同一个记录
            if (SelectedItem != record)
                return;

            // 保存图片尺寸，用于面板大小变化时重新计算
            _currentPreviewImageSize = (width, height);

            // 计算并设置 Stretch
            UpdatePreviewImageStretch();

            // 设置图片源
            _PreviewImage.Source = new Bitmap(record.PreviewImage);
        }
        catch (Exception ex)
        {
            AppCore.TryGetCurrent()?.Logger.Write("PreviewPanel", $"Failed to get image dimensions: {ex.Message}");

            // 检查当前选中项是否还是同一个记录
            if (SelectedItem != record)
                return;

            _currentPreviewImageSize = (0, 0);
            _PreviewImage.Stretch = Stretch.Uniform;
            _PreviewImage.Source = new Bitmap(record.PreviewImage);
        }
    }

    private void UpdatePreviewImageStretch()
    {
        var (width, height) = _currentPreviewImageSize;
        if (width == 0 || height == 0)
            return;

        // 获取预览面板的实际大小
        var panelWidth = Bounds.Width - 24;
        var panelHeight = Bounds.Height - 48; // 减去头部高度

        // 根据图片大小决定 Stretch
        if (width > panelWidth || height > panelHeight)
        {
            _PreviewImage.Stretch = Stretch.Uniform;
        }
        else
        {
            _PreviewImage.Stretch = Stretch.None;
        }
    }

    private void UpdateSyncedTextVisibility()
    {
        if (ViewModel == null || _SyncedText == null)
            return;

        var enableSync = ViewModel.EnableSyncHistory;
        var isSynced = SelectedItem?.SyncState == SyncStatus.Synced;

        _SyncedText.IsVisible = enableSync && isSynced;
    }

    private static async Task<(uint width, uint height)> GetImageDimensions(string imagePath)
    {
        using var stream = File.OpenRead(imagePath);
        using var bitmap = new Bitmap(stream);
        return ((uint)bitmap.PixelSize.Width, (uint)bitmap.PixelSize.Height);
    }

    private void PreviewImage_DoubleTapped(object? sender, TappedEventArgs e)
    {
        if (SelectedItem is HistoryRecordVM record && record.Type == ProfileType.Image && record.PreviewImage != null)
        {
            ViewModel?.ViewImage(record);
        }
    }
}