let wheelWindow = null;
let wheelDataHolder = null;
let listenToKey = null;

mp.events.add('ShowInteractionWheel::Client', (wheelData, tmpListenToKey) => {
    if (!mp.browsers.exists(wheelWindow)) {
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
    destroyWheelBrowser();
});

mp.events.add('OnWheelSliceClicked::Client', (wheelMenuID, wheelSliceIndex) => {
	mp.events.callRemote('OnWheelSliceClicked::Server', wheelMenuID, wheelSliceIndex);
});

mp.events.add('browserDomReady', (browser) => {
    if(browser !== wheelWindow)
        return;

    if (mp.keys.isDown(listenToKey) === true) {
        wheelWindow.execute(`initWheel('${wheelDataHolder}', ${listenToKey});`);
        mp.game.graphics.transitionToBlurred(250);
        mp.gui.cursor.show(true, true);
        mp.game.invoke("0xFC695459D4D0E219", 0.5, 0.5); // Center the cursor
    } else {
        destroyWheelBrowser();
	}
});

function destroyWheelBrowser(){
    mp.game.graphics.transitionFromBlurred(0);

    if (mp.browsers.exists(wheelWindow)) {
        wheelWindow.destroy();
        mp.gui.cursor.show(false, false);
        mp.gui.chat.activate(true);
    }
}


