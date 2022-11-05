const { checkpointBaseX, checkpointOffsetX, checkpointOffsetY, checkpointWidth, checkpointHeight } = require("gtaroleplay/TimerBars/coordsAndSizes");
const { clamp, getColorFromValue } = require("gtaroleplay/TimerBars/util");
const { drawSprite } = mp.game.graphics;
const TimerBarBase = require("gtaroleplay/TimerBars/classes/TimerBarBase");

exports = class CheckpointTimerBar extends TimerBarBase {
    static state = {
        inProgress: 0,
        completed: 1,
        failed: 2
    };

    constructor(title, numCheckpoints) {
        super(title);

        this._thin = true;
        this._color = [255, 255, 255, 255];
        this._inProgressColor = [255, 255, 255, 51];
        this._failedColor = [0, 0, 0, 255];
        this._checkpointStates = {};
        this._numCheckpoints = clamp(numCheckpoints, 0, 16);
    }

    // Properties
    get numCheckpoints() {
        return this._numCheckpoints;
    }

    get color() {
        return this._color;
    }

    get inProgressColor() {
        return this._inProgressColor;
    }

    get failedColor() {
        return this._failedColor;
    }

    set color(value) {
        this._color = getColorFromValue(value);
    }

    set inProgressColor(value) {
        this._inProgressColor = getColorFromValue(value);
    }

    set failedColor(value) {
        this._failedColor = getColorFromValue(value);
    }

    // Functions
    setCheckpointState(index, newState) {
        if (index < 0 || index >= this._numCheckpoints) {
            return;
        }

        this._checkpointStates[index] = newState;
    }

    setAllCheckpointsState(newState) {
        for (let i = 0; i < this._numCheckpoints; i++) {
            this._checkpointStates[i] = newState;
        }
    }

    draw(y) {
        super.draw(y);
        y += checkpointOffsetY;

        for (let i = 0, cpX = checkpointBaseX; i < this._numCheckpoints; i++) {
            const drawColor = this._checkpointStates[i] ? (this._checkpointStates[i] === CheckpointTimerBar.state.failed ? this._failedColor : this._color) : this._inProgressColor;
            drawSprite("timerbars", "circle_checkpoints", cpX, y, checkpointWidth, checkpointHeight, 0.0, drawColor[0], drawColor[1], drawColor[2], drawColor[3])

            cpX -= checkpointOffsetX;
        }
    }
};