using SyncClipboard.Core.Models;

namespace SyncClipboard.Core.Clipboard;

public interface IClipboardSetter<out ProfileType> : IClipboardSetter where ProfileType : Profile
{
}

public interface IClipboardSetter
{
    /// <summary>
    /// 填充数据包。用于拖拽操作和设置剪贴板，接收一个数据包并填充数据。
    /// </summary>
    Task FillPackage(object package, ClipboardMetaInfomation metaInfomation);

    /// <summary>
    /// 将数据包设置到系统剪贴板。
    /// </summary>
    Task SetLocalClipboard(ClipboardMetaInfomation metaInfomation, CancellationToken ctk);
}
