var loginWindow = null;

mp.events.add('ShowLogin::Client', () => {
    if (!mp.browsers.exists(loginWindow)) {
        loginWindow = mp.browsers.new("package://gtaroleplay/Login/index.html");
        mp.gui.chat.activate(false);
    }
});

mp.events.add('DestroyLogin::Client', () => {
    if (mp.browsers.exists(loginWindow)) {
        loginWindow.destroy();
    }
});

mp.events.add('browserDomReady', (browser) => {
    if(browser !== loginWindow)
        return;

    mp.gui.cursor.show(true, true);
});

mp.events.add('OnLoginSubmitted::Client', (username, password) => {
	mp.events.callRemote('OnLoginSubmitted::Server', username, password);
});
