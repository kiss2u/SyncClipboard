using SyncClipboard.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SyncClipboard.Desktop.ClipboardAva.Fingerprint;

/// <summary>
/// Windows 平台剪贴板指纹提供者（不使用指纹机制）
/// </summary>
internal class WindowsClipboardFingerprintProvider : IClipboardFingerprintProvider
{
    public Task<int?> GetClipboardFingerprint(CancellationToken ctk)
    {
        // Windows 不使用指纹机制，返回 null
        return Task.FromResult<int?>(null);
    }
}