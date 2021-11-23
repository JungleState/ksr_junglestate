document.addEventListener("keydown", function(event) {
    if (event.key === "Enter") {
        login();
    }
});

async function login() {

    let name = document.getElementById('inp').value;
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

    let response = await fetch(`/joinGame/${modeName}/${name}`);
    let json = await response.json();

    if (json.ok) {
        alert("OK");
    }
    else {
        alert("NO");
    }

    document.getElementById('inp').value = "";
    document.getElementById('mode').checked = false;
}