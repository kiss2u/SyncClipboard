namespace SyncClipboard.Core.Exceptions;

public class ProfileDataDownloadException : RemoteServerException
{
    public ProfileDataDownloadException(string message) : base(message)
    {
    }

    public ProfileDataDownloadException(string message, Exception? innerException) : base(message, innerException)
    {
    }
}
