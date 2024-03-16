var outfitToPurchase = {
    // Clothes and accessories to purchase
    "clothesAndAccessories":{

    }
};
var maxTexturesForComponents = {};
// regular expression that gives all digitals in a str
const re = /\d+/;

function setupSliders(tmpData){
    var data = JSON.parse(tmpData);
    if(!data)
        return;

    for(let i = 0; i < data.length; i++){
        // Setup slider
        $("#"+data[i].id).val(data[i].startValue);
        $("#"+data[i].id).ionRangeSlider({
            type: "single",
            skin: "round",
            min: data[i].min,
            max: data[i].max - 1,
            step: 1,
            grid: false
        });
        // Whenever one of our sliders changes value, we will update the outfit to purchase
        $(document).on('input', '#'+data[i].id, function() {
            let val = $("#"+data[i].id).val();
            let textureVal = $('#'+data[i].id+"TextureBox").attr("index");
            if(textureVal === null || textureVal === undefined)
                textureVal = 0;

            outfitToPurchase['clothesAndAccessories'][String(data[i].componentSlot)] =
            {"Drawable": parseInt(val), "Texture": parseInt(textureVal)};
            mp.trigger("GetDrawableMaxTexture::Client", data[i].id, data[i].isAccessory, data[i].componentSlot, val);
            mp.trigger("SetPlayerClothes::Client", data[i].isAccessory, data[i].componentSlot, val, textureVal);
        });
        setupTextureInput(data[i].id, data[i].isAccessory, data[i].componentSlot);
        updateMaxTextures(data[i].id, data[i].componentSlot, data[i].initialMaxTexture, data[i].currentTexture, false);
    }

    // Setup rotation slider
    $("#rotateSlider").ionRangeSlider({
        type: "single",
        skin: "round",
        min: 0,
        max: 360,
        step: 1,
        grid: true
    });
    $(document).on('input', '#rotateSlider', function() {
        mp.trigger("SetPlayerRotation::Client", $(this).val());
    });
}

// Every time the player moves a slider, we will have to gather the max. texture available for this drawable.
function updateMaxTextures(componentID, componentSlot, maxTextures, currentTexture, updateOutfit = true) {
    maxTexturesForComponents[componentSlot] = maxTextures;
    currentTexture = parseInt(currentTexture);

    let textureBox = $('#'+componentID+"TextureBox");
    textureBox.val("Texture " + (currentTexture+1) + "/" + maxTextures); 
    textureBox.attr("index", currentTexture);
    
    if(updateOutfit)
        outfitToPurchase['clothesAndAccessories'][String(componentSlot)]["Texture"] = currentTexture;
}

function setTorsoElements(drawable, texture){
    outfitToPurchase['clothesAndAccessories'][3] = {"Drawable": parseInt(drawable), "Texture": parseInt(texture)};
}

function purchaseOutfit(){
    if(!outfitToPurchase)
        return;
    mp.trigger("PurchaseOutfit::Client", JSON.stringify(outfitToPurchase));
}

function closeWindow(){
    mp.trigger("DestroyClothingWindow::Client");
}

// For interaction with the sliders
function increaseSlider(slider){
    let current = Number($("#"+slider).val());
    let range = $("#"+slider).data("ionRangeSlider");
    range.update({
        from: current + 1
    });  
}

function decreaseSlider(slider){
    var current = Number($("#"+slider).val());
    let range = $("#"+slider).data("ionRangeSlider");
    range.update({
        from: current - 1
    });  
}

function setupTextureInput(componentID, isAccessory, componentSlot){
    // Setup the texture buttons for this component
    $(document).on('click', '#'+componentID+"TextureNext", function () {
        let maxTexture = maxTexturesForComponents[componentSlot];
        if(!maxTexture)
            return;
        let valueOfSlider = $("#"+componentID).val();
        let textureBox = $('#'+componentID+"TextureBox");
        let currentIndex = parseInt(textureBox.attr("index"));
        if(currentIndex === (maxTexture-1))
            return;
        // Increase the index and store it on the texture box for later  
        currentIndex += 1;
        textureBox.val("Texture " + (currentIndex+1) + "/" + maxTexture);
        textureBox.attr("index", currentIndex); 
        outfitToPurchase['clothesAndAccessories'][String(componentSlot)] = {"Drawable": parseInt(valueOfSlider), "Texture": parseInt(currentIndex)};
        mp.trigger("SetPlayerClothes::Client", isAccessory, componentSlot, valueOfSlider, currentIndex);
    });
    $(document).on('click', '#'+componentID+"TexturePrevious", function () {     
        let maxTexture = maxTexturesForComponents[componentSlot];
        if(!maxTexture)
            return;
        let valueOfSlider = $("#"+componentID).val();
        let textureBox = $('#'+componentID+"TextureBox");
        let currentIndex = parseInt(textureBox.attr("index"));
        if(currentIndex === 0)
            return;
        // Increase the index and store it on the texture box for later  
        currentIndex -= 1;
        textureBox.val("Texture " + (currentIndex+1) + "/" + maxTexture); 
        textureBox.attr("index", currentIndex); 
        outfitToPurchase['clothesAndAccessories'][String(componentSlot)] = {"Drawable": parseInt(valueOfSlider), "Texture": parseInt(currentIndex)};
        mp.trigger("SetPlayerClothes::Client", isAccessory, componentSlot, valueOfSlider, currentIndex);
    });
}