const { textJustification, generateRandomString, getColorFromValue, drawTextLabel } = require("gtaroleplay/TimerBars/util");
const { initialX, bgBaseX, bgOffset, bgThinOffset, timerBarWidth, timerBarHeight, timerBarThinHeight, titleScale, titleWrap } = require("gtaroleplay/TimerBars/coordsAndSizes");
const { drawSprite } = mp.game.graphics;

exports = class TimerBarBase {
    constructor(title) {
        this._id = generateRandomString(8);
        this._thin = false;
        this._titleGxtName = `TMRB_TITLE_${this._id}`;
        this._title = title;
        this._highlightColor = null;

        this.titleDrawParams = {
            font: 0,
            color: [240, 240, 240, 255],
            scale: titleScale,
            justification: textJustification.right,
            wrap: titleWrap,
            outline: true
        };

        mp.game.gxt.set(this._titleGxtName, title);
    }

    // Properties
    get title() {
        return this._title;
    }

    get titleColor() {
        return this.titleDrawParams.color;
    }

    get highlightColor() {
        return this._highlightColor;
    }

    set title(value) {
        this._title = value;
        mp.game.gxt.set(this._titleGxtName, value);
    }

    set titleColor(value) {
        this.titleDrawParams.color = getColorFromValue(value);
    }

    set highlightColor(value) {
        this._highlightColor = value ? getColorFromValue(value) : null;
    }

    // Functions
    drawBackground(y) {
        y += this._thin ? bgThinOffset : bgOffset;

        if (this._highlightColor) {
            drawSprite("timerbars", "all_white_bg", bgBaseX, y, timerBarWidth, this._thin ? timerBarThinHeight : timerBarHeight, 0.0, this._highlightColor[0], this._highlightColor[1], this._highlightColor[2], this._highlightColor[3]);
        }

        drawSprite("timerbars", "all_black_bg", bgBaseX, y, timerBarWidth, this._thin ? timerBarThinHeight : timerBarHeight, 0.0, 255, 255, 255, 140);
    }

    drawTitle(y) {
        drawTextLabel(this._titleGxtName, [initialX, y], this.titleDrawParams);
    }

    draw(y) {
        y -= 0.1;

        this.drawBackground(y);
        this.drawTitle(y);
    }

    resetGxt() {
        mp.game.gxt.reset(this._titleGxtName);
    }
};