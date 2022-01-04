async function login(join_mode) {
    let name = document.getElementById('name').value;
    let mode = document.getElementById('mode').checked;
    let player_mode;

    // Set mode
    if (mode) {
        player_mode = 'spec';
    }
    else {
        player_mode = 'client';
    }

    if (!name) {
        return;
    }


    let options = {
        method: 'POST',
        body: JSON.stringify({"game_mode": "","player_name": name, "player_mode": player_mode}),
        headers: {
            'Content-Type': 'application/json'
        }
    }

    // Login
    let response = await fetch(`/joinGame`, options);
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
}, 1000);