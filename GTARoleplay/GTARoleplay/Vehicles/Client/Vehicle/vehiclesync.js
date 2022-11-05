mp.events.add("VehStream_SetEngineStatus", (veh, status) => {
    if (veh !== undefined && veh !== null && veh.type == 'vehicle' && typeof(veh.getType) != "undefined" && veh.getType() == 2) {
        if (veh.isSeatFree(-1)) //Turns engine on instantly if no driver, otherwise it will not turn on
        {
            veh.setEngineOn(status, true, true);
            veh.setUndriveable(true);

            if(status)
            {
                veh.setLights(0);
            }
            else
            {
                veh.setLights(1);
            }
            return;
        }
        else {
            veh.setEngineOn(status, false, true);
            veh.setUndriveable(!status);
            if(status) 
            {
                veh.setLights(0);
            }
            else
            {
                veh.setLights(1);
            }
            return;
        }
    }
});

mp.events.add("VehStream_SetVehicleDoorStatus_Single", (veh, door, state) => {
    if (veh !== undefined && veh !== null && veh.type == 'vehicle' && typeof(veh.getType) != "undefined" && veh.getType() == 2) {
        if (state === 0) {
            veh.setDoorShut(door, false);
        }
        else if (state === 1) {
            veh.setDoorOpen(door, false, false);
        }
        else {
            veh.setDoorBroken(door, true);
        }
    }
});

//Sync data on stream in
mp.events.add("entityStreamIn", (entity) => {
	if (entity === null || entity === undefined)
		return;
    if (entity.type === "vehicle") {
        let typeor = typeof entity.getVariable('VehicleSyncData');
        let actualData = entity.getVariable('VehicleSyncData');
        
        //Needed to stop vehicles from freaking out
        mp.game.streaming.requestCollisionAtCoord(entity.position.x, entity.position.y, entity.position.z);
        entity.setLoadCollisionFlag(true);
        entity.trackVisibility();
        entity.setBurnout(false);
        
        //Set doors unbreakable for a moment
        let a = 0;
        for (a = 0; a < 8; a++) {
            entity.setDoorBreakable(a, false);
        }

        if (typeor !== 'undefined') {
            if (actualData.Engine) {
                entity.setEngineOn(true, true, true);
                entity.setLights(0);
            }
            else {
                entity.setEngineOn(false, true, true);
                entity.setLights(1);
            }
        }
		
        //Make doors breakable again
		if (entity == null || entity === undefined || typeof(entity) === 'number')
			return;
		if (!entity.doesExist())
			return;
        for (x = 0; x < 8; x++) {
            entity.setDoorBreakable(x, true);
        }
    }
});