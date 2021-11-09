import os
from flask import Flask, jsonify, session, abort, send_file
from game_logic import Game
import uuid


app = Flask(__name__)
app.secret_key = os.urandom(16)

next_game_id = 0
game_list = []
player_list = {} # Dict with player_name : player_id

def GetJSON(mode, game_id, player_id=None):
    for game in game_list:
        if game.id == game_id:
            if mode == "client":#returns JSON file for client
                return jsonify(field_of_view=game.GetFieldOfView(player_id),
                               items=game.GetItemDict(player_id),
                               round=game.round)
            elif mode == "spec":#returns JSON file for spectator
                return jsonify(id=100, 
                               field=game.matrix, 
                               state=game.state, 
                               round=game.round,
                               player_list = game.GetPlayerListForJSON())


### JSON ENDPOINTS ###

@app.route('/')
def root():
    return send_file('..\\Static\\junglestate.html')

@app.route('/joinGame/<mode>/<player_name>')
def joinGame(mode, player_name):
    if not player_name in player_list.keys():
        print(f"Mode: {mode}, Name: {player_name}")
        newId = uuid.uuid4()
        session['playerId'] = newId
        session['mode'] = mode
        player_list.update({player_name:newId})

        global next_game_id

        if len(game_list) == 0:
            game_list.append(Game(next_game_id))
            game_list[next_game_id].join(player_name)
            session['gameId'] = 0
            next_game_id += 1
        else:
            game_list[next_game_id].join(player_name)
            session['gameId'] = next_game_id

        return send_file('..\\Static\\junglestate.html')

    else:
        abort(409) # Player name already in use

@app.route('/view')
def view():
    playerId = session.get('playerId')
    if playerId in player_list.values():
        gameId = session.get('gameId')
        for game in game_list:
            if game.id == gameId:
                return GetJSON(session.get('mode'), gameId)

        print("ERROR: GAME NOT AVAILABLE")
        abort(410) # Game not available

    else:
        abort(403) # Invalid player id

# Input
@app.route('/action/<command>/<int:direction>')
def action(command, direction):
    playerId = session.get('playerId')
    if playerId in player_list.values():
        return jsonify(msg="aha")

    else:
        abort(403)


# Reset - only temporary
@app.route('/reset')
def reset():
    global next_game_id
    global player_list
    global game_list

    next_game_id = 0
    game_list = []
    player_list = {}

if __name__ == '__main__':
    app.run(port=5500)