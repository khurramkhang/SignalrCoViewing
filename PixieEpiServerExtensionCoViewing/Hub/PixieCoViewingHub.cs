using System;
using Microsoft.AspNet.SignalR.Hubs;
using PixieEpiServerExtensionCoViewing.Models;
using PixieEpiServerExtensionCoViewing.Repository;
using System.Threading.Tasks;
using System.Web;
using EPiServer.ServiceLocation;

namespace PixieEpiServerExtensionCoViewing.Hub
{
    [HubName("pixieCoViewingHub")]
    public class PixieCoViewingHub : Microsoft.AspNet.SignalR.Hub
    {
        private readonly ISignalRConnectionsRepository _signalRConnectionsRepository;

        public PixieCoViewingHub()
        {
            _signalRConnectionsRepository = ServiceLocator.Current.GetInstance<ISignalRConnectionsRepository>();
        }

        public PixieCoViewingHub(ISignalRConnectionsRepository signalRConnectionsRepository)
        {
            _signalRConnectionsRepository = signalRConnectionsRepository;
        }

        public async Task<SignalRConnection> SignInAsPresenter(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
                groupName = Guid.NewGuid().ToString();
            return await SignIn(SignalRConnectionType.Presenter, groupName);
        }

        public async Task<SignalRConnection> SignIn(SignalRConnectionType connectionType)
        {
            var groupName = Guid.NewGuid().ToString();
            return await SignIn(connectionType, groupName);
        }

        public async Task<SignalRConnection> SignIn(SignalRConnectionType connectionType, string groupName)
        {
            var connection = new SignalRConnection() { ConnectionId = Context.ConnectionId, ConnectionType = connectionType, GroupName = groupName };
            await Groups.Add(Context.ConnectionId, groupName);
            return connection;
        }

        public async Task<bool> SignOut(string groupName)
        {
            await Groups.Remove(Context.ConnectionId, groupName);
            return true;
        }

        public async Task ScrollTo(string hash)
        {
            var connection = _signalRConnectionsRepository.Get();
            if (connection?.ConnectionType == SignalRConnectionType.Presenter) Clients.OthersInGroup(connection.GroupName).onScroll(hash);
        }
        public async Task RedirectTo(string url)
        {
            var connection = _signalRConnectionsRepository.Get();
            if (connection?.ConnectionType == SignalRConnectionType.Presenter) Clients.OthersInGroup(connection.GroupName).onRedirect(url);
        }

        public async Task Reconnect()
        {
            var connection = _signalRConnectionsRepository.Get();
            if (!string.IsNullOrEmpty(connection?.GroupName)) await Groups.Add(Context.ConnectionId, connection.GroupName);
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            if (stopCalled)
            {
                Console.WriteLine(String.Format("Client {0} explicitly closed the connection.", Context.ConnectionId));
            }
            else
            {
                Console.WriteLine(String.Format("Client {0} timed out .", Context.ConnectionId));
            }

            return base.OnDisconnected(stopCalled);
        }
    }
}
