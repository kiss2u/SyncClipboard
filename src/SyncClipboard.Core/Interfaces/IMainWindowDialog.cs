namespace SyncClipboard.Core.Interfaces;

public interface IMainWindowDialog
{
    Task<bool> ShowConfirmationAsync(string title, string message);
    Task ShowMessageAsync(string title, string message);
}