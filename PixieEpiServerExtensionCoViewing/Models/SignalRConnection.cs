namespace PixieEpiServerExtensionCoViewing.Models
{
    public class SignalRConnection
    {
        public string GroupName { get; set; }
        public string ConnectionId { get; set; }
        public SignalRConnectionType ConnectionType { get; set; }
    }

    public enum SignalRConnectionType
    {
        Presenter = 1,
        Audience = 2
    }
}
