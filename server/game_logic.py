from random import randint

FIELD_LENGH = 17
FIELD_HEIGHT = 17


class Item:
    def __init__(self, name, id):
        self.name = name
        self.count = 0
        self.id = id


class Player:
    def __init__(self, uuid, id, name):
        self.id = id
        self.uuid = uuid
        self.knockouts = 0
        self.hits = 0
        self.x = 0
        self.y = 0
        self.sight = 5  # dimension of field of view matrix, needs to be odd
        self.name = name
        self.health = 100
        self.item_list = [Item("coconuts", 2),
                          Item("bananas", 3),
                          Item("pinapples", 4)]
        self.points = 0


class Game:
    def __init__(self, id):
        self.move_list = []
        self.id = id
        self.forest_spawning_rate = 8  # in procentage
        self.item_spawning_rate = 10  # in procentage
        self.player_list = []
        self.state = 0
        self.round = 0
        # field dimension 1st element = x; 2nd element = y
        self.field_dim = [FIELD_LENGH-1, FIELD_HEIGHT-1]
        self.matrix = []
        self.createMap()

    def join(self, name, id):
        self.player_list.append(Player(id, name))
        x = randint(1, FIELD_LENGH-2)
        y = randint(1, FIELD_HEIGHT-2)
        self.matrix[x][y] = id
        self.player_list[len(self.player_list)-1].x = x
        self.player_list[len(self.player_list)-1].y = y

    def createMap(self):
        # create random
        for x in range(self.field_dim[0]+1):
            self.matrix.append([])
            for y in range(self.field_dim[1]+1):
                if x == 0 or x == FIELD_LENGH-1 or y == 0 or y == FIELD_HEIGHT-1:
                    self.matrix[x].append(1)
                else:
                    prob = randint(1, 100)
                    if prob <= self.forest_spawning_rate:
                        self.matrix[x].append(1)
                    elif prob <= self.forest_spawning_rate + self.item_spawning_rate:
                        self.matrix[x].append(2)
                    else:
                        self.matrix[x].append(0)
        
        for x in range(self.field_dim[0]-1):
            for y in range(self.field_dim[1]-1):
                surrounding_obstacle = 0
                plus_x_list = [1, 2, 1, 0]
                plus_y_list = [2, 1, 0, 1]
                for i in range(4):
                    if self.matrix[x+plus_x_list[i]][y+plus_y_list[i]] == 1:
                        surrounding_obstacle += 1
                
                # while surrounding_obstacle > 2:
                #     coord = randint(0, 3)
                #     if self.matrix[x+plus_x_list[coord]][y+plus_y_list[coord]] == 1:
                #         del self.matrix[x+plus_x_list[coord]][y+plus_y_list[coord]]
                #         surrounding_obstacle -= 1
            
                
                
        

        

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
                return False

        self.move_list.append([player_id, move_id, dir])
        if len(self.move_list) == len(self.player_list):
            self.doNextRound()
        return True

    def GetPlayerListForJSON(self):
        player_list = []
        for player in self.player_list:
            player_list.append({"id": player.id,
                                "name": player.name,
                                "health": player.health,
                                "knockouts": player.knockouts,
                                "hits": player.hits,
                                # coconuts
                                "coconuts": player.item_list[0].count,
                                # bananas
                                "bananas": player.item_list[1].count,
                                # pinapples
                                "pinapples": player.item_list[2].count,
                                "points": player.points})
        return player_list

    def doNextRound(self):
        for move in self.move_list:  # check for moves
            if move[1] == 1:
                for player in self.player_list:
                    if player.id == move[0]:

                        old_coor = [player.x, player.y]

                        if move[2] == 0:
                            player.y = player.y - 1
                        elif move[2] == 2:
                            player.x = player.x + 1
                        elif move[2] == 4:
                            player.y = player.y + 1
                        elif move[2] == 6:
                            player.x = player.x - 1

                        field = self.matrix[player.x][player.y]

                        if field == 0:  # empty field
                            self.matrix[player.x][player.y] = player.id
                            self.matrix[old_coor[0]
                                        ][old_coor[1]] = 0

                        elif field == 1:  # forrest field
                            player.x = old_coor[0]
                            player.y = old_coor[1]
                            # TODO: add player damage

                        elif field > 1 and field < 100:  # item field
                            # TODO: collect item
                            field = player.id
                            self.matrix[player.x][player.y] = player.id
                            self.matrix[old_coor[0]][old_coor[1]] = 0
                            pass

                        elif field > 99:
                            for player2 in self.player_list:
                                if player2.id == field:
                                    # TODO: add player damage
                                    # TODO: add player2 damage
                                    hasp2moved = False
                                    for move2 in self.move_list:
                                        if move2[0] == player2.id:
                                            if move2[1] == 1:
                                                hasp2moved = True
                                                break

                                    if hasp2moved:
                                        self.player_list.remove(player)
                                        self.matrix[old_coor[0]
                                                    ][old_coor[1]] = 0
                                        # TODO: remove a player
                                        pass

                                    else:
                                        player.x = old_coor[0]
                                        player.y = old_coor[1]

                                    # TODO: add player damage
                                    # TODO: add player2 damage
                                    break
                            player.x = old_coor[0]
                            player.y = old_coor[1]
                            # TODO: add player damage

        for move in self.move_list:  # check for shoot
            if move[1] == 2:
                for player in self.player_list:
                    if player.id == move[0]:
                        # TODO: add shooting
                        pass

        self.move_list.clear()

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
