using System.Configuration;
using Microsoft.AspNet.SignalR;
using Owin;

namespace PixieEpiServerExtensionCoViewing.ApplicationBuilderExtensions
{
    public static class SignalRExtensions
    {
        public static void SignalRStartUp(this IAppBuilder app)
        {
            bool.TryParse(ConfigurationManager.AppSettings["EnableCoViewing"], out bool enableSignalR);

            if (!enableSignalR) return;

            if (ConfigurationManager.ConnectionStrings["SignalRScaleoutServiceBus"] != null)
            {
                var serviceBusConnectionString = ConfigurationManager.ConnectionStrings["SignalRScaleoutServiceBus"].ConnectionString;

                if (!string.IsNullOrWhiteSpace(serviceBusConnectionString))
                {
                    GlobalHost.DependencyResolver.UseServiceBus(new ServiceBusScaleoutConfiguration(serviceBusConnectionString, "PixieEpiServerExtensionCoViewing"));
                }
            }

            //ToDo:Presence monitoring

            app.MapSignalR();
            GlobalHost.Configuration.MaxIncomingWebSocketMessageSize = null;
        }
    }
}
