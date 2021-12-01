import os
from flask import Flask, jsonify, session, abort, render_template, redirect, url_for
from werkzeug.utils import redirect
from game_logic import Game
import uuid


app = Flask(__name__, template_folder='templates')
app.logger.setLevel("DEBUG")
app.secret_key = os.urandom(16)

next_game_id = 0
game_list = []
player_list = {} # Dict with playerID : playerName

FIELD = (20, 30)

def newGame(playerId):
    if len(game_list) == 0:
        newId = uuid.uuid4()
        game = Game(newId, FIELD)
        game.join(player_list.get(playerId), playerId)
        game_list.append(game)
        return game.id
    else:
        game_list[-1].join(player_list.get(playerId), playerId)
        return game_list[-1].id

def GetJSON(mode, game_id, player_id=None):
    for game in game_list:
        if game.id == game_id:
            app.logger.info(f"Found game {game_id} for player {player_id}")
            if mode == "client":#returns JSON file for client
                return {"field_of_view":game.GetFieldOfView(player_id),
                               "coconuts":game.GetPlayerVar(player_id, "CC"),
                               "lives":game.GetPlayerVar(player_id, "lives"),
                               "points":game.GetPlayerVar(player_id, "P"),
                               "round":game.round}
            elif mode == "spec":#returns JSON file for spectator
                return {"id":player_id, 
                               "field":game.SerializeMatrix(), 
                               "state":game.state, 
                               "round":game.round,
                               "player_list":game.GetPlayerListForJSON()}

def isLoggedIn():
    playerId=session.get('playerId')
    return playerId in player_list.keys()
    


### JSON ENDPOINTS ###

@app.route('/')
def root():
    app.logger.debug("ROOT")
    if not isLoggedIn():
        return redirect(url_for('login'))
    else:
        dimension = 10
        return render_template('view.html', dimension_x=dimension, dimension_y=100) #need something to set dimensions right (spec != client)
        
@app.route('/login')
def login():
    return render_template('login.html') 

@app.route('/joinGame/<string:mode>/<player_name>')
def joinGame(mode, player_name):
    if not player_name in player_list.values() and not session.get('playerId'):
        app.logger.info(f"NEW PLAYER: {player_name} (Mode: {mode})")
        newId = str(uuid.uuid4())
        player_list.update({newId:player_name})

        gameId = newGame(newId)

        session['playerId'] = newId
        session['mode'] = mode
        session['gameId'] = gameId

        return jsonify(ok=True)
    

    else:
        app.logger.info("PLAYER NAME ALREADY IN USE / PLAYER ALREADY LOGGED IN")
        return jsonify(ok=False)

# View - Server knows if the request comes from a spectator or a player
@app.route('/view')
def view():
    playerId = session.get('playerId')
    # Check if player is valid
    if playerId in player_list.keys():
        gameId = session.get('gameId')
        for game in game_list:
            if game.id == gameId:
                app.logger.debug(f"VIEW: RETURN JSON FOR '{player_list.get(playerId)}' (Mode: {session.get('mode')})")
                return jsonify(GetJSON(session.get('mode'), gameId, playerId))

        app.logger.info("ERROR: GAME NOT AVAILABLE")
        abort(410) # Game not available

    else:
        app.logger.info("INVALID PLAYER ID")
        abort(403) # Invalid player id

# Input
@app.route('/action/<int:moveType>/<int:direction>')
def action(moveType, direction):
    playerId = session.get('playerId')
    # Check if player is valid
    if playerId in player_list.keys():
        for game in game_list:
            if game.id == session.get('gameId'):
                game.addMove(playerId, moveType, direction)

        return jsonify(msg="move accepted")

    else:
        app.logger.info("INVALID PLAYER ID")
        abort(403)

### only temporary ##

@app.route('/uuid')
def getUuid():
    return jsonify(id=uuid.uuid4())

### ###


if __name__ == '__main__':
    app.run(port=5500)