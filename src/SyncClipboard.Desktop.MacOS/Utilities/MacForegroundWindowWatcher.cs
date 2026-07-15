using AppKit;
using Foundation;
using SyncClipboard.Core.Interfaces;
using System;

namespace SyncClipboard.Desktop.MacOS.Utilities;

internal sealed class MacForegroundWindowWatcher : IForegroundWindowWatcher
{
    private NSObject? _observer;

    public event Action? ForegroundWindowChanged;

    public void Start()
    {
        _observer ??= NSNotificationCenter.DefaultCenter.AddObserver(
            NSWorkspace.DidActivateApplicationNotification,
            _ => ForegroundWindowChanged?.Invoke());
    }

    public void Stop()
    {
        if (_observer is null)
        {
            return;
        }

        NSNotificationCenter.DefaultCenter.RemoveObserver(_observer);
        _observer.Dispose();
        _observer = null;
    }

    public void Dispose() => Stop();
}
