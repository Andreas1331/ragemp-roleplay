const torsoDataMale = require("gtaroleplay/ClothesSelector/resources/besttorso_male.js");
const library = require("gtaroleplay/library.js");
var moddingWindow = null;

mp.events.add('ShowClothingSelector::Client', () => {
    if (!mp.browsers.exists(moddingWindow)) {
        moddingWindow = mp.browsers.new("package://gtaroleplay/ClothesSelector/index.html");
        mp.gui.chat.activate(false);
        mp.players.local.freezePosition(true);
        setupCamera();
    }
});

function setupCamera(){
    // Get the position infront of the vehicle and place the player there
    const plyRot = mp.players.local.getHeading();
    const plyPos = mp.players.local.position;
    const coordsInfront = library.getCoordsInfront(plyPos, plyRot, 2);

    let camObj = mp.cameras.new('default', new mp.Vector3(coordsInfront.x, coordsInfront.y, coordsInfront.z), new mp.Vector3(0, 0, 0), 60);
    camObj.setActive(true);
    camObj.pointAtCoord(plyPos.x, plyPos.y, plyPos.z);
    mp.players.local.taskTurnToFaceCoord(coordsInfront.x, coordsInfront.y, coordsInfront.z, 0);
    mp.game.cam.renderScriptCams(true, true, 2, true, false);
}

mp.events.add('browserDomReady', (browser) => {
    if(browser !== moddingWindow)
        return;

        let player = mp.players.local;
        let currentClothes = [
            {
                id: "hatsSlider",
                componentSlot: 0,
                isAccessory: true,
                startValue: player.getPropIndex(0),
                min: 0,
                max: player.getNumberOfPropDrawableVariations(0),
                initialMaxTexture: player.getNumberOfPropTextureVariations(0, player.getPropIndex(0)),
                currentTexture: player.getPropTextureIndex(0)
            },
            {
                id: "topsSlider",
                componentSlot: 11,
                isAccessory: false,
                startValue: player.getDrawableVariation(11),
                min: 0,
                max: player.getNumberOfDrawableVariations(11),
                initialMaxTexture: player.getNumberOfTextureVariations(11, player.getDrawableVariation(11)),
                currentTexture: player.getTextureVariation(11)
            },
            {
                id: "torsoSlider",
                componentSlot: 3,
                isAccessory: false,
                startValue: player.getDrawableVariation(3),
                min: 0,
                max: player.getNumberOfDrawableVariations(3),
                initialMaxTexture: player.getNumberOfTextureVariations(3, player.getDrawableVariation(3)),
                currentTexture: player.getTextureVariation(3)
            },
            {
                id: "undershirtSlider",
                componentSlot: 8,
                isAccessory: false,
                startValue: player.getDrawableVariation(8),
                min: 0,
                max: player.getNumberOfDrawableVariations(8),
                initialMaxTexture: player.getNumberOfTextureVariations(8, player.getDrawableVariation(8)),
                currentTexture: player.getTextureVariation(8)
            },
            {
                id: "legsSlider",
                componentSlot: 4,
                isAccessory: false,
                startValue: player.getDrawableVariation(4),
                min: 0,
                max: player.getNumberOfDrawableVariations(4),
                initialMaxTexture: player.getNumberOfTextureVariations(4, player.getDrawableVariation(4)),
                currentTexture: player.getTextureVariation(4)
            },
            {
                id: "shoesSlider",
                componentSlot: 6,
                isAccessory: false,
                startValue: player.getDrawableVariation(6),
                min: 0,
                max: player.getNumberOfDrawableVariations(6),
                initialMaxTexture: player.getNumberOfTextureVariations(6, player.getDrawableVariation(6)),
                currentTexture: player.getTextureVariation(6)
            },
            {
                id: "glassesSlider",
                componentSlot: 1,
                isAccessory: true,
                startValue: player.getPropIndex(1),
                min: 0,
                max: player.getNumberOfPropDrawableVariations(1),
                initialMaxTexture: player.getNumberOfPropTextureVariations(1, player.getPropIndex(1)),
                currentTexture: player.getPropTextureIndex(1)
            },
            {
                id: "earsSlider",
                componentSlot: 2,
                isAccessory: true,
                startValue: player.getPropIndex(2),
                min: 0,
                max: player.getNumberOfPropDrawableVariations(2),
                initialMaxTexture: player.getNumberOfPropTextureVariations(2, player.getPropIndex(2)),
                currentTexture: player.getPropTextureIndex(2)
            }
        ];

    moddingWindow.execute(`setupSliders('${JSON.stringify(currentClothes)}');`);
    mp.gui.cursor.show(true, true);
});

mp.events.add('DestroyClothingWindow::Client', () => {
    if (mp.browsers.exists(moddingWindow)) {
        moddingWindow.destroy();
        mp.gui.chat.activate(true);
        mp.gui.cursor.show(false,false);
        mp.game.cam.renderScriptCams(false, true, 0, true, false);
        mp.players.local.freezePosition(false);
        // Tell the server, and have the player re-synced 
        mp.events.callRemote('RequestResyncOfPlayerClothes::Server');
    }
});

mp.events.add('SetPlayerClothes::Client', (isAccessory, componentSlot, drawable, texture) => {
    if(isAccessory === true){
        mp.players.local.setPropIndex(parseInt(componentSlot), parseInt(drawable), parseInt(texture), true);
    } else {
        // Attempt to get the correct torso from the library, if the player is changing top
        if (componentSlot === 11 && torsoDataMale[drawable] !== undefined && torsoDataMale[drawable][texture] !== undefined) {
            let betterTorso = torsoDataMale[drawable][texture];
            if (betterTorso.BestTorsoDrawable != -1) {
                moddingWindow.execute(`setTorsoElements(${betterTorso.BestTorsoDrawable}, ${betterTorso.BestTorsoTexture});`);
                mp.players.local.setComponentVariation(3, betterTorso.BestTorsoDrawable, betterTorso.BestTorsoTexture, 2);
            }
        }

        mp.players.local.setComponentVariation(parseInt(componentSlot), parseInt(drawable), parseInt(texture), 2);
    }
});

mp.events.add('GetDrawableMaxTexture::Client', (id, isAccessory, componentSlot, drawable) => {
    if(isAccessory === true){
        let maxTextures = mp.players.local.getNumberOfPropTextureVariations(parseInt(componentSlot), parseInt(drawable));
        moddingWindow.execute(`updateMaxTextures('${id}', '${componentSlot}', '${maxTextures}', 0);`);
    } else {
        let maxTextures = mp.players.local.getNumberOfTextureVariations(parseInt(componentSlot), parseInt(drawable));
        moddingWindow.execute(`updateMaxTextures('${id}', '${componentSlot}', '${maxTextures}', 0);`);
    }

});

mp.events.add('SetPlayerRotation::Client', (rotation) => {
    mp.players.local.setRotation(0.0, 0.0, parseFloat(rotation), 1, true);
});

mp.events.add('PurchaseOutfit::Client', (outfit) => {
    if(!outfit)
        return;
    mp.events.callRemote('PurchaseOutfit::Server', outfit);
});

mp.events.add('render', () => {
    if (mp.browsers.exists(moddingWindow)) {
        mp.players.local.clearTasksImmediately();
    }
});