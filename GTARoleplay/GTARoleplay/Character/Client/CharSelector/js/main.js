var characters = [];
var currentCharacterIndex = 0;

function onClickPlay(){
    // The player has no characters, and must first create one
    if(!characters)
        return;

    let selectedCharacter = characters[currentCharacterIndex];
    mp.trigger("OnCharacterSelected::Client", selectedCharacter);
}

function setupCharacterDropdown(charactersObj){
    if(!charactersObj)
        return;

    characters = JSON.parse(charactersObj);
    var $dropdown = $("#characterdropdown");
    for(let i = 0; i < characters.length; i++){
        if(i === 0)
            $dropdown.append($("<option selected/>").val(characters[i]).text(characters[i]));
        else
            $dropdown.append($("<option />").val(characters[i]).text(characters[i]));
    }
}

function nextCharacter(){
    if(currentCharacterIndex === (characters.length - 1))
        return;
    currentCharacterIndex += 1;
    let $dropdown = $("#characterdropdown");
    $dropdown.val(characters[currentCharacterIndex]);
    console.log(currentCharacterIndex);
}

function previousCharacter(){
    if(currentCharacterIndex === 0)
        return;
    currentCharacterIndex -= 1;
    let $dropdown = $("#characterdropdown");
    $dropdown.val(characters[currentCharacterIndex]);
    console.log(currentCharacterIndex);
}

