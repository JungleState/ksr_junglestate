import os
from flask import Flask, jsonify, session, abort, render_template, redirect, url_for
from game_logic import Game
import uuid

NAME_LENGTH_MAX = 30

app = Flask(__name__, template_folder='templates')
app.logger.setLevel("DEBUG")
app.secret_key = os.urandom(16)

app.config.update(
    TEMPLATES_AUTO_RELOAD = True
)

next_game_id = 0
game_list = []
player_list = {} # Dict with playerID : playerName

FIELD = (30, 20)

def newGame(playerId, mode):
    if len(game_list) == 0:
        newId = uuid.uuid4()
        game = Game(newId, FIELD)
        if mode == 'client':
            game.join(player_list.get(playerId), playerId)
        game_list.append(game)
        return game.id
    else:
        if mode == 'client':
            game_list[-1].join(player_list.get(playerId), playerId)
        return game_list[-1].id

def GetJSON(mode, game_id, player_id):
    for game in game_list:
        if game.id == game_id:
            # app.logger.info(f"Found game {game_id} for player {player_id}")
            if mode == "client":#returns JSON file for client
                return {"field":game.GetFieldOfView(player_id),
                               "coconuts":game.GetPlayerVar(player_id, "CC"),
                               "lives":game.GetPlayerVar(player_id, "lives"),
                               "points":game.GetPlayerVar(player_id, "P"),
                               "round":game.round,
                               "mode":mode,
                               "name":player_list.get(player_id),
                               "name_list":game.GetPlayers()}
            elif mode == "spec":#returns JSON file for spectator
                return {"id":player_id, 
                               "field":game.SerializeMatrix(), 
                               "state":game.state, 
                               "round":game.round,
                               "player_list":game.GetPlayerListForJSON(),
                               "mode":mode,
                               "name_list":game.GetPlayers()}

def isLoggedIn():
    playerId=session.get('playerId')
    return playerId in player_list.keys()

def checkLogInData(name, mode):
    err = None

    # Check Name
    if len(name) == name.count(' ') or len(name) == 0:
        err = 'Invalid Name'
    elif len(name) > NAME_LENGTH_MAX:
        err = 'Too Many Characters'
    elif name in player_list.values() and mode == 'client':
        err = 'Name Already In Use'
    elif session.get('playerId'):
        err = 'Already Logged In'

    # Check Mode
    if mode != 'client' and mode != 'spec':
        err = 'Invalid Mode'

    return err
    
def kickPlayer():
    app.logger.debug(f"Kicked {player_list.get(session.get('playerId'))}")
    game_id = session.get('gameId')
    for game in game_list:
        if game.id == game_id:
            game.kickPlayer(player_list.get(session.get('playerId')))
    del player_list[session.get('playerId')]

### JSON ENDPOINTS ###

@app.route('/', methods=['GET'])
def root():
    if not isLoggedIn():
        return redirect(url_for('login'))
    else:
        dimension = None
        mode = session.get('mode')
        if mode == 'client':
            dimension = (5, 5)
        elif mode == 'spec':
            dimension = FIELD
        return render_template('view.html', dimension_x=dimension[0], dimension_y=dimension[1], mode=mode)

@app.route('/login', methods=['GET'])
def login():
    return render_template('login.html') 

@app.route('/joinGame/<string:mode>/<string:player_name>', methods=['POST'])
def joinGame(mode, player_name):

    err = checkLogInData(player_name, mode)

    if err:
        # Invalid name or mode
        app.logger.info(err)
        return jsonify(ok=False, msg=err)

    # Login data valid
    newId = str(uuid.uuid4())

    if mode == 'client':
        player_list.update({newId:player_name})
    elif mode == 'spec':
        player_list.update({newId:newId})

    gameId = newGame(newId, mode)

    session['playerId'] = newId
    session['mode'] = mode
    session['gameId'] = gameId

    return jsonify(ok=True)

# View - Server knows if the request comes from a spectator or a player
@app.route('/view', methods=['GET'])
def view():
    if isLoggedIn():
        playerId = session.get('playerId')
        gameId = session.get('gameId')
        for game in game_list:
            if game.id == gameId:
                response = GetJSON(session.get('mode'), gameId, playerId)
                return jsonify(response)

        app.logger.info("View error: game not available")
        abort(410) # Game not available

    else:
        app.logger.info("View error: invalid player id")
        abort(403) # Invalid player id

# Input
@app.route('/action/<moveType>/<direction>', methods=['POST'])
def action(moveType, direction):
    if isLoggedIn():
        if session.get('mode') == 'client':
            playerId = session.get('playerId')
            for game in game_list:
                if game.id == session.get('gameId'):
                    game.addMove(playerId, int(moveType), int(direction))

            return jsonify(ok=True)
        
        else:
            app.logger.info("Action error: spectator is not allowed to move")
            return jsonify(ok=False)

    else:
        app.logger.info("Action error: invalid player id")
        abort(403)


if __name__ == '__main__':
    app.run(port=5500)