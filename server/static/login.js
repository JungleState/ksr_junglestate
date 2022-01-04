document.addEventListener("keydown", function(event) {
    if (event.key === "Enter") {
        login('');
    }
});

async function login(loginMode) {
    let name = document.getElementById('name').value;
    let mode = document.getElementById('mode').checked;

    let modeName;

    if (mode) {
        modeName = 'spec';
    }
    else {
        modeName = 'client';
    }

    if (!name) {
        return;
    }

    let response = await fetch(`/joinGame/${modeName}/${name}`, { method: 'POST' });
    let json = await response.json();

    if (json.ok) {
        window.location.replace('/')
    }
    else {
        alert(json.msg);
    }

    document.getElementById('name').value = "";
    document.getElementById('mode').checked = false;

}

function refresh(json){
    parentElement = document.getElementById('openGames');
    json.games.forEach((item, index) => {
        console.log(`${index} : ${item}`);
        var div = document.createElement('div'); 
        div.textContent = "Server " + index;  
        parentElement.appendChild(div)  
    });
}

setInterval(async () => {
    let response = await fetch('/getGames');
    let json = await response.json();
    console.log(json);

    parentElement = document.getElementById('openGames');

    while (parentElement.hasChildNodes()) {  
        parentElement.removeChild(parentElement.firstChild);
    } 
    
    var div = document.createElement('div'); 
    div.textContent = "Server:    " + json.games.id;  
    parentElement.appendChild(div)  
    //json.games.forEach(element => console.log(element));
}, 1000);