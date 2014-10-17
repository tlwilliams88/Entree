using KeithLink.Svc.Core.Events.EventArgs;

namespace KeithLink.Svc.Core.Events.EventHandlers
{
    public delegate void FileReceivedHandler(object sender, ReceivedFileEventArgs e);
    public delegate void ExceptionEventHandler(object sender, ExceptionEventArgs e);
}
