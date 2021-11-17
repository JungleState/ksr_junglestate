import numpy as np
import pygame
import time
from pygame import draw
from random import randint
from pygame.constants import KEYDOWN, MOUSEBUTTONDOWN, MOUSEBUTTONUP, MOUSEMOTION


##--------------------VARIABLES--------------------##
WIN_WIDTH = 510
WIN_HEIGHT = 510

FPS = 1000
TIME_DELAY = int(1000/FPS)
##--------------------COLORS--------------------##
WHITE = (255, 255, 255)
GRAY = (200, 200, 200)
BLACK = (0, 0, 0)
GREEN = (0, 255, 0)
BG_COLOR = WHITE
##--------------------FUNCTIONS--------------------##
##--------------------CLASSES--------------------##

Bush_spawning_prob = 8

def viewMatrix(s,m):
    for c in range(len(m)):
        for r in range(len(m[c])):
            if m[c][r] == 1:
                pygame.draw.rect(s, GREEN, (c*10, r*10, 10, 10))
            else:
                pygame.draw.rect(s, WHITE, (c*10, r*10, 10, 10))

def createMatrix():
    matrix = []
    for c in range(51):
        matrix.append([])
        for r in range(51):
            prob = randint(1, 100)
            if prob <= Bush_spawning_prob:
                matrix[c].append(1)
            else:
                matrix[c].append(0)

    return matrix


class Game:
    """
    Main GAME class
    """

    def __init__(self):
        pygame.init()
        pygame.font.init()
        self.screen = pygame.display.set_mode(
            (WIN_WIDTH, WIN_HEIGHT)
        )  # create screen which will display everything
        self.win = pygame.display.set_mode((WIN_WIDTH, WIN_HEIGHT))
        pygame.display.set_caption("Paint from wish")  # Game title
        self.game_play = False
        self.living_cells = []


    def play(self):
        matrix = createMatrix()
                
        while True:
            #key events
            for event in pygame.event.get():
                # Exit app if click quit button
                if event.type == pygame.QUIT:
                    run = False
            
            keys = pygame.key.get_pressed()
            if keys[pygame.K_ESCAPE]:
                self.exit()

            self.screen.fill(BG_COLOR)  # draw empty screen
            viewMatrix(self.screen, matrix)

            # Update
            pygame.time.delay(TIME_DELAY)
            pygame.display.flip()
            pygame.display.update()
        pygame.quit()

##--------------------MAIN--------------------##
if __name__ == "__main__":
    Game().play()