const { textJustification, drawTextLabel } = require("gtaroleplay/TimerBars/util");
const { initialX, textOffset, playerTitleOffset, playerTitleScale, titleWrap } = require("gtaroleplay/TimerBars/coordsAndSizes");
const TextTimerBar = require("gtaroleplay/TimerBars/classes/TextTimerBar");

exports = class PlayerTimerBar extends TextTimerBar {
    constructor(title, text) {
        super(title, text);

        this.titleDrawParams = {
            font: 4,
            color: [240, 240, 240, 255],
            scale: playerTitleScale,
            justification: textJustification.right,
            wrap: titleWrap,
            shadow: true
        };
    }

    // Functions
    draw(y) {
        super.drawBackground(y);

        drawTextLabel(this._titleGxtName, [initialX, y + playerTitleOffset], this.titleDrawParams);
        drawTextLabel(this._textGxtName, [initialX, y + textOffset], this.textDrawParams);
    }
};