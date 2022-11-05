/*
	Credits:
		- Kemperr
		- Captien
		- Kar
*/

const scaleform = require('gtaroleplay/InstructionalButtons/scaleform');
const validStyles = [-1, 1];

class instructionalButtons {
	constructor(style, bgColor) { // bgColor accepts HEX and RGBA
		this.state = false;
		this.style = null;
		this.hud = new scaleform('INSTRUCTIONAL_BUTTONS');
		this.render = null;
		this.buttons = [];
		this.backgroundColors = [0, 0, 0, 180];
		if(style) this.changeStyle(style);
		this.setBackgroundColor(bgColor);
		this.resetBar();
	}

	changeStyle(style) {
		if(!validStyles.includes(style)) return mp.gui.chat.push('!{red}[ERROR] !{white}Invalid style. Please use styles (-1 or 1).');
		if(this.style === style) return mp.gui.chat.push('!{red}[ERROR] !{white}You\'re already using that style.');
		this.style = style;
		if(this.isActive()) {
			this.hud.callFunction("DRAW_INSTRUCTIONAL_BUTTONS", this.style);
		}
	}

	setBackgroundColor(bgColor) {
		if(bgColor) {
			if(Array.isArray(bgColor)) {
				this.backgroundColors[0] = [bgColor[0], bgColor[1], bgColor[2], bgColor.length === 4 ? bgColor[3] : 180];
			} else if(bgColor.match(/[0-9A-Fa-f]{6}/)) { // #
				let color = hexToRGB(bgColor); // bgColor.replace('#', '')
				this.backgroundColors[0] = [color[0], color[1], color[2], 180];
			} else {
				mp.gui.chat.push('!{orange}[WARNING] !{white}Invalid color given. Make sure it suits as specified in resource\'s description');
			}
		}
		this.hud.callFunction("SET_BACKGROUND_COLOUR", this.backgroundColors[0], this.backgroundColors[1], this.backgroundColors[2], this.backgroundColors[3]);
		if(this.isActive()) {
			this.hud.callFunction("DRAW_INSTRUCTIONAL_BUTTONS", this.style);
		}
	}

	addButton(title, control_id) {
		let controlName = getControlName(control_id);
		if(controlName === 't_ERR') {
			controlName = '';
			mp.gui.chat.push('!{orange}[WARNING] !{white}Invalid control_id, make sure its between (0, 356).');
		}
		let slot = this.buttons.push({
			control: controlName ? controlName : "",
			title: title ? title : ""
		}) - 1;
		this.hud.callFunction("SET_DATA_SLOT", slot, controlName, title);
		if(this.isActive()) {
			this.hud.callFunction("DRAW_INSTRUCTIONAL_BUTTONS", this.style);
		}
	}

	hasControl(control) {
		control = getControlName(control);
		return (this.buttons.find(button => button.control === control)) ? true : false;
	}

	addButtons(buttons) {
		if(typeof buttons === 'object') {
			Object.keys(buttons).forEach(btn => {
				let
					title = btn,
					controlName = getControlName(buttons[btn])
				;
				let slot = this.buttons.push({
					title: title ? title : "",
					control: controlName ? controlName : ""
				}) - 1;
				this.hud.callFunction("SET_DATA_SLOT", slot, controlName, title);
			});
			if(this.isActive()) {
				this.hud.callFunction("DRAW_INSTRUCTIONAL_BUTTONS", this.style);
			}
		} else {
			return mp.gui.chat.push('!{red}[ERROR] !{white}Invalid arguement form, please use object form that is instructed on the resource\'s description.');
		}
	}

	changeButtonTitle(control, new_title) {
		control = getControlName(control);
		this.buttons.forEach( (button, slot) => {
			if(button.control === control) {
				button.title = new_title;
				this.hud.callFunction("SET_DATA_SLOT", slot, button.control, button.title);
			}
		});
		if(this.isActive()) {
			this.hud.callFunction("DRAW_INSTRUCTIONAL_BUTTONS", this.style);
		}
	}

	changeButtonControl(title, new_control) {
		this.buttons.forEach( (button, slot) => {
			if(button.title === title) {
				button.control = getControlName(new_control);
				this.hud.callFunction("SET_DATA_SLOT", slot, button.control, button.title);
			}
		});
		if(this.isActive()) {
			this.hud.callFunction("DRAW_INSTRUCTIONAL_BUTTONS", this.style);
		}
	}

	removeButton(btn) {
		switch (typeof btn) {
			case 'string': {
				this.buttons = this.buttons.filter( (button, slot) => {
					if(button.title === btn || button.control === getControlName(btn)) {
						this.hud.callFunction("SET_DATA_SLOT", slot, "", "");
					} else {
						return true;
					}
				});
				break;
			}
			case 'number': {
				this.buttons = this.buttons.filter( (button, slot) => {
					if(button.control === getControlName(btn)) {
						this.hud.callFunction("SET_DATA_SLOT", slot, "", "");
					} else {
						return true;
					}
				});
				break;
			}
		}
		if(this.isActive()) {
			this.hud.callFunction("DRAW_INSTRUCTIONAL_BUTTONS", this.style);
		}
	}

	removeButtons() {
		this.buttons = [];
		this.resetBar();
	}

	getButtonCount() {
		return this.buttons.length;
	}

	toggleHud(state) {
		if(state) {
			this.hud.callFunction("CLEAR_ALL");
			this.buttons.forEach( (button, slot) => {
				this.hud.callFunction("SET_DATA_SLOT", slot, button.control, button.title);
			});
			this.hud.callFunction("SET_BACKGROUND_COLOUR", this.backgroundColors[0], this.backgroundColors[1], this.backgroundColors[2], this.backgroundColors[3]);
			this.hud.callFunction("DRAW_INSTRUCTIONAL_BUTTONS", this.style);
			if(this.render === null) {
				this.render = new mp.Event('render', () => {
					this.hud.renderFullscreen();
				});
				this.state = true;
			} else {
				this.render.enable();
				this.state = true;
			}
		} else {
			if(this.render !== null) {
				this.render.destroy();
				this.state = false;
			}
			else return false;
		}
	}

	isActive () {
		return this.state;
	}

	resetBar () {
		this.hud.callFunction("CLEAR_ALL");
		this.hud.callFunction("TOGGLE_MOUSE_BUTTONS", 0);
		this.hud.callFunction("CREATE_CONTAINER");
		this.hud.callFunction("SET_CLEAR_SPACE", 100);
	}
}

function getControlName(control) {
	return (typeof control === 'number' && control >= 0 && control <= 356) ? mp.game.controls.getControlActionName(2, control, true) : (typeof control === 'string') ? ('t_' + control) : 't_ERR';
}

function hexToRGB(hex) {
	let bigint = parseInt(hex.replace(/[^0-9A-F]/gi, ''), 16);
	return [(bigint >> 16) & 255, (bigint >> 8) & 255, bigint & 255];
}

exports = instructionalButtons;