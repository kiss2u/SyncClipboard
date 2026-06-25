using SyncClipboard.Core.Interfaces;
using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

namespace SyncClipboard.Desktop.ClipboardAva.Fingerprint;

/// <summary>
/// macOS 平台剪贴板指纹提供者（使用 NSPasteboard.changeCount）
/// </summary>
[SupportedOSPlatform("macos")]
internal class MacOSClipboardFingerprintProvider : IClipboardFingerprintProvider
{
    [DllImport("/usr/lib/libobjc.A.dylib", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private static extern IntPtr objc_getClass(string name);

    [DllImport("/usr/lib/libobjc.A.dylib", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    private static extern IntPtr sel_registerName(string name);

    [DllImport("/usr/lib/libobjc.A.dylib")]
    private static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector);

    private static readonly IntPtr nsPasteboardClass = objc_getClass("NSPasteboard");
    private static readonly IntPtr generalPasteboardSel = sel_registerName("generalPasteboard");
    private static readonly IntPtr changeCountSel = sel_registerName("changeCount");

    public Task<int?> GetClipboardFingerprint(CancellationToken ctk)
    {
        return Task.Run<int?>(() =>
        {
            try
            {
                var pasteboard = objc_msgSend(nsPasteboardClass, generalPasteboardSel);
                if (pasteboard == IntPtr.Zero)
                    return null;

                var changeCount = objc_msgSend(pasteboard, changeCountSel);
                return (int)changeCount.ToInt64();
            }
            catch
            {
                return null;
            }
        }, ctk);
    }
}