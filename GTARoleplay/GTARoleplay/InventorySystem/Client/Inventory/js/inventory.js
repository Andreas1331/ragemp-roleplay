var inventoryWindow = null;
var itms = null;
var maxWeight = null;

mp.events.add('ShowInventory::Client', (maxWeightTmp, itmsTmp) => {
    if (!mp.objects.exists(inventoryWindow)) {
        inventoryWindow = mp.browsers.new("package://gtaroleplay/Inventory/index.html");
        itms = itmsTmp;
        maxWeight = maxWeightTmp;
        mp.gui.chat.activate(false);
    }
});

mp.events.add('RefreshInventory::Client', (maxWeightTmp, itmsTmp) => {
    if (mp.objects.exists(inventoryWindow)) {
        itms = itmsTmp;
        maxWeight = maxWeightTmp;
        inventoryWindow.execute("clearItemsContainer();");
        inventoryWindow.execute(`addItemsToContainer(${maxWeight}, '${itms}');`);
        mp.gui.cursor.show(true, true);
    }
});


mp.events.add('browserDomReady', (browser) => {
    if(browser !== inventoryWindow)
        return;
    inventoryWindow.execute(`addItemsToContainer(${maxWeight}, '${itms}');`);
    mp.gui.cursor.show(true, true);
});

mp.events.add('DestroyInventory::Client', () => {
    if (mp.objects.exists(inventoryWindow)) {
        inventoryWindow.destroy();
        mp.gui.chat.activate(true);
        mp.gui.cursor.show(false,false);
    }
});

mp.events.add('UseItemInInventory::Client', (itmIndentifier) => {
    mp.events.callRemote('UseItem::Server', itmIndentifier);
    // Disable the cursor until we get a response from the server
    mp.gui.cursor.show(false,false);
});
