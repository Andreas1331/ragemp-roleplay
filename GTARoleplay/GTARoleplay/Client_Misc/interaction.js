const interactableEntities = (2 | 16); // 2: Vehicles (Use it as bit value to store more entities)
const interactionCheckerInterval = 350; // In ms
const raycastDistance = 5.5; // In meters
const raycastFromDistance = 0; // In meters behind the camera

global.interactEntityType = null;
global.interactEntityValue = null;

setInterval( () => { 
    var res = getCameraPointingAt();
    if(res){
        // Check if NaN should ignore static world objects as they don't have the getType() method
        if(isNaN(res.entity)){
            global.interactEntityType = res.entity.getType();
            global.interactEntityValue = res.entity.remoteId;
        }
	}else{
        global.interactEntityType = null;
        global.interactEntityValue = null;
	}
}, interactionCheckerInterval);

function getCameraPointingAt() {
    const camera = mp.cameras.new("gameplay"); // gets the current gameplay camera

    let position = camera.getCoord(); // grab the position of the gameplay camera as Vector3

    let direction = camera.getDirection(); // get the forwarding vector of the direction you aim with the gameplay camera as Vector3

    let fromPosition = new mp.Vector3((direction.x * raycastFromDistance) + (position.x),
                                      (direction.y * raycastFromDistance) + (position.y),
                                      (direction.z * raycastFromDistance) + (position.z));
    let farAway = new mp.Vector3((direction.x * raycastDistance) + (position.x), (direction.y * raycastDistance) + (position.y), (direction.z * raycastDistance) + (position.z)); // calculate a random point, drawn on a invisible line between camera position and direction (* distance)

    // Ignore the player when raycasting
    var result = mp.raycasting.testPointToPoint(fromPosition, farAway, mp.players.local, interactableEntities); // now test point to point - intersects with map and objects [1 + 16]

    return result; // and return the result ( undefined, if no hit )
}