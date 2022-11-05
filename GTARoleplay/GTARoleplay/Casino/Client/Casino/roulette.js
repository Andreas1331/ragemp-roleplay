const instructions = require('gtaroleplay/InstructionalButtons/better_instructions');
const library = require("gtaroleplay/library.js");
const timerBarPool = require("gtaroleplay/TimerBars");
const TextTimerBar = require("gtaroleplay/TimerBars/classes/TextTimerBar");

/* Instructional buttons down right corner */
const horizontalInstructionList = new instructions(-1);
horizontalInstructionList.addButton('Leave', 194);
horizontalInstructionList.addButton('Decrease bet', 173);
horizontalInstructionList.addButton('Increase bet', 172);
horizontalInstructionList.addButton('Remove bet', 25);
horizontalInstructionList.addButton('Place bet', 24);

// 0x26 is the UpArrow key code
mp.keys.bind(0x26, true, upArrow_KeyPressedDown);
// 0x28 is the DownArrow key code
mp.keys.bind(0x28, true, downArrow_KeyPressedDown);

const currentBetBar = new TextTimerBar("Current bet:", "$50");
const timeLeftBar = new TextTimerBar("Time left:", "60s");
currentBetBar.textColor = 18;

let currentSpins = 0;
let lastSpinTime = 0;
let isSpinning = false;
let canBet = false;
const neededSpins = 5;

/* Handles for the game objects */
let table = null;
let ball = null;

/* Which animation to play when the wheel finishes */
let wheelAnim = "";
let ballAnim = "";

/* Camera */
let rouletteCamera = null;
const mouseSensitivity = 6.5;

let tableMarkers = []; // The game objects on the table indicating what the player points at
let chipsOnTable = {}; // The chips placed on the table indicating what the player has bet on
let currentHighlightedField = null; // Which betting field the player is hovering his aim on
let chipObject = null; // The chip object that moves around with the aim of the player to point with

/* Timer */
let timerHandle = null;
let timeLeft = 60;

/* Betting variables */
const maxBetting = 500;
const bettingIncreaseSize = 50;
let currentBettingValue = bettingIncreaseSize;
let currentBettingIndices = [];

// The types of bets a player can do: Split, Red, StraightUp etc.
const rouletteBets =
{
    StraightUp: 1 << 0,
    Split: 1 << 1,
    Basket: 1 << 2,
    Street: 1 << 3,
    Corner: 1 << 4,
    SixLine: 1 << 5, //
    FirstColumn: 1 << 6,
    SecondColumn: 1 << 7,
    ThirdColumn: 1 << 8,
    FirstDozen: 1 << 9,
    SecondDozen: 1 << 10,
    ThirdDozen: 1 << 11,
    Odd: 1 << 12,
    Even: 1 << 13,
    Red: 1 << 14,
    Black: 1 << 15,
    OneToEigthteen: 1 << 16, // Lower number
    NineteenToThirtySix: 1 << 17 // High number
}

mp.events.add('SitAtRouletteTable::Client', (tableObj, ballObj,) => {
    table = tableObj;
    ball = ballObj;
    canBet = false;

    // Setup the camera
    mp.game.streaming.requestAnimDict("anim_casino_b@amb@casino@games@roulette@table");
    setupCamera(tableObj.position, tableObj.getHeading());
    setupInstructionalUI();

    // Create the floating chip
    if (chipObject == null) {
        chipObject = mp.objects.new(mp.game.joaat("vw_prop_chip_100dollar_x1"),
            new mp.Vector3(tableObj.position.x, tableObj.position.y, tableObj.position.z + 0.4));
        chipObject.setCollision(false, false);
    }
});

mp.events.add('OpenRouletteForBets::Client', () => {
    if(!doesRouletteExist())
        return;

    // Give the player 1 minute to place his bets
    startTimer();
    isSpinning = false;
    canBet = true;
    currentBettingIndices = [];
    destroyAllChipsOnTable();
});

mp.events.add('StartRouletteWheel::Client', (wheelAnimTmp, ballAnimTmp) => {
    if(!doesRouletteExist())
        return;

    wheelAnim = wheelAnimTmp;
    ballAnim = ballAnimTmp;

    startTheWheel();
});

function setupInstructionalUI() {
    if (!horizontalInstructionList.isActive()) {
        horizontalInstructionList.toggleHud(true);
    }
    timerBarPool.add(currentBetBar, timeLeftBar);
}

function setupCamera(tablePos, heading) {
    // Create a new camera
    const coordsNextTo = library.getCoordsBehind(tablePos, heading, 1);

    rouletteCamera = mp.cameras.new('default', new mp.Vector3(coordsNextTo.x, coordsNextTo.y, coordsNextTo.z + 2.5),
        new mp.Vector3(-90, 0, heading),
        60);
    rouletteCamera.setActive(true);
    mp.game.cam.renderScriptCams(true, false, 0, true, false);
}

function startTheWheel() {
    if(!doesRouletteExist())
        return;

    table.playAnim("intro_wheel", "anim_casino_b@amb@casino@games@roulette@table", 1000.0, false, true, true, 0, 131072);
    table.forceAiAndAnimationUpdate();
    const ballPos = table.getWorldPositionOfBone(table.getBoneIndexByName("Roulette_Wheel"));
    ball.position = ballPos;

    ball.setCoordsNoOffset(ballPos.x, ballPos.y, ballPos.z, !1, !1, !1);
    const ballRot = table.getRotation(2);
    ball.setRotation(ballRot.x, ballRot.y, ballRot.z + 90, 2, !1)

    ball.playAnim("intro_ball", "anim_casino_b@amb@casino@games@roulette@table", 1000.0, false, true, false, 0, 136704);
    ball.forceAiAndAnimationUpdate();

    resetWheelValues();
}

function startTimer() {
    timeLeft = 60;
    if (timerHandle) {
        clearInterval(timerHandle);
    }
    timerHandle = setInterval(timerElapsed, 1000);
}

function timerElapsed() {
    timeLeft--;
    if (timeLeft < 0) {
        clearInterval(timerHandle);
        timeLeft = 0;
    }

    timeLeftBar.text = timeLeft + "s";
}

function upArrow_KeyPressedDown() {
    if (!global.loggedIn || global.isChatOpened || !doesRouletteExist())
        return;

    if (currentBettingValue < maxBetting) {
        currentBettingValue += bettingIncreaseSize;
        currentBetBar.text = "$" + currentBettingValue;
    }
}

function downArrow_KeyPressedDown() {
    if (!global.loggedIn || global.isChatOpened || !doesRouletteExist())
        return;
    if (currentBettingValue > bettingIncreaseSize) {
        currentBettingValue -= bettingIncreaseSize;
        currentBetBar.text = "$" + currentBettingValue;
    }
}

mp.events.add('render', () => {
    if (!doesRouletteExist())
        return;

    // Detect the players bet choices
    if (chipObject != null) {
        // Left mouse button
        if (mp.game.controls.isDisabledControlJustReleased(0, 25) && !mp.gui.cursor.visible)
            handleRightClick();

        if (mp.game.controls.isDisabledControlJustReleased(0, 24) && !mp.gui.cursor.visible)
            handleLeftClick();

        if (mp.game.controls.isDisabledControlJustReleased(0, 194) && !mp.gui.cursor.visible){
            leaveTable();
            return;
        }

        let hitObject = getCameraHitObject();
        if (hitObject != null) {

            let closestTableFieldIndex = getClosestTableField(hitObject.position);
            if (closestTableFieldIndex !== null) {
                // the raycast has hit close to one of the fields on the table
                createTableFieldObjects(closestTableFieldIndex);
            }

            chipObject.setCoordsNoOffset(hitObject.position.x, hitObject.position.y, table.position.z + 0.95, false, false, false);
        }
    }

    handleMouseMovement();

    if (isSpinning) {

        if (table.isPlayingAnim("anim_casino_b@amb@casino@games@roulette@table", "intro_wheel", 3)) {
            if (table.getAnimCurrentTime("anim_casino_b@amb@casino@games@roulette@table", "intro_wheel") > 0.9425) {
                table.playAnim("loop_wheel", "anim_casino_b@amb@casino@games@roulette@table", 1000.0, true, true, true, 0, 131072);
            }
        }

        if (ball.isPlayingAnim("anim_casino_b@amb@casino@games@roulette@table", "intro_ball", 3)) {
            if (ball.getAnimCurrentTime("anim_casino_b@amb@casino@games@roulette@table", "intro_ball") > 0.99) {
                const ballPos = table.getWorldPositionOfBone(table.getBoneIndexByName("Roulette_Wheel"));
                const ballRot = table.getRotation(2);
                ball.position = new mp.Vector3(ballPos.x, ballPos.y, ballPos.z);
                ball.rotation = new mp.Vector3(ballRot.x, ballRot.y, ballRot.z + 90);

                ball.playAnim("loop_ball", "anim_casino_b@amb@casino@games@roulette@table", 1000.0, true, true, false, 0, 136704);
            }
        }

        if (table.isPlayingAnim("anim_casino_b@amb@casino@games@roulette@table", "loop_wheel", 3)) {
            /* One trip/lap around the wheel takes roughly 1 second */
            if (table.getAnimCurrentTime("anim_casino_b@amb@casino@games@roulette@table", "loop_wheel") >= 0.9 && Date.now() - lastSpinTime > 1000) {
                currentSpins++;
                lastSpinTime = Date.now();
            }
            if (currentSpins == neededSpins - 1) {
                ball.setAnimSpeed("anim_casino_b@amb@casino@games@roulette@table", "loop_ball", 0.70);
            }
            if (currentSpins == neededSpins && table.getAnimCurrentTime("anim_casino_b@amb@casino@games@roulette@table", "loop_wheel") > 0.99) {
                table.playAnim(wheelAnim, "anim_casino_b@amb@casino@games@roulette@table", 1000.0, false, true, true, 0, 131072);

                const ballPos = table.getWorldPositionOfBone(table.getBoneIndexByName("Roulette_Wheel"));
                const ballRot = table.getRotation(2);
                ball.position = new mp.Vector3(ballPos.x, ballPos.y, ballPos.z);
                ball.rotation = new mp.Vector3(ballRot.x, ballRot.y, ballRot.z + 90);
                ball.playAnim(ballAnim, "anim_casino_b@amb@casino@games@roulette@table", 1000.0, false, true, true, 0, 136704);

                isSpinning = false;
            }
        }
    }
});


const tableMarkersOffsets =
{
    "0": [-0.137451171875, -0.146942138671875, 0.9449996948242188],
    "00": [-0.1387939453125, 0.10546875, 0.9449996948242188],
    "1": [-0.0560302734375, -0.1898193359375, 0.9449996948242188],
    "2": [-0.0567626953125, -0.024017333984375, 0.9449996948242188],
    "3": [-0.056884765625, 0.141632080078125, 0.9449996948242188],
    "4": [0.02392578125, -0.187347412109375, 0.9449996948242188],
    "5": [0.0240478515625, -0.02471923828125, 0.9449996948242188],
    "6": [0.02392578125, 0.1422119140625, 0.9449996948242188],
    "7": [0.1038818359375, -0.18902587890625, 0.9449996948242188],
    "8": [0.1044921875, -0.023834228515625, 0.9449996948242188],
    "9": [0.10546875, 0.1419677734375, 0.9449996948242188],
    "10": [0.18701171875, -0.188385009765625, 0.9449996948242188],
    "11": [0.18603515625, -0.0238037109375, 0.9449996948242188],
    "12": [0.1851806640625, 0.143157958984375, 0.9449996948242188],
    "13": [0.2677001953125, -0.18780517578125, 0.9449996948242188],
    "14": [0.26806640625, -0.02301025390625, 0.9449996948242188],
    "15": [0.26611328125, 0.143310546875, 0.9449996948242188],
    "16": [0.3497314453125, -0.18829345703125, 0.9449996948242188],
    "17": [0.349609375, -0.023101806640625, 0.9449996948242188],
    "18": [0.3497314453125, 0.142242431640625, 0.9449996948242188],
    "19": [0.4307861328125, -0.18829345703125, 0.9449996948242188],
    "20": [0.4312744140625, -0.02392578125, 0.9449996948242188],
    "21": [0.431884765625, 0.1416015625, 0.9449996948242188],
    "22": [0.51220703125, -0.188873291015625, 0.9449996948242188],
    "23": [0.5123291015625, -0.023773193359375, 0.9449996948242188],
    "24": [0.511962890625, 0.14215087890625, 0.9449996948242188],
    "25": [0.5931396484375, -0.18890380859375, 0.9449996948242188],
    "26": [0.59375, -0.023651123046875, 0.9449996948242188],
    "27": [0.59375, 0.14080810546875, 0.9449996948242188],
    "28": [0.67529296875, -0.189849853515625, 0.9449996948242188],
    "29": [0.6751708984375, -0.02337646484375, 0.9449996948242188],
    "30": [0.674560546875, 0.141845703125, 0.9449996948242188],
    "31": [0.756591796875, -0.18798828125, 0.9449996948242188],
    "32": [0.7547607421875, -0.0234375, 0.9449996948242188],
    "33": [0.7554931640625, 0.14263916015625, 0.9449996948242188],
    "34": [0.836669921875, -0.188323974609375, 0.9449996948242188],
    "35": [0.8365478515625, -0.0244140625, 0.9449996948242188],
    "36": [0.8359375, 0.14276123046875, 0.9449996948242188]
};

const tableChipsOffsets =
    [
        [-0.154541015625, -0.150604248046875, 0.9449996948242188, ["0"], (rouletteBets.StraightUp)],
        [-0.1561279296875, 0.11505126953125, 0.9449996948242188, ["00"], (rouletteBets.StraightUp)],
        [-0.059326171875, -0.18701171875, 0.9449996948242188, ["1"], (rouletteBets.StraightUp)],
        [-0.058349609375, -0.019378662109375, 0.9449996948242188, ["2"], (rouletteBets.StraightUp)],
        [-0.0587158203125, 0.142059326171875, 0.9449996948242188, ["3"], (rouletteBets.StraightUp)],
        [0.02294921875, -0.1920166015625, 0.9449996948242188, ["4"], (rouletteBets.StraightUp)],
        [0.023193359375, -0.01947021484375, 0.9449996948242188, ["5"], (rouletteBets.StraightUp)],
        [0.024658203125, 0.147369384765625, 0.9449996948242188, ["6"], (rouletteBets.StraightUp)],
        [0.105224609375, -0.1876220703125, 0.9449996948242188, ["7"], (rouletteBets.StraightUp)],
        [0.1055908203125, -0.028472900390625, 0.9449996948242188, ["8"], (rouletteBets.StraightUp)],
        [0.10400390625, 0.147430419921875, 0.9449996948242188, ["9"], (rouletteBets.StraightUp)],
        [0.187744140625, -0.191802978515625, 0.9449996948242188, ["10"], (rouletteBets.StraightUp)],
        [0.1866455078125, -0.02667236328125, 0.9449996948242188, ["11"], (rouletteBets.StraightUp)],
        [0.1842041015625, 0.145965576171875, 0.9449996948242188, ["12"], (rouletteBets.StraightUp)],
        [0.2696533203125, -0.182464599609375, 0.9449996948242188, ["13"], (rouletteBets.StraightUp)],
        [0.265869140625, -0.027862548828125, 0.9449996948242188, ["14"], (rouletteBets.StraightUp)],
        [0.2667236328125, 0.138946533203125, 0.9449996948242188, ["15"], (rouletteBets.StraightUp)],
        [0.35009765625, -0.186126708984375, 0.9449996948242188, ["16"], (rouletteBets.StraightUp)],
        [0.348876953125, -0.027740478515625, 0.9449996948242188, ["17"], (rouletteBets.StraightUp)],
        [0.3497314453125, 0.14715576171875, 0.9449996948242188, ["18"], (rouletteBets.StraightUp)],
        [0.43212890625, -0.17864990234375, 0.9449996948242188, ["19"], (rouletteBets.StraightUp)],
        [0.4337158203125, -0.02508544921875, 0.9449996948242188, ["20"], (rouletteBets.StraightUp)],
        [0.430419921875, 0.138336181640625, 0.9449996948242188, ["21"], (rouletteBets.StraightUp)],
        [0.51416015625, -0.18603515625, 0.9449996948242188, ["22"], (rouletteBets.StraightUp)],
        [0.5135498046875, -0.02301025390625, 0.9449996948242188, ["23"], (rouletteBets.StraightUp)],
        [0.5146484375, 0.14239501953125, 0.9449996948242188, ["24"], (rouletteBets.StraightUp)],
        [0.59130859375, -0.192413330078125, 0.9449996948242188, ["25"], (rouletteBets.StraightUp)],
        [0.596923828125, -0.022216796875, 0.9449996948242188, ["26"], (rouletteBets.StraightUp)],
        [0.5924072265625, 0.14385986328125, 0.9449996948242188, ["27"], (rouletteBets.StraightUp)],
        [0.6749267578125, -0.187286376953125, 0.9449996948242188, ["28"], (rouletteBets.StraightUp)],
        [0.67431640625, -0.0262451171875, 0.9449996948242188, ["29"], (rouletteBets.StraightUp)],
        [0.6756591796875, 0.140594482421875, 0.9449996948242188, ["30"], (rouletteBets.StraightUp)],
        [0.7542724609375, -0.19415283203125, 0.9449996948242188, ["31"], (rouletteBets.StraightUp)],
        [0.7542724609375, -0.01898193359375, 0.9449996948242188, ["32"], (rouletteBets.StraightUp)],
        [0.75439453125, 0.1448974609375, 0.9449996948242188, ["33"], (rouletteBets.StraightUp)],
        [0.8392333984375, -0.18951416015625, 0.9449996948242188, ["34"], (rouletteBets.StraightUp)],
        [0.837646484375, -0.023468017578125, 0.9449996948242188, ["35"], (rouletteBets.StraightUp)],
        [0.8380126953125, 0.14227294921875, 0.9449996948242188, ["36"], (rouletteBets.StraightUp)],
        [-0.1368408203125, -0.02099609375, 0.9449996948242188, ["0", "00"], (rouletteBets.Split)],
        [-0.055419921875, -0.105804443359375, 0.9449996948242188, ["1", "2"], (rouletteBets.Split)],
        [-0.0567626953125, 0.058624267578125, 0.9449996948242188, ["2", "3"], (rouletteBets.Split)],
        [0.02587890625, -0.10498046875, 0.9449996948242188, ["4", "5"], (rouletteBets.Split)],
        [0.0244140625, 0.058837890625, 0.9449996948242188, ["5", "6"], (rouletteBets.Split)],
        [0.100341796875, -0.10382080078125, 0.9449996948242188, ["7", "8"], (rouletteBets.Split)],
        [0.1064453125, 0.06011962890625, 0.9449996948242188, ["8", "9"], (rouletteBets.Split)],
        [0.19189453125, -0.1060791015625, 0.9449996948242188, ["10", "11"], (rouletteBets.Split)],
        [0.1856689453125, 0.05438232421875, 0.9449996948242188, ["11", "12"], (rouletteBets.Split)],
        [0.27099609375, -0.10870361328125, 0.9449996948242188, ["13", "14"], (rouletteBets.Split)],
        [0.2667236328125, 0.058502197265625, 0.9449996948242188, ["14", "15"], (rouletteBets.Split)],
        [0.3463134765625, -0.107696533203125, 0.9449996948242188, ["16", "17"], (rouletteBets.Split)],
        [0.34814453125, 0.0556640625, 0.9449996948242188, ["17", "18"], (rouletteBets.Split)],
        [0.42822265625, -0.109130859375, 0.9449996948242188, ["19", "20"], (rouletteBets.Split)],
        [0.4302978515625, 0.0550537109375, 0.9449996948242188, ["20", "21"], (rouletteBets.Split)],
        [0.511474609375, -0.107421875, 0.9449996948242188, ["22", "23"], (rouletteBets.Split)],
        [0.512451171875, 0.0614013671875, 0.9449996948242188, ["23", "24"], (rouletteBets.Split)],
        [0.5980224609375, -0.107147216796875, 0.9449996948242188, ["25", "26"], (rouletteBets.Split)],
        [0.596435546875, 0.0574951171875, 0.9449996948242188, ["26", "27"], (rouletteBets.Split)],
        [0.673828125, -0.106903076171875, 0.9449996948242188, ["28", "29"], (rouletteBets.Split)],
        [0.6751708984375, 0.058685302734375, 0.9449996948242188, ["29", "30"], (rouletteBets.Split)],
        [0.7532958984375, -0.1102294921875, 0.9449996948242188, ["31", "32"], (rouletteBets.Split)],
        [0.750244140625, 0.06103515625, 0.9449996948242188, ["32", "33"], (rouletteBets.Split)],
        [0.834716796875, -0.108978271484375, 0.9449996948242188, ["34", "35"], (rouletteBets.Split)],
        [0.836181640625, 0.05828857421875, 0.9449996948242188, ["35", "36"], (rouletteBets.Split)],
        [-0.0167236328125, -0.187042236328125, 0.9449996948242188, ["1", "4"], (rouletteBets.Split)],
        [-0.0167236328125, -0.02154541015625, 0.9449996948242188, ["2", "5"], (rouletteBets.Split)],
        [-0.0164794921875, 0.140350341796875, 0.9449996948242188, ["3", "6"], (rouletteBets.Split)],
        [0.064453125, -0.1865234375, 0.9449996948242188, ["4", "7"], (rouletteBets.Split)],
        [0.06494140625, -0.01727294921875, 0.9449996948242188, ["5", "8"], (rouletteBets.Split)],
        [0.068603515625, 0.13873291015625, 0.9449996948242188, ["6", "9"], (rouletteBets.Split)],
        [0.144287109375, -0.184173583984375, 0.9449996948242188, ["7", "10"], (rouletteBets.Split)],
        [0.14501953125, -0.024139404296875, 0.9449996948242188, ["8", "11"], (rouletteBets.Split)],
        [0.14501953125, 0.136993408203125, 0.9449996948242188, ["9", "12"], (rouletteBets.Split)],
        [0.2291259765625, -0.18670654296875, 0.9449996948242188, ["10", "13"], (rouletteBets.Split)],
        [0.227783203125, -0.0242919921875, 0.9449996948242188, ["11", "14"], (rouletteBets.Split)],
        [0.2286376953125, 0.14398193359375, 0.9449996948242188, ["12", "15"], (rouletteBets.Split)],
        [0.308349609375, -0.18792724609375, 0.9449996948242188, ["13", "16"], (rouletteBets.Split)],
        [0.308837890625, -0.02374267578125, 0.9449996948242188, ["14", "17"], (rouletteBets.Split)],
        [0.3099365234375, 0.14410400390625, 0.9449996948242188, ["15", "18"], (rouletteBets.Split)],
        [0.39111328125, -0.192230224609375, 0.9449996948242188, ["16", "19"], (rouletteBets.Split)],
        [0.390869140625, -0.0189208984375, 0.9449996948242188, ["17", "20"], (rouletteBets.Split)],
        [0.39111328125, 0.146514892578125, 0.9449996948242188, ["18", "21"], (rouletteBets.Split)],
        [0.470947265625, -0.188690185546875, 0.9449996948242188, ["19", "22"], (rouletteBets.Split)],
        [0.4705810546875, -0.0205078125, 0.9449996948242188, ["20", "23"], (rouletteBets.Split)],
        [0.4725341796875, 0.140167236328125, 0.9449996948242188, ["21", "24"], (rouletteBets.Split)],
        [0.5491943359375, -0.189666748046875, 0.9449996948242188, ["22", "25"], (rouletteBets.Split)],
        [0.548095703125, -0.022552490234375, 0.9449996948242188, ["23", "26"], (rouletteBets.Split)],
        [0.553955078125, 0.1446533203125, 0.9449996948242188, ["24", "27"], (rouletteBets.Split)],
        [0.6324462890625, -0.191131591796875, 0.9449996948242188, ["25", "28"], (rouletteBets.Split)],
        [0.635498046875, -0.0224609375, 0.9449996948242188, ["26", "29"], (rouletteBets.Split)],
        [0.6392822265625, 0.139190673828125, 0.9449996948242188, ["27", "30"], (rouletteBets.Split)],
        [0.71533203125, -0.187042236328125, 0.9449996948242188, ["28", "31"], (rouletteBets.Split)],
        [0.7181396484375, -0.02447509765625, 0.9449996948242188, ["29", "32"], (rouletteBets.Split)],
        [0.7152099609375, 0.138153076171875, 0.9449996948242188, ["30", "33"], (rouletteBets.Split)],
        [0.7969970703125, -0.1904296875, 0.9449996948242188, ["31", "34"], (rouletteBets.Split)],
        [0.7955322265625, -0.024871826171875, 0.9449996948242188, ["32", "35"], (rouletteBets.Split)],
        [0.7960205078125, 0.137664794921875, 0.9449996948242188, ["33", "36"], (rouletteBets.Split)],
        [-0.0560302734375, -0.271240234375, 0.9449996948242188, ["1", "2", "3"], (rouletteBets.Street)],
        [0.024658203125, -0.271392822265625, 0.9449996948242188, ["4", "5", "6"], (rouletteBets.Street)],
        [0.1051025390625, -0.272125244140625, 0.9449996948242188, ["7", "8", "9"], (rouletteBets.Street)],
        [0.1898193359375, -0.27001953125, 0.9449996948242188, ["10", "11", "12"], (rouletteBets.Street)],
        [0.2696533203125, -0.271697998046875, 0.9449996948242188, ["13", "14", "15"], (rouletteBets.Street)],
        [0.351318359375, -0.268096923828125, 0.9449996948242188, ["16", "17", "18"], (rouletteBets.Street)],
        [0.4287109375, -0.269561767578125, 0.9449996948242188, ["19", "20", "21"], (rouletteBets.Street)],
        [0.5098876953125, -0.2716064453125, 0.9449996948242188, ["22", "23", "24"], (rouletteBets.Street)],
        [0.5960693359375, -0.271148681640625, 0.9449996948242188, ["25", "26", "27"], (rouletteBets.Street)],
        [0.67724609375, -0.268524169921875, 0.9449996948242188, ["28", "29", "30"], (rouletteBets.Street)],
        [0.7523193359375, -0.27227783203125, 0.9449996948242188, ["31", "32", "33"], (rouletteBets.Street)],
        [0.8382568359375, -0.272125244140625, 0.9449996948242188, ["34", "35", "36"], (rouletteBets.Street)],
        [-0.017333984375, -0.106170654296875, 0.9449996948242188, ["1", "2", "4", "5"], (rouletteBets.Corner)],
        [-0.0162353515625, 0.060882568359375, 0.9449996948242188, ["2", "3", "5", "6"], (rouletteBets.Corner)],
        [0.06591796875, -0.110107421875, 0.9449996948242188, ["4", "5", "7", "8"], (rouletteBets.Corner)],
        [0.0653076171875, 0.060028076171875, 0.9449996948242188, ["5", "6", "8", "9"], (rouletteBets.Corner)],
        [0.146484375, -0.10888671875, 0.9449996948242188, ["7", "8", "10", "11"], (rouletteBets.Corner)],
        [0.1451416015625, 0.057159423828125, 0.9449996948242188, ["8", "9", "11", "12"], (rouletteBets.Corner)],
        [0.22705078125, -0.1092529296875, 0.9449996948242188, ["10", "11", "13", "14"], (rouletteBets.Corner)],
        [0.22802734375, 0.059356689453125, 0.9449996948242188, ["11", "12", "14", "15"], (rouletteBets.Corner)],
        [0.307373046875, -0.1043701171875, 0.9449996948242188, ["13", "14", "16", "17"], (rouletteBets.Corner)],
        [0.309814453125, 0.05584716796875, 0.9449996948242188, ["14", "15", "17", "18"], (rouletteBets.Corner)],
        [0.3919677734375, -0.111083984375, 0.9449996948242188, ["16", "17", "19", "20"], (rouletteBets.Corner)],
        [0.3924560546875, 0.0596923828125, 0.9449996948242188, ["17", "18", "20", "21"], (rouletteBets.Corner)],
        [0.471923828125, -0.1044921875, 0.9449996948242188, ["19", "20", "22", "23"], (rouletteBets.Corner)],
        [0.4698486328125, 0.060028076171875, 0.9449996948242188, ["20", "21", "23", "24"], (rouletteBets.Corner)],
        [0.5531005859375, -0.106170654296875, 0.9449996948242188, ["22", "23", "25", "26"], (rouletteBets.Corner)],
        [0.5546875, 0.059417724609375, 0.9449996948242188, ["23", "24", "26", "27"], (rouletteBets.Corner)],
        [0.633544921875, -0.101531982421875, 0.9449996948242188, ["25", "26", "28", "29"], (rouletteBets.Corner)],
        [0.6337890625, 0.0579833984375, 0.9449996948242188, ["26", "27", "29", "30"], (rouletteBets.Corner)],
        [0.7156982421875, -0.106292724609375, 0.9449996948242188, ["28", "29", "31", "32"], (rouletteBets.Corner)],
        [0.7158203125, 0.0604248046875, 0.9449996948242188, ["29", "30", "32", "33"], (rouletteBets.Corner)],
        [0.7947998046875, -0.108642578125, 0.9449996948242188, ["31", "32", "34", "35"], (rouletteBets.Corner)],
        [0.7952880859375, 0.059051513671875, 0.9449996948242188, ["32", "33", "35", "36"], (rouletteBets.Corner)],
        [-0.099609375, -0.2711181640625, 0.9449996948242188, ["0", "00", "1", "2", "3"], (rouletteBets.Basket)],
        [-0.0147705078125, -0.27154541015625, 0.9449996948242188, ["1", "2", "3", "4", "5", "6"], (rouletteBets.SixLine)],
        [0.064697265625, -0.270263671875, 0.9449996948242188, ["4", "5", "6", "7", "8", "9"], (rouletteBets.SixLine)],
        [0.144775390625, -0.271209716796875, 0.9449996948242188, ["7", "8", "9", "10", "11", "12"], (rouletteBets.SixLine)],
        [0.226806640625, -0.27142333984375, 0.9449996948242188, ["10", "11", "12", "13", "14", "15"], (rouletteBets.SixLine)],
        [0.306396484375, -0.27142333984375, 0.9449996948242188, ["13", "14", "15", "16", "17", "18"], (rouletteBets.SixLine)],
        [0.3895263671875, -0.27099609375, 0.9449996948242188, ["16", "17", "18", "19", "20", "21"], (rouletteBets.SixLine)],
        [0.468017578125, -0.275238037109375, 0.9449996948242188, ["19", "20", "21", "22", "23", "24"], (rouletteBets.SixLine)],
        [0.5509033203125, -0.2738037109375, 0.9449996948242188, ["22", "23", "24", "25", "26", "27"], (rouletteBets.SixLine)],
        [0.6336669921875, -0.27386474609375, 0.9449996948242188, ["25", "26", "27", "28", "29", "30"], (rouletteBets.SixLine)],
        [0.7144775390625, -0.272186279296875, 0.9449996948242188, ["28", "29", "30", "31", "32", "33"], (rouletteBets.SixLine)],
        [0.7935791015625, -0.272918701171875, 0.9449996948242188, ["31", "32", "33", "34", "35", "36"], (rouletteBets.SixLine)],
        [0.0643310546875, -0.304718017578125, 0.9449996948242188, ["1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12"], (rouletteBets.FirstColumn)],
        [0.392822265625, -0.304779052734375, 0.9449996948242188, ["13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24"], (rouletteBets.SecondColumn)],
        [0.712158203125, -0.30303955078125, 0.9449996948242188, ["25", "26", "27", "28", "29", "30", "31", "32", "33", "34", "35", "36"], (rouletteBets.ThirdColumn)],
        [0.9222412109375, -0.185882568359375, 0.9449996948242188, ["1", "4", "7", "10", "13", "16", "19", "22", "25", "28", "31", "34"], (rouletteBets.FirstDozen)],
        [0.9229736328125, -0.0181884765625, 0.9449996948242188, ["2", "5", "8", "11", "14", "17", "20", "23", "26", "29", "32", "35"], (rouletteBets.SecondDozen)],
        [0.9248046875, 0.14849853515625, 0.9449996948242188, ["3", "6", "9", "12", "15", "18", "21", "24", "27", "30", "33", "36"], (rouletteBets.ThirdDozen)],
        [-0.011474609375, -0.378875732421875, 0.9449996948242188, ["1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18"], (rouletteBets.OneToEigthteen)],
        [0.142822265625, -0.375732421875, 0.9449996948242188, ["2", "4", "6", "8", "10", "12", "14", "16", "18", "20", "22", "24", "26", "28", "30", "32", "34", "36"], (rouletteBets.Even)],
        [0.308349609375, -0.37542724609375, 0.9449996948242188, ["1", "3", "5", "7", "9", "12", "14", "16", "18", "19", "21", "23", "25", "27", "30", "32", "34", "36"], (rouletteBets.Red)],
        [0.4713134765625, -0.376861572265625, 0.9449996948242188, ["2", "4", "6", "8", "10", "11", "13", "15", "17", "20", "22", "24", "26", "28", "29", "31", "33", "35"], (rouletteBets.Black)],
        [0.6341552734375, -0.376495361328125, 0.9449996948242188, ["1", "3", "5", "7", "9", "11", "13", "15", "17", "19", "21", "23", "25", "27", "29", "31", "33", "35"], (rouletteBets.Odd)],
        [0.7926025390625, -0.382232666015625, 0.9449996948242188, ["19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32", "33", "34", "35", "36"], (rouletteBets.NineteenToThirtySix)]
];

function leaveTable(){
    cleanUpEverything();
    // Inform the server
    mp.events.callRemote("OnPlayerLeaveRouletteTable::Server");
}

function handleLeftClick() {
    if (currentHighlightedField != null && !isSpinning && canBet) {
        if(currentBettingIndices.includes(currentHighlightedField)){
            mp.gui.chat.push("You've already placed a bet on this field. Please remove the previous bet first!");
            return;
        }
        // Create and place a chip at the bet the player made
        const currentBettingPosition = mp.game.object.getObjectOffsetFromCoords(table.position.x, table.position.y, table.position.z, table.getHeading(), 
        tableChipsOffsets[currentHighlightedField][0], tableChipsOffsets[currentHighlightedField][1], tableChipsOffsets[currentHighlightedField][2]);
        let chipOnTable = mp.objects.new(mp.game.joaat("vw_prop_chip_100dollar_x1"), currentBettingPosition);
        chipOnTable.setCollision(false, false);
        chipsOnTable[currentHighlightedField] = chipOnTable;

        mp.events.callRemote("OnPlayerBetRoulette::Server", JSON.stringify(tableChipsOffsets[currentHighlightedField][3]), tableChipsOffsets[currentHighlightedField][4], currentBettingValue);
        currentBettingIndices.push(currentHighlightedField);
    }
}

function handleRightClick() {
    if (currentHighlightedField != null && !isSpinning) {
        if(currentBettingIndices.includes(currentHighlightedField)){
            // Remove the index from the betting list
            const index = currentBettingIndices.indexOf(currentHighlightedField);
            if (index > -1) {
                currentBettingIndices.splice(index, 1);
            }

            // Delete the chip object
            if(currentHighlightedField in chipsOnTable){
                // Find the object, delete it and then find the index of the object and remove from chip objs list
                let chipObj = chipsOnTable[currentHighlightedField];
                if(chipObj !== null)
                    chipObj.destroy();
                delete chipsOnTable[currentHighlightedField];
            }

            mp.events.callRemote("OnPlayerRemoveBetRoulette::Server", JSON.stringify(tableChipsOffsets[currentHighlightedField][3]));
        }
    }
}

function handleMouseMovement() {
    mp.game.controls.disableAllControlActions(2);

    let x = (mp.game.controls.getDisabledControlNormal(7, 1) * mouseSensitivity);
    let y = (mp.game.controls.getDisabledControlNormal(7, 2) * mouseSensitivity);

    let currentRot = rouletteCamera.getRot(2);
    currentRot = new mp.Vector3(currentRot.x - y, 0, currentRot.z - x);
    // Clamp the rotation on the x-axis between -80 and 80 to avoid odd behavior.
    if (currentRot.x < -80)
        currentRot.x = -80;
    else if (currentRot.x > 80)
        currentRot.x = 80;

    rouletteCamera.setRot(currentRot.x, currentRot.y, currentRot.z, 2);
}

function getCameraHitObject() {
    let position = rouletteCamera.getCoord();
    let direction = rouletteCamera.getDirection();
    let farAway = new mp.Vector3((direction.x * 3) + position.x, (direction.y * 3) + position.y, (direction.z * 3) + position.z);

    let hitData = mp.raycasting.testPointToPoint(position, farAway);
    if (hitData != undefined) {
        return hitData;
    }
    return null;
}

function getClosestTableField(rayHitPos) {
    // Loop through the table chip offsets
    // use 0.025 as default to ensure the ray cast is close so we don't just create the objects no matter what
    let closestTableFieldIndex = null;
    let dist = null;
    let prevDistance = 0.025;

    for (var i = 0; i < tableChipsOffsets.length; i++) {
        // FieldPos is the world coords of the field we're iterating
        let fieldPos = mp.game.object.getObjectOffsetFromCoords(table.position.x, table.position.y, table.position.z, table.getHeading(),
            tableChipsOffsets[i][0], tableChipsOffsets[i][1], tableChipsOffsets[i][2]);
        // find the distance from the camera ray cast his position on the table and the table field position
        dist = mp.game.gameplay.getDistanceBetweenCoords(rayHitPos.x, rayHitPos.y, rayHitPos.z, fieldPos.x, fieldPos.y, fieldPos.z, false);

        // Compare the distance found to the previous
        if (dist <= prevDistance) {
            closestTableFieldIndex = i;
            prevDistance = dist;
        }
    }

    return closestTableFieldIndex;
}

function createTableFieldObjects(closestTableFieldIndex) {
    if (closestTableFieldIndex === null)
        return;

    // If we hit something new
    if (closestTableFieldIndex != currentHighlightedField) {
        // update the old field to point to the new we're aiming at
        currentHighlightedField = closestTableFieldIndex;

        // remove the old field objects if any exists
        clearTableMarkers();

        if (closestTableFieldIndex != null) {
            // Loop the list of belonging fields, e.g [1,3,5,7,9 ..., n] for odd numbers
            for (var i = 0; i < tableChipsOffsets[closestTableFieldIndex][3].length; i++) {
                // get the key for this field
                let key = tableChipsOffsets[closestTableFieldIndex][3][i];
                if (key == "00" || key == "0") {
                    let newCardPos = mp.game.object.getObjectOffsetFromCoords(table.position.x, table.position.y, table.position.z, table.getHeading(), tableMarkersOffsets[key][0], tableMarkersOffsets[key][1], tableMarkersOffsets[key][2]);

                    let obj = mp.objects.new(269022546, new mp.Vector3(newCardPos.x, newCardPos.y, newCardPos.z), { rotation: new mp.Vector3(0, 0, table.getHeading()) });
                    obj.setCollision(false, false);
                    tableMarkers.push(obj);
                }
                else {
                    let newCardPos = mp.game.object.getObjectOffsetFromCoords(table.position.x, table.position.y, table.position.z, table.getHeading(), tableMarkersOffsets[key][0], tableMarkersOffsets[key][1], tableMarkersOffsets[key][2]);

                    let obj = mp.objects.new(3267450776, new mp.Vector3(newCardPos.x, newCardPos.y, newCardPos.z), { rotation: new mp.Vector3(0, 0, table.getHeading()) });
                    obj.setCollision(false, false);
                    tableMarkers.push(obj);
                }
            }
        }
    }
}

function destroyAllChipsOnTable(){
    if(chipsOnTable === null || chipsOnTable === undefined)
        return;

    for (let key in chipsOnTable) {
        let chipObj = chipsOnTable[key];
        if(chipObj !== null)
            chipObj.destroy();
    }
    chipsOnTable = {};
}

function clearTableMarkers() {
    for (var i = 0; i < tableMarkers.length; i++) {
        tableMarkers[i].destroy();
    }
    tableMarkers = [];
}

function doesRouletteExist() {
    return (table !== undefined && table !== null && ball !== undefined && ball !== null && rouletteCamera !== undefined && rouletteCamera !== null);
}

function resetWheelValues() {
    lastSpinTime = 0;
    currentSpins = 0;
    isSpinning = true;
    canBet = false;
}

function destroyRouletteCamera(){
    if(!rouletteCamera)
        return;
    mp.game.cam.renderScriptCams(false, false, 0, true, false);
    rouletteCamera.destroy();
    rouletteCamera = null;
}

function cleanUpEverything(){
    table = null;
    ball = null;
    clearTableMarkers();
    destroyAllChipsOnTable();
    destroyRouletteCamera();
    currentHighlightedField = null;
    if(chipObject !== null)
        chipObject.destroy();
    chipObject = null;
    currentBettingValue = bettingIncreaseSize;
    currentBettingIndices = [];
    if(timerHandle !== null){
        clearInterval(timerHandle);
        timerHandle = null;
    }
    horizontalInstructionList.toggleHud(false);
    timerBarPool.clear();
}