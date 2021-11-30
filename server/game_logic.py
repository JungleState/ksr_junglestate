from random import randint
import logging

logging.getLogger().setLevel("DEBUG")

FIELD_LENGTH = 17
FIELD_HEIGHT = 17


class Item:
    def __init__(self, name, id):
        self.name = name
        self.id = id

    def __str__(self) -> str:
        return self.id


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
        self.sight = 5  # dimension of field of view matrix, needs to be odd
        self.name = name
        self.lives = 3
        self.coconuts = 2
        self.points = 0


class MapGenerator:
    """ A map generator that creates empty maps with forest all around."""

    def generate(self, width, height):
        matrix = []
        for y in range(height):
            row = []
            matrix.append(row)
            for x in range(width):
                if y == 0 or y == height - 1 or x == 0 or x == width - 1:
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
            for y in range(len(matrix)-2):
                y += 1
                for x in range(len(matrix[y])-2):
                    x += 1
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
                            if y_coord != 0 and y_coord != len(matrix)-1 and x_coord != 0 and x_coord != len(matrix[y])-1 and matrix[y_coord][x_coord] == Items.FOREST:
                                matrix[y_coord][x_coord] = super().inner()
                                surrounding_obstacles -= 1
        return matrix


class Game:
    def __init__(self, id, generator=RandomGenerator(20, 1, 1, 1)):
        self.move_list = []
        self.id = id
        self.player_list = []
        self.state = 0
        self.round = 0
        # field dimension 1st element = x; 2nd element = y
        self.matrix = generator.purge(
            generator.generate(FIELD_LENGTH, FIELD_HEIGHT))
        self.field_dim = [FIELD_LENGTH, FIELD_HEIGHT]

    def join(self, name, id):
        logging.debug(f"Player {id} joined as {name}")
        player = Player(id, id, name)
        while True:
            x = randint(1, self.field_dim[0]-1)
            y = randint(1, self.field_dim[1]-1)
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

    def addMove(self, player_id, move_id, dir):
        # move_id list:
        # 0: Stay
        # 1: Move
        # 2: Shoot
        #
        # dir list:
        # -1: No direction
        # 0: up
        # 1: up right
        # 2: right
        # 3: down right
        # 4: down
        # 5: down left
        # 6: left
        # 7: up left
        for move in self.move_list:
            if move[0] == player_id:
                #DEBUG : logging.debug(f"Rejecting move from Player {player_id} who already moved.")
                return False

        if self.getPlayerFromID(player_id) == False:
            #DEBUG : logging.debug(f"Rejecting move from Player {player_id} who is dead.")
            return False

        logging.debug(f"Adding move from {player_id}.")
        self.move_list.append([player_id, move_id, dir])
        if len(self.move_list) == len(self.player_list):
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
        for move in self.move_list:  # check for moving
            if move[1] == "1":
                player = self.getPlayerFromID(move[0])
                self.executeMoving(player, move[2])

        for move in self.move_list:  # check for shooting
            if move[1] == "2":
                player = self.getPlayerFromID(move[0])
                self.executeShooting(player, move[2])

        self.move_list.clear()

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
            toCoordinates[1] = toCoordinates[1] - 1
        elif dir == 2:
            toCoordinates[0] = toCoordinates[0] + 1
        elif dir == 4:
            toCoordinates[1] = toCoordinates[1] + 1
        elif dir == 6:
            toCoordinates[0] = toCoordinates[0] - 1

        checkField = self.getElementAtCoords(toCoordinates)

        if checkField == Items.EMPTY:  # empty field
            self.setElementAt(player.x, player.y, Items.EMPTY)
            self.setElementAt(toCoordinates, player)
            player.x, player.y = toCoordinates[0], toCoordinates[1]

        elif checkField == Items.FOREST:  # forest field
            self.handlePlayerDamage(player)

        elif isinstance(checkField, Player):
            self.handlePlayerDamage(player)
            player2 = checkField
            self.handlePlayerDamage(player2)

        elif isinstance(checkField, Item):
            self.setElementAt(player.x, player.y, Items.EMPTY)
            self.setElementAtCoords(toCoordinates, player)
            player.x, player.y = toCoordinates[0], toCoordinates[1]
            # TODO: collect item

    def handlePlayerDamage(self, player):
        player.lives -= 1  # TODO custom damage depending on situation
        if player.lives < 1:
            self.setElementAt(player.x, player.y, Items.EMPTY)
            self.player_list.remove(player)

    def executeShooting(self, player, dir):
        logging.debug(f"Shooting player {player.id} in direction {dir}!")

        toCoordinates = [player.x, player.y]

        if dir == 0:
            toCoordinates[1] = toCoordinates[1] - 1
        elif dir == 1:
            toCoordinates[0] = toCoordinates[0] + 1
            toCoordinates[1] = toCoordinates[1] - 1
        elif dir == 2:
            toCoordinates[0] = toCoordinates[0] + 1
        elif dir == 3:
            toCoordinates[0] = toCoordinates[0] + 1
            toCoordinates[1] = toCoordinates[1] + 1
        elif dir == 4:
            toCoordinates[1] = toCoordinates[1] + 1
        elif dir == 5:
            toCoordinates[1] = toCoordinates[1] + 1
            toCoordinates[0] = toCoordinates[0] - 1
        elif dir == 6:
            toCoordinates[0] = toCoordinates[0] - 1
        elif dir == 7:
            toCoordinates[1] = toCoordinates[1] - 1
            toCoordinates[0] = toCoordinates[0] - 1

        checkField = self.getElementAtCoords(toCoordinates)

        if isinstance(checkField, Player):  # player field
            player2 = checkField
            player2.lives = player2.lives - 1
            logging.debug(f'Player {player2.uuid} hit')

    def GetFieldOfView(self, player_id):  # for specific player
        for player in self.player_list:
            if player.id == player_id:
                field_of_view_matrix = []
                # checks for vision disability ecause of field border/ detects point of player in view matrix
                sight_x = player.sight
                sight_y = player.sight
                point_of_player_in_sight_matrix = [
                    int(player.sight/2), int(player.sight/2)]

                if player.x < int(player.sight/2):
                    sight_x -= int(player.sight/2) - player.x
                    point_of_player_in_sight_matrix[0] -= player.sight - sight_x
                if player.x > self.field_dim[0] - int(player.sight/2):
                    sight_x -= int(player.sight/2) - \
                        self.field_dim[0] + player.x

                if player.y < int(player.sight/2):
                    sight_y -= int(player.sight/2) - player.y
                    point_of_player_in_sight_matrix[1] -= player.sight - sight_y
                if player.y > self.field_dim[1] - int(player.sight/2):
                    sight_y -= int(player.sight/2) - \
                        self.field_dim[1] + player.y

                # makes matrix
                for x in range(sight_x):
                    field_of_view_matrix.append([])
                    for y in range(sight_y):
                        field_of_view_matrix[x].append(
                            self.matrix[x+player.x-point_of_player_in_sight_matrix[0]][y+player.y-point_of_player_in_sight_matrix[1]].id)
                return field_of_view_matrix
        return []

    def GetPlayerVar(self, player_id, var):  # for specific player
        for player in self.player_list:
            if player.id == player_id:
                item_dict = {}
                # for item in player.item_list:
                #     item_dict[f'{item.name}'] = item.count
                return item_dict
        return []
