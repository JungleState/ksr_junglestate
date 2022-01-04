async function login(join_mode) {
    let name = document.getElementById('name').value;
    let mode = document.getElementById('mode').checked;
    let password = document.getElementById('password').value;
    let player_mode;

    let options = {
        method: 'POST',
        body: JSON.stringify({"name":"Matt"}),
        headers: {
            'Content-Type': 'application/json'
        }
    }

    if (mode) {
        player_mode = 'spec';
    }
    else {
        player_mode = 'client';
    }

    if (!name) {
        return;
    }

    // Check chosen option
    if (join_mode == 'newGame') {

    }
    else if (join_mode == 'joinExisting') {

    }


    // Login
    let response = await fetch(`/joinGame`, options);
    let json = await response.json();

    if (json.ok) {
        // window.location.replace('/');
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