document.addEventListener("keydown", function(event) {
    if (event.key === "Enter") {
        login();
    }
});

async function login(mode) {
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