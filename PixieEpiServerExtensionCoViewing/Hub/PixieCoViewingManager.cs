using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using EPiServer.ServiceLocation;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Hubs;
using PixieEpiServerExtensionCoViewing.Models;
using PixieEpiServerExtensionCoViewing.Repository;

namespace PixieEpiServerExtensionCoViewing.Hub
{
    [ServiceConfiguration(typeof(IPixieCoViewingManager), Lifecycle = ServiceInstanceScope.Singleton)]
    public class PixieCoViewingManager : IPixieCoViewingManager
    {
        private const string hubName = "pixieCoViewingHub";
        private readonly ISignalRConnectionsRepository _signalRConnectionsRepository;

        public PixieCoViewingManager(ISignalRConnectionsRepository signalRConnectionsRepository)
        {
            _signalRConnectionsRepository = signalRConnectionsRepository;
        }

        private string SignInPath => "/joinme/signin?group={0}&returnurl={1}";
        private string _baseUri = string.Empty;
        private string BaseUri
        {
            get
            {
                if (!string.IsNullOrEmpty(_baseUri)) return _baseUri;
                var request = HttpContext.Current.Request;
                _baseUri = request.Url.GetLeftPart(UriPartial.Authority);

                return _baseUri;
            }
        }
        public IHubContext HubContext => GlobalHost.ConnectionManager.GetHubContext<PixieCoViewingHub>();

        public string StartPresenterSession()
        {

            var hubConnection = new HubConnection(BaseUri) { CookieContainer = new CookieContainer() };
            IHubProxy stockTickerHubProxy = hubConnection.CreateHubProxy(hubName);
            Task.WaitAll(hubConnection.Start());
            var cookie = _signalRConnectionsRepository.Get();
            var connection = stockTickerHubProxy.Invoke<SignalRConnection>("SignInAsPresenter", cookie?.GroupName).GetAwaiter().GetResult();
            if (connection != null)
            {
                _signalRConnectionsRepository.Save(connection);
                return $"{BaseUri}{string.Format(SignInPath, connection.GroupName, BaseUri)}";
            }

            return string.Empty;
        }

        public void SignInAudience(string groupName)
        {
            var hubConnection = new HubConnection(BaseUri) { CookieContainer = new CookieContainer() };
            IHubProxy stockTickerHubProxy = hubConnection.CreateHubProxy(hubName);
            Task.WaitAll(hubConnection.Start());
            var connection = stockTickerHubProxy.Invoke<SignalRConnection>("SignIn", SignalRConnectionType.Audience, groupName).GetAwaiter().GetResult();
            if (connection != null)
            {
                _signalRConnectionsRepository.Save(connection);
            }
        }

        public void SignOut(string groupName)
        {
            var hubConnection = new HubConnection(BaseUri) { CookieContainer = new CookieContainer() };
            IHubProxy stockTickerHubProxy = hubConnection.CreateHubProxy(hubName);
            Task.WaitAll(hubConnection.Start());
            stockTickerHubProxy.Invoke<SignalRConnection>("SignOut", groupName).GetAwaiter().GetResult();
            _signalRConnectionsRepository.Delete();
        }
    }
}
