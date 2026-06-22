using SyncClipboard.Core.Clipboard;
using SyncClipboard.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

namespace SyncClipboard.WinUI3.ClipboardWinUI;

internal class FileClipboardSetter : ClipboardSetterBase<FileProfile>, IClipboardSetter<GroupProfile>
{
    public static string[] UnusualType = [".lnk", ".url", ".wsh"];

    public override async Task FillPackage(object package, ClipboardMetaInfomation metaInfomation)
    {
        if (metaInfomation.Files is null || metaInfomation.Files.Length == 0)
        {
            throw new ArgumentException("Not Contain File.");
        }

        if (package is DataPackage dataPackage)
        {
            List<IStorageItem> list = [];
            foreach (var file in metaInfomation.Files)
            {
                if (Directory.Exists(file))
                {
                    list.Add(await StorageFolder.GetFolderFromPathAsync(file));
                }
                else if (IsUnusualType(file))
                {
                    list.Add(new UnusualStorageItem(file));
                }
                else
                {
                    list.Add(await StorageFile.GetFileFromPathAsync(file));
                }
            }

            dataPackage.SetStorageItems(list, false);
        }
    }

    public static bool IsUnusualType(string file)
    {
        var exention = Path.GetExtension(file).ToLower();
        foreach (var type in UnusualType)
        {
            if (exention == type)
                return true;
        }
        return false;
    }
}
