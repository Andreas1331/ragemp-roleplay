let wheelWindow = null;
let wheelDataHolder = null;
let listenToKey = null;

mp.events.add('ShowInteractionWheel::Client', (wheelData, tmpListenToKey) => {
    if (!mp.objects.exists(wheelWindow)) {
        mp.game.graphics.transitionToBlurred(250);
        wheelDataHolder = wheelData;
        listenToKey = tmpListenToKey;
        createWheelBrowser();
    }
});

function createWheelBrowser(){
    wheelWindow = mp.browsers.new("package://gtaroleplay/GenericWheel/index.html");
    mp.gui.chat.activate(false);
}

mp.events.add('DestroyWheel::Client', () => {
    mp.game.graphics.transitionFromBlurred(0);
    destroyWheelBrowser();
});

mp.events.add('OnWheelSliceClicked::Client', (wheelMenuID, wheelSliceIndex) => {
	mp.events.callRemote('OnWheelSliceClicked::Server', wheelMenuID, wheelSliceIndex);
});

mp.events.add('browserDomReady', (browser) => {
    if(browser !== wheelWindow)
        return;

    wheelWindow.execute(`initWheel('${wheelDataHolder}', '${listenToKey}');`);
    mp.gui.cursor.show(true, true);
});

function destroyWheelBrowser(){
    if (mp.objects.exists(wheelWindow)) {
        wheelWindow.destroy();
        mp.gui.cursor.show(false, false);
        mp.gui.chat.activate(true);
    }
}


