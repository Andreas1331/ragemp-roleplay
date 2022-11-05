var charSelectorWindow = null;
var charactersLst = null;

mp.events.add('ShowCharSelector::Client', (characters) => {
    if (!mp.objects.exists(charSelectorWindow)) {
        charSelectorWindow = mp.browsers.new("package://gtaroleplay/CharSelector/index.html");
        charactersLst = JSON.parse(characters);
        mp.gui.chat.activate(false);
    }
});

mp.events.add('DestroyCharSelector::Client', () => {
    if (mp.objects.exists(charSelectorWindow)) {
        charSelectorWindow.destroy();
        mp.gui.chat.activate(true);
        mp.gui.cursor.show(false,false);
    }
});

mp.events.add('browserDomReady', (browser) => {
    if(browser !== charSelectorWindow)
        return;

    mp.gui.cursor.show(true, true);
    if(charactersLst)
        charSelectorWindow.execute(`setupCharacterDropdown('${JSON.stringify(charactersLst)}');`);
});

mp.events.add('OnCharacterSelected::Client', (character) => {
	mp.events.callRemote('OnCharacterSelected::Server', character);
});
