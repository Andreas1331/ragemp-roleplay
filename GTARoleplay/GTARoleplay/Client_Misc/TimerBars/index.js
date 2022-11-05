const { hideHudComponents } = require("gtaroleplay/TimerBars/util");
const { gfxAlignWidth, gfxAlignHeight, initialY, initialBusySpinnerY, timerBarMargin, timerBarThinMargin } = require("gtaroleplay/TimerBars/coordsAndSizes");
const TimerBarBase = require("gtaroleplay/TimerBars/classes/TimerBarBase");
const invoke = mp.game.invoke;

const BUSYSPINNER_IS_ON = "0xD422FCC5F239A915";
const SET_SCRIPT_GFX_ALIGN = "0xB8A850F20A067EB6";
const SET_SCRIPT_GFX_ALIGN_PARAMS = "0xF5A2C681787E579D";
const RESET_SCRIPT_GFX_ALIGN = "0xE3A3DB414A373DAB";

let timerBarPool = [];

// Request texture dictionary
mp.game.graphics.requestStreamedTextureDict("timerbars", true);

// Rendering
mp.events.add("render", () => {
    const max = timerBarPool.length;
    if (max === 0) {
        return;
    }

    // Hide HUD items that appear near timerbars
    hideHudComponents();

    // Apply drawing config
    invoke(SET_SCRIPT_GFX_ALIGN, 82, 66);
    invoke(SET_SCRIPT_GFX_ALIGN_PARAMS, 0.0, 0.0, gfxAlignWidth, gfxAlignHeight);

    // Draw
    for (let i = 0, drawY = (invoke(BUSYSPINNER_IS_ON) ? initialBusySpinnerY : initialY); i < max; i++) {
        timerBarPool[i].draw(drawY);
        drawY -= timerBarPool[i]._thin ? timerBarThinMargin : timerBarMargin;
    }

    // Reset drawing config
    invoke(RESET_SCRIPT_GFX_ALIGN);
});

// API
exports = {
    add: function(...args) {
        const validTimerBars = args.filter(arg => arg instanceof TimerBarBase);
        timerBarPool.push(...validTimerBars);
    },

    has: function(timerBar) {
        return timerBarPool.includes(timerBar);
    },

    remove: function(timerBar) {
        const idx = timerBarPool.indexOf(timerBar);
        if (idx === -1) {
            return;
        }

        timerBarPool.splice(idx, 1);
    },

    clear: function() {
        timerBarPool = [];
    }
};