using System;
using System.Collections.Generic;
using System.Web;
using EPiServer.ServiceLocation;
using Newtonsoft.Json;
using PixieEpiServerExtensionCoViewing.Models;

namespace PixieEpiServerExtensionCoViewing.Repository
{
    [ServiceConfiguration(typeof(ISignalRConnectionsRepository), Lifecycle = ServiceInstanceScope.Singleton)]
    public class SignalRConnectionsRepository : ISignalRConnectionsRepository
    {
        private const string CookieName = "signalRConnection";
        private HttpContext CurrentContext => HttpContext.Current;

        public void Save(SignalRConnection connection)
        {
            var data = JsonConvert.SerializeObject(connection);
            HttpCookie cookie = CurrentContext.Request.Cookies.Get(CookieName);
            if (cookie != null)
            {
                cookie.Value = data;
            }
            else
            {
                cookie = new HttpCookie(CookieName, data);
            }

            cookie.Expires = DateTime.Now.AddHours(1);
            CurrentContext.Response.Cookies.Add(cookie);

        }

        public SignalRConnection Get()
        {
            var cookie = CurrentContext.Request.Cookies.Get(CookieName);

            if (cookie?.Value == null) return null;
            return JsonConvert.DeserializeObject<SignalRConnection>(cookie.Value);
        }

        public void Delete()
        {
            var cookie = CurrentContext.Request.Cookies.Get(CookieName);
            if (cookie == null) return;
            cookie.Expires = DateTime.Now.AddDays(-1);
            CurrentContext.Response.Cookies.Add(cookie);
        }
    }
}
