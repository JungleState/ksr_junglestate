FIELD_LENGH = 50
FIELD_HEIGHT = 50


class Item:
    def __init__(self, name, id):
        self.name = name
        self.count = 0
        self.id = id


class Player:
    def __init__(self, id, name):
        self.id = id
        self.knockouts = 0
        self.hits = 0
        self.x = 0
        self.y = 0
        self.sight = 5  # dimension of field of view matrix, needs to be odd
        self.name = name
        self.health = 100
        self.item_list = [Item("coconuts", 2), Item(
            "bananas", 3), Item("pinapples", 4)]
        self.points = 0


class Game:
    def __init__(self, id):
        self.move_list = []
        self.id = id
        self.player_list = []
        self.state = 0
        self.round = 0
        # field dimension 1st element = x; 2nd element = y
        self.field_dim = [FIELD_LENGH-1, FIELD_HEIGHT-1]
        self.matrix = []
        for x in range(self.field_dim[0]+1):
            self.matrix.append([])
            for y in range(self.field_dim[1]+1):
                self.matrix[x].append(0)

    def join(self, name):
        id = 100
        self.player_list.append(Player(id, name))

    def addMove(self, player_id, move_id, dir):
        # move_id list:
        # 0: Stay
        # 1: Move
        # 2: Shoot
        # 
        # dir list:
        # 0: up
        # 1: up right
        # 2: right
        # 3: down right
        # 4: down
        # 5: down left
        # 6: left
        # 7: up left
        pass

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
