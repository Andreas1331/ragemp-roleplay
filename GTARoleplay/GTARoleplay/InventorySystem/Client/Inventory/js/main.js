const itemIcons = {
  'Cocaine': 'powder',
  'Water bottle': 'water-bottle'
};

var currentWeight = 0;
var maxWeight = 0;
var isUsingItem = false;

function clearItemsContainer(){
  var itm_container = $("#item_container");
  if(itm_container !== null)
    itm_container.empty();

  currentWeight = 0;
  clearWeightText();

  $(".spinner-border").hide();
  $(".card-body").animate({
    opacity: 1,
  }, 300);

  isUsingItem = false;
}

function addItemsToContainer(maxWeightTmp, itemsObj){
  maxWeight = maxWeightTmp;
  // Grab a reference to the item container and append for each item
  var itm_container = $("#item_container");
  let itms = JSON.parse(itemsObj);

  clearWeightText();

  for(let i = 0; i < itms.length; i++){
    let img = getItemIconImg(itms[i].Name);

    itm_container.append(`<div class="col">
    <div class="card shadow rounded-3" style="width: 128px; height: 128px; background-color: rgba(255, 255, 255, 0.85);">
      <img src="resources/icons/${img}.svg" width="64" height="64" class="card-img-top pt-2">
      <div class="card-body p-2">
        <h6 class="card-title">${itms[i].Name}</h6>
        <img src="resources/icons/use.svg" width="32" height="32" class="use-img" onclick="onClickUseItem(${itms[i].Identifier});" role="button">
        <img src="resources/icons/drop.svg" width="32" height="32" class="drop-img p-1" onclick="onClickDropItem(${itms[i].Identifier});" role="button">
        <div class="badge bg-primary amount d-flex justify-content-center">${itms[i].Amount}</div>
      </div>
    </div>
  </div>`);
    updateWeightText(itms[i]);
  }
}

function getItemIconImg(itm){
  if(itm in itemIcons){
    return itemIcons[itm];
  }
}

function clearWeightText(){
  let weightObj = $("#weight");
  weightObj.empty();
  weightObj.append(`<img src="resources/icons/weight.svg" width="32" height="32" class="p-1">Weight 0/${maxWeight}kg`);
}

function updateWeightText(itm){
  let weightObj = $("#weight");
  let itmTotalWeight = itm.Amount * itm.WeightPerItem;
  currentWeight += itmTotalWeight;

  weightObj.empty();
  weightObj.append(`<img src="resources/icons/weight.svg" width="32" height="32" class="p-1">Weight ${(currentWeight/1000).toFixed(2)}/${(maxWeight/1000)}kg`);
}

function onClickUseItem(itmIdentifier){
  if(isUsingItem)
    return;

  // Start the spinning to indicate process
  $(".spinner-border").show();
  $(".card-body").animate({
      opacity: 0.5,
  }, 300);
  
  isUsingItem = true;
  mp.trigger("UseItemInInventory::Client", itmIdentifier);
}

function onClickDropItem(itmIdentifier){
  if(isUsingItem)
    return;

  // TODO: Add drop functionality
}

$(document).ready(function()
{
    $("body").keydown(function(event)
    {
        // If 'escape' is pressed
        if (event.which == 27) {
          destroyInventory();
        }
    });
});

function destroyInventory(){
  mp.trigger("DestroyInventory::Client");
}