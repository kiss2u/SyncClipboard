using System;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;
#nullable enable

namespace SyncClipboard.Utility.Notification
{
    public static class Toast
    {
        public static readonly ToastNotifier Notifer = ToastNotificationManager.CreateToastNotifier(Global.AppUserModelId);
        public static void SendText(string title, string text, params Button[] buttons)
        {
            new ToastSession(title) { Text1 = text, Buttons = new(buttons) }.Show();
        }

        public static void SendImage(string title, string text, Uri uri, params Button[] buttons)
        {
            var content = new ToastContentBuilder()
                .AddHeroImage(uri, "alternateText")
                .AddText(title)
                .AddText(text);
            foreach (var button in buttons)
            {
                content.AddButton(button);
            }
            content.Show();
        }
    }
}