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
    def __init__(self, forest_spawning_rate, coconut_rate, banana_rate):
        self.forest_spawning_rate = forest_spawning_rate
        self.coconut_rate = coconut_rate
        self.banana_rate = banana_rate

    def inner(self):
        prob = randint(1, 100)
        if prob <= self.forest_spawning_rate:
            return Items.FOREST
        elif prob <= self.forest_spawning_rate + self.coconut_rate:
            return Items.COCONUT
        elif prob <= self.forest_spawning_rate + self.coconut_rate + self.banana_rate:
            return Items.BANANA
        else:
            return super().inner()


class Game:
    def __init__(self, id, generator=RandomGenerator(0, 1, 1)):
        self.move_list = []
        self.id = id
        self.player_list = []
        self.state = 0
        self.round = 0
        # field dimension 1st element = x; 2nd element = y
        self.matrix = generator.generate(FIELD_LENGTH, FIELD_HEIGHT)
        self.field_dim = [len(self.matrix[0]), len(self.matrix)]

    def join(self, name, id):
        logging.debug(f"Player {id} joined as {name}")
        player = Player(id, id, name)
        while True:
            x = randint(1, self.field_dim[0])
            y = randint(1, self.field_dim[1])
            if self.matrix[y][x] == Items.EMPTY:
                player.x = x
                player.y = y
                self.matrix[y][x] = player
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
                logging.debug(f"Rejecting move from Player {player_id} who already moved.")
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

        for move in self.move_list:  # check for shoot
            if move[1] == "2":
                for player in self.player_list:
                    if player.id == move[0]:

                        shoot_coor = [player.x, player.y]

                        if move[2] == 0:
                            shoot_coor[1] = shoot_coor[1] - 1
                        elif move[2] == 1:
                            shoot_coor[0] = shoot_coor[0] + 1
                            shoot_coor[1] = shoot_coor[1] - 1
                        elif move[2] == 2:
                            shoot_coor[0] = shoot_coor[0] + 1
                        elif move[2] == 3:
                            shoot_coor[0] = shoot_coor[0] + 1
                            shoot_coor[1] = shoot_coor[1] + 1
                        elif move[2] == 4:
                            shoot_coor[1] = shoot_coor[1] + 1
                        elif move[2] == 5:
                            shoot_coor[1] = shoot_coor[1] + 1
                            shoot_coor[0] = shoot_coor[0] - 1
                        elif move[2] == 6:
                            shoot_coor[0] = shoot_coor[0] - 1
                        elif move[2] == 7:
                            shoot_coor[1] = shoot_coor[1] - 1
                            shoot_coor[0] = shoot_coor[0] - 1

                        # if self.matrix[shoot_coor[0]][shoot_coor[1]]
                        # self.matrix

                        # TODO: add shooting
                        pass

        self.move_list.clear()

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

        checkField = self.matrix[toCoordinates[0]][toCoordinates[1]]

        if checkField == Items.EMPTY:  # empty field
            self.matrix[player.x][player.y] = Items.EMPTY
            self.matrix[toCoordinates[0]][toCoordinates[1]] = player
            player.x, player.y = toCoordinates[0], toCoordinates[1]

        elif checkField == Items.FOREST:  # forest field
            player.lives = player.lives - 1
            if player.lives < 1:
                self.matrix[player.x][player.y] = Items.EMPTY
                self.player_list.remove(player)

        elif isinstance(checkField, Item) and not isinstance(checkField, Player):
            self.matrix[player.x][player.y] = Items.EMPTY
            self.matrix[toCoordinates[0]][toCoordinates[1]] = player
            player.x, player.y = toCoordinates[0], toCoordinates[1]
            # TODO: collect item

        elif isinstance(checkField, Player):
            player.lives = player.lives - 1
            player2 = self.matrix[toCoordinates[0]][toCoordinates[1]]
            player2.lives = player2.lives - 1

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
                            self.matrix[x+player.x-point_of_player_in_sight_matrix[0]][y+player.y-point_of_player_in_sight_matrix[1]])
                return field_of_view_matrix
        return []

    def GetItemDict(self, player_id):  # for specific player
        for player in self.player_list:
            if player.id == player_id:
                item_dict = {}
                for item in player.item_list:
                    item_dict[f'{item.name}'] = item.count
                return item_dict
        return []
