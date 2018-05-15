# SignalrCoViewing
Signalr Coviewing Research for EPiServer Quicksilver

install 
Microsoft.AspNet.SignalR
Microsoft.AspNet.SignalR.Client
Microsoft.AspNet.SignalR.ServiceBus

Add app.SignalRStartUp(); into Startup Class

Copy Scripts/js/SignalrJs into QuickSlver Project

Open QuickSilver.js and intialize JS library by adding line SignalR.init(); after Recommendations.init();

We need to add references for Scripts/jquery.signalR-{version}.js, I bundled this 
bundles.Add(new ScriptBundle("~/bundles/jquerysignalr").Include("~/Scripts/jquery.signalR-{version}.js"));
and added @Scripts.Render("~/bundles/jquerysignalr") into _Layout.cshtml

This should be added after JQuery Files, and remove old JQuery files

Project is ready to stat working with.

To start a chat session, Presenter have to register, Our Code will generate a url that presenter will email/pass this to friend, On accessing that URL by friend , Co viewin sesssion will start.

Tom Implement this flow, I added a simple form,

Views/Start/Index.cshtml
and added following HTML, so form can be posted
<div class="row">
        <div class="col-xs-12">
            @using (Html.BeginForm("Index", null, new { node = Model.StartPage.ContentLink }, FormMethod.Post))
            {
                <div class="form-group">
                    <input type="text" placeholder="Invite your friend's by providing us his/her email address to share your experience" class="form-control form-inline" id="email" name="email" />
                    <input type="submit" value="Invite"/>
                </div>
            }
        </div>
    </div>


and updated the start controller

[CommerceTracking(TrackingType.Home)]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ViewResult Index(StartPage currentPage, string email = "")
        {
            var viewModel = new StartPageViewModel()
            {
                StartPage = currentPage,
                Recommendations = this.GetHomeRecommendations().Take(6),
                Promotions = GetActivePromotions()
            };

            if (!string.IsNullOrEmpty(email))
                InviteFreinds(email);

            return View(viewModel);
        }

        private void InviteFreinds(string email)
        {
            var mailService = ServiceLocator.Current.GetInstance<IMailService>();
            var signalRManager = ServiceLocator.Current.GetInstance<IPixieCoViewingManager>();
            var link = signalRManager.StartPresenterSession();
            mailService.Send("Join me", link, email);

        }