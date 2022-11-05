var drawHUD = false;
var resX, resY = 0;

// variables to render, usually received by the server
let distanceTravelled = 0;
let fuelLeft = 0;
let cashOnHand = 0;
let cashInBank = 0;

const updateInterval = 500; // milliseconds, lower value = more accurate, at the cost of performance
const Natives = {
    IS_RADAR_HIDDEN: "0x157F93B036700462",
    IS_RADAR_ENABLED: "0xAF754F20EB5CD51A",
    SET_TEXT_OUTLINE: "0x2513DFB0FB8400FE"
};
let isMetric = false;
let streetName = null;
let zoneName = null;
let minimap = {};

mp.events.add('EnableHUD::Client', (tmpDrawHUD) => {
    drawHUD = tmpDrawHUD;
    
    mp.game.ui.displayAreaName(tmpDrawHUD);
    mp.game.ui.displayRadar(tmpDrawHUD);
    mp.game.ui.displayHud(tmpDrawHUD);

    setScreenRes();
});

mp.events.add('render', () => {
    if(!global.loggedIn || !drawHUD)
        return;

    drawGamemodeText();
    drawLocationText();
    drawVehicleText();
    drawMoneyText();
});

function drawMoneyText(){
    drawText(`~g~${cashOnHand}`, [0.98, 0.08], 4, [255, 255, 255, 255], 0.70, true);
}

function drawVehicleText(){
    let vehicle = mp.players.local.vehicle;
    if (vehicle) {
        drawText(`${(vehicle.getSpeed() * (isMetric ? 3.6 : 2.236936)).toFixed(0)} ${(isMetric) ? "KM/H" : "MPH"}`, [minimap.rightX - 0.003, minimap.bottomY - 0.0485], 4, [255, 255, 255, 255], 0.45, true);
        drawText(`${distanceTravelled.toFixed(2)} km`, [minimap.rightX - 0.003, minimap.bottomY - 0.0785], 4, [255, 255, 255, 255], 0.45, true);
        drawText(`${fuelLeft.toFixed(2)}% fuel`, [minimap.rightX - 0.003, minimap.bottomY - 0.1085], 4, [255, 255, 255, 255], 0.45, true);
    }
}

function drawLocationText(){
    if (streetName && zoneName) {
        drawText(streetName, [minimap.rightX + 0.01, minimap.bottomY - 0.065], 4, [255, 255, 255, 255], 0.55);
        drawText(zoneName, [minimap.rightX + 0.01, minimap.bottomY - 0.035], 4, [255, 255, 255, 255], 0.5);
     }
}

function drawGamemodeText(){
    mp.game.graphics.drawText(
    "GTA ~b~Roleplay ~s~- " + global.version, 
    [(resX / 2) / resX, 0.97], 
    {
      font: 4,
      color: [255, 255, 255, 255],
      scale: [0.5, 0.5],
      outline: true
    });
}

function setScreenRes(){
    let resolution = mp.game.graphics.getScreenActiveResolution(0,0);
	resX = resolution.x;
	resY = resolution.y;
}


// https://github.com/glitchdetector/fivem-minimap-anchor
function getMinimapAnchor() {
    let sfX = 1.0 / 20.0;
    let sfY = 1.0 / 20.0;
    let safeZone = mp.game.graphics.getSafeZoneSize();
    let aspectRatio = mp.game.graphics.getScreenAspectRatio(false);
    let resolution = mp.game.graphics.getScreenActiveResolution(0, 0);
    let scaleX = 1.0 / resolution.x;
    let scaleY = 1.0 / resolution.y;

    let minimap = {
        width: scaleX * (resolution.x / (4 * aspectRatio)),
        height: scaleY * (resolution.y / 5.674),
        scaleX: scaleX,
        scaleY: scaleY,
        leftX: scaleX * (resolution.x * (sfX * (Math.abs(safeZone - 1.0) * 10))),
        bottomY: 1.0 - scaleY * (resolution.y * (sfY * (Math.abs(safeZone - 1.0) * 10))),
    };

    minimap.rightX = minimap.leftX + minimap.width;
    minimap.topY = minimap.bottomY - minimap.height;
    return minimap;
}

function drawText(text, drawXY, font, color, scale, alignRight = false) {
    mp.game.ui.setTextEntry("STRING");
    mp.game.ui.addTextComponentSubstringPlayerName(text);
    mp.game.ui.setTextFont(font);
    mp.game.ui.setTextScale(scale, scale);
    mp.game.ui.setTextColour(color[0], color[1], color[2], color[3]);
    mp.game.invoke(Natives.SET_TEXT_OUTLINE);

    if (alignRight) {
        mp.game.ui.setTextRightJustify(true);
        mp.game.ui.setTextWrap(0, drawXY[0]);
    }

    mp.game.ui.drawText(drawXY[0], drawXY[1]);
}

setInterval(() => {
    // Disable the idle camera 
    mp.game.invoke('0x9E4CFFF989258472');
    mp.game.invoke('0xF4F2C0D4EE209E20');
    
    // Only do stuff if radar is enabled and visible
    if (mp.game.invoke(Natives.IS_RADAR_ENABLED) && !mp.game.invoke(Natives.IS_RADAR_HIDDEN)) {
        isMetric = mp.game.gameplay.getProfileSetting(227) == 1;
        minimap = getMinimapAnchor();

        const position = mp.players.local.position;
        let getStreet = mp.game.pathfind.getStreetNameAtCoord(position.x, position.y, position.z, 0, 0);
        zoneName = mp.game.ui.getLabelText(mp.game.zone.getNameOfZone(position.x, position.y, position.z));
        streetName = mp.game.ui.getStreetNameFromHashKey(getStreet.streetName);
        if (getStreet.crossingRoad && getStreet.crossingRoad != getStreet.streetName) streetName += ` / ${mp.game.ui.getStreetNameFromHashKey(getStreet.crossingRoad)}`;
    } else {
        streetName = null;
        zoneName = null;
    }
}, updateInterval);

mp.events.add('UpdateVehicleStats::Client', (distance, fuel) => {
    distanceTravelled = distance;
    fuelLeft = fuel;
});

mp.events.add('UpdateCashStats::Client', (_cashOnHand, _cashInBank) => {
    cashOnHand = _cashOnHand;
    cashInBank = _cashInBank;
});