var SignalR = {
    init: function () {
        var con = $.hubConnection();
        var hub = con.createHubProxy('pixieCoViewingHub');
        hub.on('onHitRecorded', function (i) {
            $('#hitCount').text(i);
        });
        hub.on('onScroll', function (i) {
            $(window).scrollTop(i);
        });
        hub.on('onRedirect', function (url) {
            setTimeout(function () { window.location = url; }, 2000);
        });
        con.start(function () {
            hub.invoke("Reconnect").fail(function (e) {
                console.log("Can't reconnect.");
                console.log(e);
            });
            $(window).scroll(function (event) {
                var scroll = $(window).scrollTop();
                hub.invoke("ScrollTo", scroll).fail(function (e) {
                    console.log("Failed on scrolling");
                    console.log(e);
                });
            });
            $("a").click(function (event) {
                var addressValue = $(this).attr("href");
                hub.invoke("RedirectTo", addressValue).fail(function (e) {
                    console.log("Failed while redirection");
                    console.log(e);
                });
            });
        });
    }
};