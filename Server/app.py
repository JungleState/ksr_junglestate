import os
from typing import _SpecialForm
from flask import Flask
from flask import jsonify
from flask.helpers import get_template_attribute
from game_logic import Game


app = Flask(__name__)
app.secret_key = os.urandom(16)

next_game_id = 0
game_list = []
game_list.append(Game(next_game_id))
next_game_id += 1

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
@app.route("/")
def index():
    pass

@app.route("/SpectView/<int:game_id>")
def SpectView(game_id):
    for game in game_list:
        if game.id == game_id:
            return GetJSON("spec", game_id)

@app.route("/joinGame/<str:player_name>", methods=['POST'])
def joinGame(player_name):
    global next_game_id
    if len(game_list) == 0:
        game_list.append(Game(next_game_id))
        next_game_id += 1
    game_list[0].join(player_name)