const walkingStyles = {
    "Normal":  null,
    "Brave": "move_m@brave",
    "Confident": "move_m@confident",
    "Drunk": "move_m@drunk@verydrunk",
    "Fat": "move_m@fat@a",
    "Gangster": "move_m@shadyped@a",
    "Hurry": "move_m@hurry@a",
    "Injured": "move_m@injured",
    "Intimidated": "move_m@intimidation@1h",
    "Quick": "move_m@quick",
    "Sad": "move_m@sad@a",
    "Tough": "move_m@tool_belt@a"
};

function setWalkingStyle(player, style) {
    if (!style) {
        player.resetMovementClipset(0.0);
    } else {
        if (!mp.game.streaming.hasClipSetLoaded(style)) {
            mp.game.streaming.requestClipSet(style);
            while(!mp.game.streaming.hasClipSetLoaded(style)) mp.game.wait(0);
        }

        player.setMovementClipset(style, 0.0);
    }
}

mp.events.add("entityStreamIn", (entity) => {
    if (entity.type !== "player") return;
    setWalkingStyle(entity, walkingStyles[entity.getVariable("walkingStyle")]);
});

mp.events.addDataHandler("walkingStyle", (entity, value) => {
    if (entity.type === "player") setWalkingStyle(entity, walkingStyles[value]);
});