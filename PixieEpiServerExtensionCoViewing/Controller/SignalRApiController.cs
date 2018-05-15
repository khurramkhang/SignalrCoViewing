using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Hubs;
using PixieEpiServerExtensionCoViewing.Hub;
using PixieEpiServerExtensionCoViewing.Models;
using PixieEpiServerExtensionCoViewing.Repository;

namespace PixieEpiServerExtensionCoViewing.Controller
{
    [RoutePrefix("joinme")]
    public class SignalRApiController : ApiController
    {
        private readonly IPixieCoViewingManager _coViewingManager;

        public SignalRApiController(IPixieCoViewingManager coViewingManager)
        {
            _coViewingManager = coViewingManager;
        }

        [Route("signin")]
        [AcceptVerbs("GET")]
        public IHttpActionResult SignIn(string group, string returnurl)
        {

            _coViewingManager.SignInAudience(group);
            return Redirect(returnurl);
        }

        [Route("signout")]
        [AcceptVerbs("GET")]
        public IHttpActionResult SignOut(string group, string returnurl)
        {
            _coViewingManager.SignOut(group);
            return Redirect(returnurl);
        }
    }
}
