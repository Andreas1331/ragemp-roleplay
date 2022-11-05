const library = require("gtaroleplay/library.js");

var moddingWindow = null;
var vehicleToMod = null;

var availableModsData = {};

mp.events.add('ShowVehicleModding::Client', (vehID) => {
    if (!mp.objects.exists(moddingWindow)) {
        moddingWindow = mp.browsers.new("package://gtaroleplay/VehicleModding/index.html");
        mp.gui.chat.activate(false);
        vehicleToMod = mp.vehicles.atRemoteId(vehID);

        availableModsData['primaryColor'] = vehicleToMod.getCustomPrimaryColour(0,0,0);
        availableModsData['secondaryColor'] = vehicleToMod.getCustomSecondaryColour(0,0,0);
        availableModsData['spoilers'] = vehicleToMod.getNumMods(0);
        availableModsData['frontBumpers'] = vehicleToMod.getNumMods(1);
        availableModsData['rearBumpers'] = vehicleToMod.getNumMods(2);
        availableModsData['skirts'] = vehicleToMod.getNumMods(3);
        availableModsData['exhauts'] = vehicleToMod.getNumMods(4);
        availableModsData['frames'] = vehicleToMod.getNumMods(5);
        availableModsData['grilles'] = vehicleToMod.getNumMods(6);
        availableModsData['hoods'] = vehicleToMod.getNumMods(7);
        availableModsData['leftFenders'] = vehicleToMod.getNumMods(8);
        availableModsData['rightFenders'] = vehicleToMod.getNumMods(9);
        availableModsData['roofs'] = vehicleToMod.getNumMods(10);
        availableModsData['armors'] = vehicleToMod.getNumMods(16);

        availableModsData['steeringWheels'] = vehicleToMod.getNumMods(33);
        availableModsData['hydraulics'] = vehicleToMod.getNumMods(38);

        setPlayerPosition();
    }
});

function setPlayerPosition(){
    // Get the position infront of the vehicle and place the player there
    const vehRot = vehicleToMod.getHeading();
    const vehiclePos = vehicleToMod.position;
    const coordsInfront = library.getCoordsInfront(vehiclePos, vehRot, 5);

    let camObj = mp.cameras.new('default', new mp.Vector3(coordsInfront.x, coordsInfront.y, coordsInfront.z + 2), new mp.Vector3(0, 0, 0), 60);
    camObj.setActive(true);
    camObj.pointAtCoord(vehiclePos.x, vehiclePos.y, vehiclePos.z);
    mp.game.cam.renderScriptCams(true, true, 2, true, false);

    mp.players.local.position = coordsInfront;
    mp.players.local.setAlpha(0);
}

mp.events.add('browserDomReady', (browser) => {
    if(browser !== moddingWindow)
        return;
    moddingWindow.execute(`setupSubMenus('${JSON.stringify(availableModsData)}');`);
    mp.gui.cursor.show(true, true);
});

mp.events.add('DestroyVehicleModding::Client', () => {
    if (mp.objects.exists(moddingWindow)) {
        moddingWindow.destroy();
        mp.gui.chat.activate(true);
        mp.gui.cursor.show(false,false);
        mp.players.local.setAlpha(255);
        mp.game.cam.renderScriptCams(false, true, 0, true, false);
        // Tell the server, and have the vehicle re-synced with the client 
        // TODO: Should probably set the players position back?
        mp.events.callRemote('RequestResyncOfVehicle::Server', vehicleToMod);
    }
});

mp.events.add('SetVehicleRotation::Client', (rotation) => {
    if(vehicleToMod){
        vehicleToMod.setRotation(0.0, 0.0, parseFloat(rotation), 1, true);
    }
});

mp.events.add('SetVehiclePrimaryColor::Client', (r, g, b) => {
    if(vehicleToMod){
        vehicleToMod.setCustomPrimaryColour(parseInt(r), parseInt(g), parseInt(b));
    }
});

mp.events.add('SetVehicleSecondaryColor::Client', (r, g, b) => {
    if(vehicleToMod){
        vehicleToMod.setCustomSecondaryColour(parseInt(r), parseInt(g), parseInt(b));
    }
});

mp.events.add('SetVehicleWheels::Client', (modType, wheelType, wheel) => {
    if(vehicleToMod){
        vehicleToMod.setWheelType(parseInt(wheelType));
        vehicleToMod.setMod(parseInt(modType), parseInt(wheel));
    }
});

// Shared by many menus
mp.events.add('SetVehicleMod::Client', (modType, modValue) => {
    if(vehicleToMod){
        vehicleToMod.setMod(parseInt(modType), parseInt(modValue));
    }
});

mp.events.add('SetVehicleWindowTint::Client', (modValue) => {
    if(vehicleToMod){
        vehicleToMod.setWindowTint(modValue);
    }
});

mp.events.add('PurchasePrimaryVehicleColor::Client', (r, g, b) => {
    mp.events.callRemote('PurchasePrimaryVehicleColor::Server', vehicleToMod, parseInt(r), parseInt(g), parseInt(b));
});

mp.events.add('PurchaseSecondaryVehicleColor::Client', (r, g, b) => {
    mp.events.callRemote('PurchaseSecondaryVehicleColor::Server', vehicleToMod, parseInt(r), parseInt(g), parseInt(b));
});

mp.events.add('PurchaseVehicleMod::Client', (modType, modValue) => {
    mp.events.callRemote('PurchaseVehicleMod::Server', vehicleToMod, parseInt(modType), parseInt(modValue));
});

mp.events.add('PurchaseVehicleWheel::Client', (modType, wheelType, wheelValue) => {
    mp.events.callRemote('PurchaseVehicleWheel::Server', vehicleToMod,  parseInt(modType), parseInt(wheelType), parseInt(wheelValue));
});