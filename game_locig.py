class Item:
    def __init__(self, name, id):
        self.name = name
        self.count = 0
        self.id = id

class Player:
    def __init__(self, id, name):
        self.id 
        self.x = 0
        self.y = 0
        self.sight = 5#dimension of field of view matrix, needs to be odd
        self.name = name
        self.health = 100
        self.item_list = [Item("coconut", 2), Item("banana", 3), Item("pinapple", 4)]
        self.points = 0
    

class Game:
    def __init__(self, id):
        self.id = id
        self.player_list = []
        self.state = "waiting"
        self.round = 0
        self.field_dim = [50, 50] #field dimension 1st element = x; 2nd element = y
        self.matrix = []
        for x in range(self.field_dim[0]):
            self.matrix.append([])
            for y in range(self.field_dim[1]):
                self.matrix[x].append(0)
          
    def join(self, id, name):
        self.player_list.append(Player(id, name))
    
    def GetPlayerListForJSON(self):
        player_list = []
        for player in self.player_list:
            player_list.append([player.id,
                                player.name,
                                player.health,
                                player.coconuts,
                                player.bananas,
                                player.pinapples,
                                player.poinzs])
        return player_list

    def GetFieldOfView(self, player_id):#for specific player
        for player in self.player_list:
            if player.id == player_id:
                field_of_view_matrix = []
                #checks for vision disability ecause of field border
                sight_x = player.sight
                sight_y = player.sight
                for border in range(int(player.sight_x/2)):
                    if player.x == border:
                        sight_x -= border+1 
                for border in range(int(player.sight_y/2)):
                    if player.y == border:
                        sight_y -= border+1 
                #makes matrix
                for x in range(sight_x):
                    field_of_view_matrix.append([])
                    for y in range(sight_y):
                        field_of_view_matrix.append(0)
                return field_of_view_matrix
        return []

    def GetItemList(self, player_id):#for specific player
        for player in self.player_list:
            if player.id == player_id:
                item_list = []
                for item in player.item_list:
                    item_list.append([item.id, item.count])
                return item_list
        return []