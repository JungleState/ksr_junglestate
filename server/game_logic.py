from random import randint
import logging
import threading
logging.getLogger().setLevel("DEBUG")


class Item:
    def __init__(self, name, id):
        self.name = name
        self.id = id

    def __str__(self) -> str:
        return self.id


SIGHT = 2


class Rules:
    TIME_TO_MOVE = 0.5

    class Scores:
        KNOCK_OUT = 25
        HIT = 10
        PINEAPPLE = 50
        BANANA = 25

    class Damage:
        COCONUT = 1
        FOREST = 1
        PLAYER = 1


class Items:
    EMPTY = Item("empty", "  ")
    FOREST = Item("forest", "FF")
    COCONUT = Item("coconut", "CC")
    BANANA = Item("banana", "BB")
    PINEAPPLE = Item("pineapple", "PP")


class Player(Item):
    def __init__(self, uuid, id, name):
        self.id = id
        self.uuid = uuid
        self.knockouts = 0
        self.hits = 0
        self.x = 0
        self.y = 0
        self.sight = SIGHT * 2 + 1  # dimension of field of view matrix, needs to be odd
        self.name = name
        self.lives = 3
        self.coconuts = 2
        self.points = 0
        self.state = 0  # 0 = alive ; 1 = dead

    def item_dict(self):
        return {"coconuts":self.coconuts,
                "id": self.id,
                "knockouts": self.knockouts,
                "hits": self.hits,
                "name": self.name,
                "lives": self.lives,
                "points": self.points}

class MapGenerator:
    """ A map generator that creates empty maps with forest all around."""

    def generate(self, width, height):
        matrix = []
        for y in range(height):
            row = []
            matrix.append(row)
            for x in range(width):
                if y < SIGHT or y >= height - SIGHT or x < SIGHT or x >= width - SIGHT:
                    row.append(self.border())
                else:
                    row.append(self.inner())
        return matrix

    def border(self):
        return Items.FOREST

    def inner(self):
        return Items.EMPTY


class RandomGenerator(MapGenerator):
    def __init__(self, forest_spawning_rate, coconut_rate, banana_rate, pinapple_rate):
        self.forest_spawning_rate = forest_spawning_rate
        self.coconut_rate = coconut_rate
        self.banana_rate = banana_rate
        self.pinapple_rate = pinapple_rate

    def inner(self):
        prob = randint(1, 100)
        if prob <= self.forest_spawning_rate:
            return Items.FOREST
        elif prob <= self.forest_spawning_rate + self.coconut_rate:
            return Items.COCONUT
        elif prob <= self.forest_spawning_rate + self.coconut_rate + self.banana_rate:
            return Items.BANANA
        elif prob <= self.forest_spawning_rate + self.coconut_rate + self.banana_rate + self.pinapple_rate:
            return Items.PINEAPPLE
        else:
            return super().inner()

    def purge(self, matrix):
        plus_list = [1, -1, 0, 0]
        too_many_surrounding_obstacles = 1
        while too_many_surrounding_obstacles > 0:
            too_many_surrounding_obstacles = 0
            for y in range(len(matrix)-SIGHT*2):
                y += SIGHT
                for x in range(len(matrix[y])-SIGHT*2):
                    x += SIGHT
                    if matrix[y][x] != Items.FOREST:
                        surrounding_obstacles = 0
                        for i in range(4):
                            if matrix[y+plus_list[i]][x+plus_list[-(i+1)]] == Items.FOREST:
                                surrounding_obstacles += 1
                        if surrounding_obstacles > 2:
                            too_many_surrounding_obstacles += 1
                        while surrounding_obstacles > 2:
                            index = randint(0, 3)
                            y_coord = y+plus_list[index]
                            x_coord = x+plus_list[-(index+1)]
                            if y_coord > SIGHT-1 and y_coord < len(matrix)-SIGHT and x_coord > SIGHT-1 and x_coord < len(matrix[y])-SIGHT and matrix[y_coord][x_coord] == Items.FOREST:
                                matrix[y_coord][x_coord] = super().inner()
                                surrounding_obstacles -= 1
        return matrix


class Game:
    def __init__(self, id, field_dimensions, generator=RandomGenerator(20, 1, 1, 1)):
        self.id = id
        self.state = 0
        self.round = 0
        self.move_list = []
        self.player_list = []
        self.safed_items_list = []
        (self.field_lengh, self.field_height) = field_dimensions
        # field dimension 1st element = x; 2nd element = y
        self.matrix = generator.purge(
            generator.generate(self.field_lengh, self.field_height))
        self.field_dim = [self.field_lengh, self.field_height]

    def join(self, name, id):
        logging.debug(f"Player {id} joined as {name}")
        player = Player(id, id, name)
        while True:
            x = randint(1, self.field_dim[0]-SIGHT)
            y = randint(1, self.field_dim[1]-SIGHT)
            if self.getElementAt(x, y) == Items.EMPTY:
                player.x = x
                player.y = y
                self.setElementAt(x, y, player)
                self.player_list.append(player)
                break

    def SerializeMatrix(self):
        rows = []
        for row in self.matrix:
            rows.append("".join([self.SerializeItem(item) for item in row]))
        return rows

    def SerializeItem(self, item):
        if isinstance(item, Player):
            return f"{self.player_list.index(item):02d}"
        return str(item)

    def getPlayerFromID(self, player_id):
        for player in self.player_list:
            if player.id == player_id:
                return player
        return False

    def kickPlayer(self, player_name):
        """gets player name, kicks player"""
        for player in self.player_list:
            if player.name == player_name:
                coconut_in_this_cell = False
                for safed_item in self.safed_items_list:
                    if [player.x, player.y] == safed_item[1]:
                        coconut_in_this_cell = True
                        self.setElementAt(player.x, player.y, Items.COCONUT)
                        del self.safed_items_list[self.safed_items_list.index(
                            safed_item)]
                        break
                if not coconut_in_this_cell:
                    self.setElementAt(player.x, player.y, Items.EMPTY)
                del self.player_list[self.player_list.index(player)]

    def addMove(self, player_id, move_id, dir):
        """move_id list:
        0: Stay
        1: Move
        2: Shoot
        
        dir list:
        -1: No direction
        0: up
        1: up right
        2: right
        3: down right
        4: down
        5: down left
        6: left
        7: up left"""
        if len(self.move_list) == 0:
            timer = threading.Timer(Rules.TIME_TO_MOVE, self.doNextRound)
            timer.start()

        for move in self.move_list:
            if move[0] == player_id:
                self.move_list[self.move_list.index(move)] = [
                    player_id, move_id, dir]
                return True

        if self.getPlayerFromID(player_id).state == 1:
            logging.debug(f"Rejecting move from Player {player_id} who is knocked out.")
            return False

        logging.debug(f"Adding move from {player_id}.")
        self.move_list.append([player_id, move_id, dir])

        alive = 0
        for player in self.player_list:
            if player.state == 0:
                alive += 1

        if len(self.move_list) == alive:
            logging.debug(f"All players moved - next round!")
            self.doNextRound()
        return True

    def GetPlayerListForJSON(self):
        player_list = []
        for player in self.player_list:
            player_list.append({"id": player.id,
                                "name": player.name,
                                "health": player.lives,
                                "knockouts": player.knockouts,
                                "hits": player.hits,
                                # coconuts
                                "coconuts": player.coconuts,
                                "points": player.points})
        return player_list

    def doNextRound(self):
        move_list = list(self.move_list)
        self.move_list.clear()
        for move in move_list:  # check for moving
            if move[1] == 1:
                player = self.getPlayerFromID(move[0])
                self.executeMoving(player, move[2])

        for move in move_list:  # check for shooting
            if move[1] == 2:
                player = self.getPlayerFromID(move[0])
                self.executeShooting(player, move[2])

        for player in self.player_list:
            if player.lives <= 0 and player.state == 0:
                player.state = 1
                self.setElementAt(player.x, player.y, Items.EMPTY)

        for safed_item in self.safed_items_list:
            if safed_item[2] != self.round:  # Item is from previous round round
                if self.getElementAtCoords(safed_item[1]) == Items.EMPTY:
                    self.setElementAtCoords(safed_item[1], safed_item[0])
                elif isinstance(self.getElementAtCoords(safed_item[1]), Player) == False:
                    del self.safed_items_list[self.safed_items_list.index(
                        safed_item)]
        self.round += 1

    def getElementAt(self, x, y):
        return self.matrix[y][x]

    def getElementAtCoords(self, coords):
        return self.getElementAt(coords[0], coords[1])

    def setElementAt(self, x, y, item):
        self.matrix[y][x] = item

    def setElementAtCoords(self, coords, item):
        self.setElementAt(coords[0], coords[1], item)

    def executeMoving(self, player, dir):
        logging.debug(f"Moving player {player.id} in direction {dir}!")

        toCoordinates = [player.x, player.y]

        if dir == 0:
            toCoordinates[1] -= 1
        elif dir == 2:
            toCoordinates[0] += 1
        elif dir == 4:
            toCoordinates[1] += 1
        elif dir == 6:
            toCoordinates[0] -= 1

        checkField = self.getElementAtCoords(toCoordinates)

        if checkField == Items.EMPTY:  # empty field
            self.setElementAt(player.x, player.y, Items.EMPTY)
            self.setElementAtCoords(toCoordinates, player)
            player.x, player.y = toCoordinates[0], toCoordinates[1]

        elif checkField == Items.FOREST:  # forest field
            self.handlePlayerDamage(player, Rules.Damage.FOREST)

        elif isinstance(checkField, Player):
            self.handlePlayerDamage(player, Rules.Damage.PLAYER)
            player2 = checkField
            self.handlePlayerDamage(player2, Rules.Damage.PLAYER)

        elif isinstance(checkField, Item):
            if checkField == Items.PINEAPPLE:
                self.handleScore(player, Rules.Scores.PINEAPPLE)

            elif checkField == Items.BANANA:
                if player.lives < 3:
                    player.lives += 1
                else:
                    self.handleScore(player, Rules.Scores.BANANA)

            elif checkField == Items.COCONUT:
                print(player.coconuts)
                if player.coconuts < 3:
                    player.coconuts += 1
                    for safed_item in self.safed_items_list:
                        if safed_item[1] == toCoordinates:
                            index = self.safed_items_list.index(safed_item)
                            del self.safed_items_list[index]
                            break
                else:
                    safed_item_in_safed_items_list = False
                    for safed_item in self.safed_items_list:
                        if safed_item[0] == toCoordinates:
                            safed_item_in_safed_items_list = True
                    if not safed_item_in_safed_items_list:
                        self.safed_items_list.append(
                            (Items.COCONUT, toCoordinates, self.round))
            if checkField != Items.FOREST:  # empty field
                self.setElementAt(player.x, player.y, Items.EMPTY)
                self.setElementAtCoords(toCoordinates, player)
                player.x, player.y = toCoordinates[0], toCoordinates[1]

    def handlePlayerDamage(self, player, damage=1):
        """Inflicts damage on the given player and returns True if the player is knocked out."""
        logging.debug(f'Player {player.uuid} is hurting {damage}')
        player.lives -= damage
        if player.lives < 1:
            logging.debug(f'Player {player.uuid} is knocked out - sleep well!')
            player.state = 1
            self.setElementAt(player.x, player.y, Items.EMPTY)
            return True
        return False

    def handleScore(self, player, score=0):
        """Changes the given player's score."""
        logging.debug(f'Player {player.uuid} scored {score}')
        player.points += score

    def executeShooting(self, player, dir):
        logging.debug(f"Shooting player {player.id} in direction {dir}!")

        toCoordinates = [player.x, player.y]

        if dir == 0:
            toCoordinates[1] -= 1
        elif dir == 1:
            toCoordinates[0] += 1
            toCoordinates[1] -= 1
        elif dir == 2:
            toCoordinates[0] += 1
        elif dir == 3:
            toCoordinates[0] += 1
            toCoordinates[1] += 1
        elif dir == 4:
            toCoordinates[1] += 1
        elif dir == 5:
            toCoordinates[1] += 1
            toCoordinates[0] -= 1
        elif dir == 6:
            toCoordinates[0] -= 1
        elif dir == 7:
            toCoordinates[1] -= 1
            toCoordinates[0] -= 1

        checkField = self.getElementAtCoords(toCoordinates)

        if isinstance(checkField, Player):  # player field
            player2 = checkField
            logging.debug(f'Player {player2.uuid} hit')
            if self.handlePlayerDamage(player2, Rules.Damage.COCONUT):
                self.handleScore(player, Rules.Scores.KNOCK_OUT)
            else:
                self.handleScore(player, Rules.Scores.HIT)

        player.coconuts -= 1

        for safed_item in self.safed_items_list:
            if safed_item[1] == [player.x, player.y]:
                player.coconuts += 1
                del self.safed_items_list[self.safed_items_list.index(
                    safed_item)]
                break

    def getFOV(self, player):
        field_of_view_matrix = []
        sight_x = player.sight
        sight_y = player.sight
        point_of_player_in_sight_matrix = [
            int(player.sight/2), int(player.sight/2)]

        # makes matrix
        for y in range(sight_y):
            field_of_view_matrix.append("")
            for x in range(sight_x):
                final_y = y+player.y-point_of_player_in_sight_matrix[1]
                final_x = x+player.x-point_of_player_in_sight_matrix[0]
                field_of_view_matrix[y] += self.SerializeItem(
                    self.getElementAt(final_x, final_y))

        return field_of_view_matrix

    def GetFieldOfView(self, player_id):  # for specific player
        for player in self.player_list:
            if player.id == player_id:
                return self.getFOV(player)

    def GetPlayerVar(self, player_id, item):  # for specific player
        for player in self.player_list:
            if player.id == player_id:
                if item == "CC":
                    return player.coconuts
                elif item == "P":
                    return player.points
                elif item == "lives":
                    return player.lives

    def GetPlayers(self):
        return {player.id : player.name for player in self.player_list}

    def Scoreboard(self, sortby, hyrarchy):
        sorted_player_list = [i for i in range(len(self.player_list))]
        item_list_dict = {"coconuts":[player.coconuts for player in self.player_list],
                           "lives": [player.lives for player in self.player_list],
                           "points": [player.points for player in self.player_list],
                           "knockouts": [player.knockouts for player in self.player_list],
                           "hits": [player.hits for player in self.player_list],
                           "name": [player.name[0] for player in self.player_list]}

        sorted_list = sorted(item_list_dict[f"{sortby}"])
        item_list = item_list_dict[f"{sortby}"]
        for i in range(len(item_list)):
            plus_index = 0
            while isinstance(sorted_player_list[sorted_list.index(item_list[i]) + plus_index], Player):
                plus_index += 1
            sorted_player_list[sorted_list.index(item_list[i]) + plus_index] = self.player_list[i]
        
        sorted_player_id_list = [player.item_dict() for player in sorted_player_list]
        if hyrarchy == "decr":
            if sortby != "name":
                sorted_player_id_list.reverse()
        elif hyrarchy == "incr":
            if sortby == "name":
                sorted_player_id_list.reverse()
        return sorted_player_id_list