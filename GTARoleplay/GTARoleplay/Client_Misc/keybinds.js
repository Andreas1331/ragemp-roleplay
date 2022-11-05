global.isChatOpened = false;
// 0x43 is the C key code
mp.keys.bind(0x43, true, c_KeyPressedDown);
// 0x45 is the E key code
mp.keys.bind(0x45, true, e_KeyPressedDown);
// 0x54 is the T key code
mp.keys.bind(0x54, true, t_KeyPressedDown);
// 0x0D is the ENTER key code
mp.keys.bind(0x0D, true, enter_KeyPressedDown);
// 0x1B is the ESC key code
mp.keys.bind(0x1B, true, esc_KeyPressedDown);
// 0x59 is the Y key code
mp.keys.bind(0x59, true, y_KeyPressedDown);
// 0x4C is the L key code
mp.keys.bind(0x4C, true, l_KeyPressedDown);


function c_KeyPressedDown(){
    if(!global.loggedIn || global.isChatOpened)
        return;

    mp.events.callRemote('CKeyPressed::Server');
}


function e_KeyPressedDown(){
    if(!global.loggedIn || global.isChatOpened || global.interactEntityType == null || global.interactEntityValue == null)
        return;

    mp.events.callRemote('EKeyPressed::Server', global.interactEntityType, global.interactEntityValue);
}

function y_KeyPressedDown(){
    if(!global.loggedIn || global.isChatOpened)
        return;

    mp.events.callRemote('YKeyPressed::Server');
}

function l_KeyPressedDown(){
    if(!global.loggedIn || global.isChatOpened)
        return;

    mp.events.callRemote('LKeyPressed::Server');
}

function t_KeyPressedDown(){
    if(!global.isChatOpened)
        global.isChatOpened = true;
}

function enter_KeyPressedDown(){
    if (global.isChatOpened) 
        global.isChatOpened = false;
}

function esc_KeyPressedDown(){
    if (global.isChatOpened) 
        global.isChatOpened = false;
}
