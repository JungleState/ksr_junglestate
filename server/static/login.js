async function login(join_mode) {
    let name = document.getElementById('name').value;
    let mode = document.getElementById('mode').checked;
    let password = document.getElementById('password').value;
    let player_mode;

    // Set mode
    if (mode) {
        player_mode = 'spec';
    }
    else {
        player_mode = 'client';
    }

    if (!name || (mode == 'spec' && join_mode == 'newGame')) {
        return;
    }

    // Set options
    let options = {
        method: 'POST',
        body: JSON.stringify({"mode": join_mode, "password": password, "player_name": name, "player_mode": player_mode}),
        headers: {
            'Content-Type': 'application/json'
        }
    }


    // Login
    let response = await fetch('/joinGame', options);
    let json = await response.json();

    if (json.ok) {
        window.location.replace('/');
    }
    else {
        alert(json.msg);
    }

    document.getElementById('name').value = "";
    document.getElementById('mode').checked = false;
    document.getElementById('password').value = "";
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
    div.textContent = "Server 1 | Spieler Online 1";
    var img = document.createElement('img');
    img.style.height = '1vw';
    img.style.width = '1vw';
    if(true == false)
        img.src = 'static//sprites//padlock.png';
    else
        img.src = 'static//sprites//open-padlock.png';

    var join_button = document.createElement('button');
    join_button.textContent = 'JOIN';

    div.appendChild(img);
    div.appendChild(join_button);
    parentElement.appendChild(div);

    json.games.forEach((item, index) => {
        console.log(`${index} : ${item}`);
        var div = document.createElement('div'); 
        div.textContent = "Server " + index + " | Spieler Online " + item.players;
        var img = document.createElement('img');
        img.src = 'static//sprites//padlock.png';
        div.appendChild(img);
        parentElement.appendChild(div);
    });
    
}, 1000);