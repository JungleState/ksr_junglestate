import os
from flask import Flask, jsonify, session, abort, render_template, redirect, url_for, request
from game_logic import Game
import threading
import uuid

NAME_LENGTH_MAX = 30
MAX_PLAYER_TIMEOUT = 10

app = Flask(__name__, template_folder='templates')
app.logger.setLevel("DEBUG")
app.secret_key = os.urandom(16)

app.config.update(
    TEMPLATES_AUTO_RELOAD=True
)

user_list = []
game_list = []
FIELD = (30, 20)


class User:
    def __init__(self, name, mode):
        self.mode = mode
        self.uuid = str(uuid.uuid4())
        self.active = True
        self.name = self.set_name(name)
        self.game_id = newGame(self)
        self.timer = threading.Timer(MAX_PLAYER_TIMEOUT, kickPlayer, [self])
    
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

        return None


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


def GetJSON(game_id, user):
    for game in game_list:
        if game.id == game_id:
            if user.mode == "client":#returns JSON file for client
                return {"field":game.GetFieldOfView(user.uuid),
                               "coconuts":game.GetPlayerVar(user.uuid, "CC"),
                               "lives":game.GetPlayerVar(user.uuid, "lives"),
                               "points":game.GetPlayerVar(user.uuid, "P"),
                               "round":game.round,
                               "mode":user.mode,
                               "name":user.name,
                               "name_list":game.GetPlayers()}
            elif user.mode == "spec":#returns JSON file for spectator
                return {"id":user.uuid, 
                               "field":game.SerializeMatrix(), 
                               "state":game.state, 
                               "round":game.round,
                               "scoreboard":game.Scoreboard("points", "decr"),
                               "mode":user.mode,
                               "name_list":game.GetPlayers()}

def updatePlayerActive(user):
    for game in game_list:
        if game.id == user.game_id:
            for i, player in enumerate(game.player_list):
                if player.uuid == user.uuid:
                    game.player_list[i].active = user.active
                    return 

def isLoggedIn():
    user = User.get_user_by_id(session.get('playerId'))

    if not user:
        session['playerId'] = None
    else:
        user.active = True
        updatePlayerActive(user)
        user.timer.cancel()
        user.timer = threading.Timer(MAX_PLAYER_TIMEOUT, kickPlayer, [user])
        user.timer.start()

    return user

def checkLogInData(name, mode):
    err = None

    # Check Name
    if len(name) == name.count(' ') or len(name) == 0:
        err = 'Invalid Name'
    elif len(name) > NAME_LENGTH_MAX:
        err = 'Too Many Characters'
    elif name in [user.name for user in user_list if user.mode == 'client'] and mode == 'client':
        err = 'Name Already In Use'
    elif session.get('playerId'):
        err = 'Already Logged In'

    # Check Mode
    if mode != 'client' and mode != 'spec':
        err = 'Invalid Mode'

    return err


def kickPlayer(user):
    app.logger.debug(f"Kicked {user.name}")
    for game in game_list:
        if game.id == user.game_id:
            game.kickPlayer(user.name)
    user_list.remove(user)


### JSON ENDPOINTS ###


@app.route('/', methods=['GET'])
def root():
    user = isLoggedIn()
    if not user:
        return redirect(url_for('login'))
    else:
        dimension = None
        if user.mode == 'client':
            dimension = (5, 5)
        elif user.mode == 'spec':
            dimension = FIELD
        return render_template('view.html', dimension_x=dimension[0], dimension_y=dimension[1], mode=user.mode)


@app.route('/login', methods=['GET'])
def login():
    return render_template('login.html')

@app.route('/getGames')
def getGames():
    gamesJson = {"games": []}
    for game in game_list:
        gamesJson['games'].append({
            "id": game.id,
            "players": len(game.player_list),
            "secured": bool(game.password)
        })

    return jsonify(gamesJson)

@app.route('/joinGame', methods=['POST'])
def joinGame():

    data = request.get_json() # Post request arguments
    player_name = data['player_name']
    player_mode = data['player_mode']
    game_mode = data['game_mode']

    err = checkLogInData(player_name, player_mode)

    if err:
        # Invalid name or mode
        app.logger.info(err)
        return jsonify(ok=False, msg=err)

    # Login data valid
    user = User(player_name, player_mode)
    user_list.append(user)
    session['playerId'] = user.uuid

    return jsonify(ok=True)

# View - Server knows if the request comes from a spectator or a player


@app.route('/view', methods=['GET'])
def view():
    user = isLoggedIn()
    if user:
        for game in game_list:
            if game.id == user.game_id:
                response = GetJSON(user.game_id, user)
                return jsonify(response)

        app.logger.info("View error: game not available")
        abort(410)  # Game not available

    else:
        app.logger.info("View error: invalid player id")
        abort(403)  # Invalid player id

# Input


@app.route('/action/<moveType>/<direction>', methods=['POST'])
def action(moveType, direction):
    user = isLoggedIn()
    if user:
        if user.mode == 'client':
            for game in game_list:
                if game.id == user.game_id:
                    game.addMove(user.uuid, int(moveType), int(direction))

            return jsonify(ok=True)

        else:
            app.logger.info("Action error: spectator is not allowed to move")
            return jsonify(ok=False)

    else:
        app.logger.info("Action error: invalid player id")
        abort(403)

# Inactive - AFK (player will be kicked automatically after timeout)

@app.route('/inactive', methods=['POST'])
def leave():
    user = User.get_user_by_id(session.get('playerId'))
    if user:
        print(f"{user.name} has left the game")
        user.active = False
        updatePlayerActive(user)

    return jsonify(ok=True)

if __name__ == '__main__':
    app.run(debug=True, host="0.0.0.0", port=int(os.environ.get("PORT", 5500)))
