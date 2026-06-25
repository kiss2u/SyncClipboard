using System.Threading;
using System.Threading.Tasks;

namespace SyncClipboard.Core.Interfaces;

/// <summary>
/// 剪贴板指纹提供者接口（轻量级，用于快速检测剪贴板是否变化）
/// </summary>
public interface IClipboardFingerprintProvider
{
    /// <summary>
    /// 获取剪贴板指纹
    /// Linux: 返回 TimeStamp
    /// macOS: 返回 ChangeCount
    /// Windows: 返回 null（不使用指纹机制）
    /// </summary>
    Task<int?> GetClipboardFingerprint(CancellationToken ctk);
}