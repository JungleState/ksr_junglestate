import os
from flask import Flask, jsonify, session, abort, render_template, redirect, url_for
from game_logic import Game
import datetime
import uuid

NAME_LENGTH_MAX = 30

app = Flask(__name__, template_folder='templates')
app.logger.setLevel("DEBUG")
app.secret_key = os.urandom(16)

app.config.update(
    TEMPLATES_AUTO_RELOAD=True
)

user_list = []

class User:
    def __init__(self, name, mode):
        self.mode = mode
        self.uuid = str(uuid.uuid4())
        self.active = True
        self.name = self.set_name(name)
    
    def set_name(self, name):
        if self.mode == 'spec':
            return 'Spectator'
        else:
            return name

    @staticmethod
    def get_user_by_id(id):
        for user in user_list:
            if user.uuid == id:
                return user



next_game_id = 0
game_list = []

FIELD = (30, 20)


def newGame(user):
    if len(game_list) == 0:
        newId = uuid.uuid4()
        game = Game(newId, FIELD)
        if user.mode == 'client':
            game.join(user.name, user.uuid)
        game_list.append(game)
        return game.id
    else:
        if user.mode == 'client':
            game_list[-1].join(user.name, user.uuid)
        return game_list[-1].id


def GetJSON(mode, game_id, user):
    for game in game_list:
        if game.id == game_id:
            if mode == "client":#returns JSON file for client
                return {"field":game.GetFieldOfView(user.uuid),
                               "coconuts":game.GetPlayerVar(user.uuid, "CC"),
                               "lives":game.GetPlayerVar(user.uuid, "lives"),
                               "points":game.GetPlayerVar(user.uuid, "P"),
                               "round":game.round,
                               "mode":mode,
                               "name":user.name,
                               "name_list":game.GetPlayers()}
            elif mode == "spec":#returns JSON file for spectator
                return {"id":user.uuid, 
                               "field":game.SerializeMatrix(), 
                               "state":game.state, 
                               "round":game.round,
                               "scoreboard":game.Scoreboard("points", "decr"),
                               "mode":mode,
                               "name_list":game.GetPlayers()}

def isLoggedIn():
    playerId = session.get('playerId')
    return playerId in [user.uuid for user in user_list]


def checkLogInData(name, mode):
    err = None

    # Check Name
    if len(name) == name.count(' ') or len(name) == 0:
        err = 'Invalid Name'
    elif len(name) > NAME_LENGTH_MAX:
        err = 'Too Many Characters'
    elif name in [user.name for user in user_list] and mode == 'client':
        err = 'Name Already In Use'
    elif session.get('playerId'):
        err = 'Already Logged In'

    # Check Mode
    if mode != 'client' and mode != 'spec':
        err = 'Invalid Mode'

    return err


def kickPlayer():
    user = User.get_user_by_id(session.get('playerId'))
    app.logger.debug(f"Kicked {user.name}")
    game_id = session.get('gameId')
    for game in game_list:
        if game.id == game_id:
            game.kickPlayer(user.name)
    user_list.remove(user)


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
    user = User(player_name, mode)

    user_list.append(user)
    gameId = newGame(user)

    session['playerId'] = user.uuid
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
                user = User.get_user_by_id(playerId)
                response = GetJSON(session.get('mode'), gameId, user)
                return jsonify(response)

        app.logger.info("View error: game not available")
        abort(410)  # Game not available

    else:
        app.logger.info("View error: invalid player id")
        abort(403)  # Invalid player id

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


@app.route('/leave', methods=['POST'])
def leave():
    
    return jsonify(ok=True)

if __name__ == '__main__':
    app.run(port=5500)
