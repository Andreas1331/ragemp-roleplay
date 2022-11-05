let distTimer, distanceTravelled = 0;
const requiredDistBeforeSave = 2; // in km
const statsIncreaserInterval = 5000; // in ms

mp.events.add("playerEnterVehicle", playerEnterVehicleHandler);
mp.events.add("playerLeaveVehicle", playerLeaveVehicleHandler);

mp.events.add('render', () =>
{
	handleGForPassenger();
});

function handleGForPassenger(){
	const controls = mp.game.controls;
	
	controls.enableControlAction(0, 23, true);
	controls.disableControlAction(0, 58, true);
	
	if(controls.isDisabledControlJustPressed(0, 58))
	{
		let position = mp.players.local.position;		
		let vehHandle = mp.game.vehicle.getClosestVehicle(position.x, position.y, position.z, 5, 0, 70);
		
		let vehicle = mp.vehicles.atHandle(vehHandle);
		
		if(vehicle
			&& vehicle.isAnySeatEmpty()
			&& vehicle.getSpeed() < 5)
		{
			mp.players.local.taskEnterVehicle(vehicle.handle, 5000, 0, 2, 1, 0);
		}
	}
}

function playerEnterVehicleHandler(vehicle, seat) {
	if(vehicle){
		// Ignore bicycles
		if(!mp.game.vehicle.isThisModelABicycle(mp.players.local.vehicle.model) && isDriver()){
			startVehicleStatsIncreaser();
		}
	}
}

function playerLeaveVehicleHandler(vehicle, seat){
	if(vehicle){
		if(!mp.game.vehicle.isThisModelABicycle(vehicle.model) && isDriver()){
			mp.events.callRemote('VehicleTravelled::Server', distanceTravelled);
			if(distTimer)
				clearInterval(distTimer);
			distTimer = null;
			distanceTravelled = 0;
		}
	}
}

function startVehicleStatsIncreaser() {
    let veh = mp.players.local.vehicle;
	if(!veh)
		return;
    let speed = 0;
    distTimer = setInterval(() => {
		// m/s
		speed = veh.getSpeed();
		// distance: speed * time
		distanceTravelled += ((speed * (statsIncreaserInterval/1000)) / 3600);
		// if the vehicle has travelled more than x meters
		if(distanceTravelled >= requiredDistBeforeSave){
			mp.events.callRemote('VehicleTravelled::Server', distanceTravelled);
			distanceTravelled = 0;
		}
    }, statsIncreaserInterval);
}

function isDriver() {
    if(mp.players.local.vehicle) return mp.players.local.vehicle.getPedInSeat(-1) === mp.players.local.handle;
}