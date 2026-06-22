using SyncClipboard.Core.Clipboard;
using SyncClipboard.Core.Models;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace SyncClipboard.WinUI3.ClipboardWinUI;

internal class TextClipboardSetter : ClipboardSetterBase<TextProfile>
{
    public override Task FillPackage(object package, ClipboardMetaInfomation metaInfomation)
    {
        if (package is DataPackage dataPackage)
        {
            dataPackage.SetText(metaInfomation.Text);
        }
        return Task.CompletedTask;
    }
}
