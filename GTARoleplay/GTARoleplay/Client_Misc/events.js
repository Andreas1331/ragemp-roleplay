global.loggedIn = false;
global.version = "";

mp.events.add('SetFreezeState::Client', (state) => {
	mp.players.local.freezePosition(state);
});

mp.events.add('PlayerLoggedIn::Client', () => {
	global.loggedIn = true;
});

mp.events.add('SetVersion::Client', (version) => {
	global.version = version;
});

mp.events.add('ChangeHandcuffsState::Client', (state) => {
	// SET_ENABLE_HANDCUFFS
	mp.game.invoke('0xDF1AF8B5D56542FA', mp.players.local.handle, state);
});

mp.events.add('SetCurrentPedWeapon::Client', (weapon) => {
	// SET_CURRENT_PED_WEAPON
	mp.game.invoke('0xADF692B254977C0C', mp.players.local.handle, weapon, true);
});