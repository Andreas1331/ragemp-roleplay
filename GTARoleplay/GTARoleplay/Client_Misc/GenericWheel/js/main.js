let allWheelsData = []; // is a list of objects sent from the server, containing the data for each wheel
let activeWheel = null; // is a Raphael Wheelnav object

const slicePathAttr = { fill: 'black', stroke: 'white', 'stroke-width': 1, 'opacity': 0.5, cursor: 'pointer' };
const sliceHoverAttr = { fill: 'black', stroke: 'white', 'stroke-width': 1, 'opacity': 0.4, cursor: 'pointer' };
const sliceSelectedAttr = { fill: 'black', stroke: 'white', 'stroke-width': 1, 'opacity': 0.4, cursor: 'pointer' };

const titleAttr = { fill: '#ffffff', stroke: 'black', 'stroke-width': 0.2, font: 'Helvetica', 'font-size': 32, 'font-weight': 'bold', cursor: 'pointer' };
const titleHoverAttr = { fill: '#ffffff', stroke: 'black', 'stroke-width': 0.2, font: 'Helvetica', 'font-size': 32, 'font-weight': 'bold', cursor: 'pointer' };
const titleSelectedAttr = { fill: '#ffffff', stroke: 'black', 'stroke-width': 0.2, font: 'Helvetica', 'font-size': 32, 'font-weight': 'bold', cursor: 'pointer' };

const spreaderPathAttr = { fill: '#000000', stroke: '#000000', 'opacity': 0 };
const spreaderTitleInOutAttr = { fill: '#ffffff', stroke: '#000000', 'stroke-width': 1, font: 'Helvetica', 'font-size': 26, 'opacity': 1 };

let iconsDict = null;

function setupIcons(){
    iconsDict = {"icon.car": icon.car, "icon.lock": icon.lock, 
                "icon.key": icon.key, "icon.power": icon.power,
                "icon.inventory": "imgsrc:resources/icons/inventory.svg",
                "icon.arrowleft": icon.arrowleft2, "icon.list": icon.list,
                "icon.play": icon.play,
                "icon.wrench": icon.wrench, "icon.cross": icon.cross, "icon.palm": icon.palm};
}

function initWheel(wheelData, key){
    allWheelsData = JSON.parse(wheelData);
    setupIcons();
    createWheel(allWheelsData[0]);    
    
    window.addEventListener("keyup", function onEvent(event) {
        // Destroy the wheel when the key is released
        if (event.key === key) {
            destroyEverything();
        }
    });
}

function createWheel(wheelData){
    activeWheel = new wheelnav('piemenu', null, 600, 600);

    // Set the selected item to null on initialization
    activeWheel.selectedNavItemIndex = null;

    // Setup the wheel
    activeWheel.slicePathFunction = slicePath().DonutSlice;
    activeWheel.slicePathCustom = slicePath().DonutSliceCustomization();

    // Disable the bouncy effect when a slice is picked
    activeWheel.sliceSelectedPathCustom = activeWheel.slicePathCustom;
    // Disable the bouncy effect when the wheel is initialized
    activeWheel.sliceInitPathCustom = activeWheel.slicePathCustom;
    activeWheel.sliceHoverPathCustom = activeWheel.slicePathCustom;

    // slicePathAttr is whenever you see the slices without interacting with them
    activeWheel.slicePathAttr = slicePathAttr;
    // sliceHoverAttr is whenever the mouse hovers above a slice
    activeWheel.sliceHoverAttr = sliceHoverAttr;
    // sliceSelectedAttr is whenever you click on a slice
    activeWheel.sliceSelectedAttr = sliceSelectedAttr;

    // Disable the rotation of the wheel when a slice is picked
    activeWheel.clickModeRotate = false;

    // Font options on the slices
    activeWheel.titleAttr = titleAttr;
    activeWheel.titleHoverAttr = titleHoverAttr;
    activeWheel.titleSelectedAttr = titleSelectedAttr;

    activeWheel.spreaderEnable = true;
    activeWheel.spreaderRadius = 45;
    activeWheel.spreaderInTitle = "";//icon.plus;
    activeWheel.spreaderOutTitle = "";//icon.cross;
    activeWheel.spreaderPathInAttr = spreaderPathAttr;
    activeWheel.spreaderPathOutAttr = spreaderPathAttr;
    activeWheel.spreaderTitleInAttr = spreaderTitleInOutAttr;
    activeWheel.spreaderTitleOutAttr = spreaderTitleInOutAttr;

    // Push the slices away from the middle, thus creating some space between each other
    activeWheel.animatetime = 150;
    activeWheel.animateeffect = 'linear';
    activeWheel.wheelRadius = activeWheel.wheelRadius * 0.9;

    activeWheel.titleWidth = 64;
    activeWheel.titleHeight = 64;

    let sliceTexts = [];
    for(let n = 0; n < wheelData.Slices.length; n++){
        let tmpTxt = getIcon(wheelData.Slices[n].Text);
        sliceTexts.push(tmpTxt);
    }

    // Create the slices with the provided text or icons
    activeWheel.createWheel(sliceTexts);

    // Set the functions for each slice
    for(let k = 0; k < activeWheel.navItems.length; k++){
        //TODO: OnMouseOver, change the spreaderTitle to some text from the slice
        // Enable the click functionality for both the slice, and its title.
        activeWheel.navItems[k].navigateFunction = () => { onClickSlice(); };

        let onClickSlice = function () {
            // Determine if this slice opens a submenu
            if(wheelData.Slices[k].hasOwnProperty("IsSubMenuBtn") && wheelData.Slices[k].IsSubMenuBtn){
                // Delete the currently active wheel and call for the creation of the linked wheel
                activeWheel.raphael.remove();
                createWheel(allWheelsData[wheelData.Slices[k].LinkedMenuID]);
                return;
            } else{
                // If this is not a submenu, tell the server which slice was clicked
                let wheelMenuID = wheelData.ID;
                let wheelSliceIndex = k;
                mp.trigger("OnWheelSliceClicked::Client", wheelMenuID, wheelSliceIndex);

                // If the slice closes the menu, invoke deletion
                if(wheelData.Slices[k].WillCloseMenu){
                    destroyEverything();
                }
            }
        };
    }

    $('#wheel-title').text(wheelData.Title);
}

function destroyEverything(){
    if(activeWheel !== null){
        activeWheel.raphael.remove();
    }
    mp.trigger("DestroyWheel::Client");
}

function getIcon(txt){
    if(txt in iconsDict)
        return iconsDict[txt];
    else return txt;
}