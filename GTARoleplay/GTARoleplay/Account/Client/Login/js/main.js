function OnClickSubmit(){
    var user = $("#username").val();
    var pass = $("#password").val();

    if(!user || !pass)
        return;

    mp.trigger("OnLoginSubmitted::Client", user, pass);
}

$(document).ready(function () {
    $("body").keydown(function(event)
    {
        if (event.which == 13) {
            OnClickSubmit();
        }
    });
});
