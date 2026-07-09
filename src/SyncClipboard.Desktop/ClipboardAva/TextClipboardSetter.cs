using Avalonia.Input;
using SyncClipboard.Core.Models;
using System;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SyncClipboard.Desktop.ClipboardAva;

internal class TextClipboardSetter : ClipboardSetterBase<TextProfile>
{
    public override Task FillPackage(object package, ClipboardMetaInfomation metaInfomation)
    {
        if (package is not DataTransfer dataTransfer)
        {
            return Task.CompletedTask;
        }

        var text = metaInfomation?.Text ?? "";
        var item = new DataTransferItem();

        item.SetText(text);

        dataTransfer.Add(item);
        return Task.CompletedTask;
    }

    public override Task SetLocalClipboard(ClipboardMetaInfomation metaInfomation, CancellationToken ctk)
    {
        if (OperatingSystem.IsLinux())
        {
            return base.SetLocalClipboard(metaInfomation, ctk);
        }
        return App.Current.Clipboard.SetTextAsync(metaInfomation?.Text ?? "").WaitAsync(ctk);
    }
}
