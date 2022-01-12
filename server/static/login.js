async function login(join_mode, game_id) {
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
    if ((!name) && player_mode == 'client') {
        alert("Invalid Login Data");
        return;
    }

    if (player_mode == 'spec') {
        name = 'Spectator';
    }

    // Set options
    const options = {
        method: 'POST',
        body: JSON.stringify({"mode": join_mode, "password": password, "player_name": name, "player_mode": player_mode, "game_id": game_id}),
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

    json.games.forEach((item, index) => {
        console.log(`${index} : ${item}`);

        var div = document.createElement('div'); 
        div.classList.add('server');

        var img = document.createElement('img');
        img.classList.add('lockimage');
        img.style.height = '1.4vw';
        img.style.width = '1.4vw';
        if(item.secured == true)
            img.src = 'static//sprites//padlock.png';
        else
            img.src = 'static//sprites//open-padlock.png';
        div.appendChild(img);

        var text = document.createElement('p');
        text.classList.add('serverinfo');
        text.textContent = "Server: " + index + " | Spieler Online: " + item.players;
        div.appendChild(text);
    
        var join_button = document.createElement('button');
        join_button.classList.add('joinbutton');
        join_button.textContent = 'Join';
        join_button.addEventListener("click", () => {
            login('joinExisting', item.id);
        });

        div.appendChild(join_button);

        parentElement.appendChild(div);
    });
    
}, 1000);