let specCam = null;

/* Camera settings */
const mouseSensitivity = 6.5;
const zoomSpeed = 3.5;
const minZoom = 5.0;
const maxZoom = 60.0;

mp.events.add('StartSpectatingTarget::Client', (target) => {
    mp.players.local.attachTo(target.handle, 0, 0, 0, 10, 0, 0, 0, true, false, false, false, 0, false);
    setupSpectatingCamera(target);
});

mp.events.add('StopSpectatingTarget::Client', () => {
    mp.players.local.detach(true, false);
    mp.game.cam.renderScriptCams(false, false, 0, true, false);
    specCam.destroy();
    specCam = null;
});

function setupSpectatingCamera(target){
    // Create a new camera
	specCam = mp.cameras.new('default', target.position, 
                                        new mp.Vector3(0, 0, target.getHeading()), 
                                        60);
	specCam.attachTo(target.handle, 0, -1.75, 1.75, true);
	specCam.setActive(true);
    mp.game.cam.renderScriptCams(true, false, 0, true, false);                                    
}

mp.events.add('render', () => {
    if (specCam !== null && specCam.isActive() && specCam.isRendering()) {
        mp.game.controls.disableAllControlActions(2);

        var x = (mp.game.controls.getDisabledControlNormal(7, 1) * mouseSensitivity);
        var y = (mp.game.controls.getDisabledControlNormal(7, 2) * mouseSensitivity);
        var zoomIn = (mp.game.controls.getDisabledControlNormal(2, 40) * zoomSpeed);
        var zoomOut = (mp.game.controls.getDisabledControlNormal(2, 41) * zoomSpeed);

        var currentRot = specCam.getRot(2);

        currentRot = new mp.Vector3(currentRot.x - y, 0, currentRot.z - x);

        specCam.setRot(currentRot.x, currentRot.y, currentRot.z, 2);

        if (zoomIn > 0)
        {
            var currentFov = specCam.getFov();
            currentFov -= zoomIn;
            if (currentFov < minZoom)
                currentFov = minZoom;
            specCam.setFov(currentFov);
        } else if (zoomOut > 0)
        {
            var currentFov = specCam.getFov();
            currentFov += zoomOut;
            if (currentFov > maxZoom)
                currentFov = maxZoom;
            specCam.setFov(currentFov);
        }
    }
});