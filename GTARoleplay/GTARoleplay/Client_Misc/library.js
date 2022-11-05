function getCoordsInfront(position, heading, distance) {
    let x = position.x + Math.sin(degreesToRadians(heading)) * distance;
    let y = position.y + Math.cos(degreesToRadians(heading)) * distance;
    return new mp.Vector3(x, y, position.z);
}

function getCoordsToTheRight(position, heading, distance) {
    const coordsInFront = getCoordsInfront(position, heading, distance);
    let angle = degreesToRadians(90); // Convert to radians
    let x = Math.cos(angle) * (coordsInFront.x - position.x) - Math.sin(angle) * (coordsInFront.y - position.y) + position.x;
    let y = Math.sin(angle) * (coordsInFront.x - position.x) + Math.cos(angle) * (coordsInFront.y - position.y) + position.y;
    return new mp.Vector3(x, y, position.z);
}

function getCoordsBehind(position, heading, distance) {
    const coordsInFront = getCoordsInfront(position, heading, distance);
    let angle = degreesToRadians(180); // Convert to radians
    let x = Math.cos(angle) * (coordsInFront.x - position.x) - Math.sin(angle) * (coordsInFront.y - position.y) + position.x;
    let y = Math.sin(angle) * (coordsInFront.x - position.x) + Math.cos(angle) * (coordsInFront.y - position.y) + position.y;
    return new mp.Vector3(x, y, position.z);
}

function degreesToRadians(degree) {
    return -degree * Math.PI / 180.0;
}

exports.getCoordsInfront = getCoordsInfront;
exports.getCoordsToTheRight = getCoordsToTheRight;
exports.getCoordsBehind = getCoordsBehind;

