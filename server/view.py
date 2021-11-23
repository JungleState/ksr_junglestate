from random import randint
import pygame
import sys

from pygame.constants import KEYDOWN
import game_logic

pygame.init()

WIN_WIDTH = 680
WIN_HEIGHT = 680

SCREEN = pygame.display.set_mode((WIN_WIDTH, WIN_HEIGHT))

BLACK = (0, 0, 0)
WHITE = (255, 255, 255)
YELLOW = (255, 211, 0)
BROWN = (121, 63, 13)
BACKGROUND = (195, 142, 90)
GREEN = (0, 82, 33)
RED = (255, 0, 0)
ORANGE = (255, 157, 0)

P1 = (120, 50, 0)
P2 = (120, 50, 15)
P3 = (120, 50, 30)
P4 = (120, 50, 45)
P5 = (120, 50, 60)


def view(game):
    gen = game_logic.RandomGenerator(30, 1, 1, 1)
    clock = pygame.time.Clock()
    run = True
    map = game.matrix
    while run:

        for event in pygame.event.get():
            if(event.type == pygame.QUIT):
                pygame.quit()
                sys.exit()

            if(event.type == KEYDOWN):
                map = gen.purge(map)

        SCREEN.fill(BACKGROUND)
        for i in range(len(map)):
            for j in range(len(map[i])):
                if(map[i][j].id) == "FF":
                    pygame.draw.circle(SCREEN, GREEN,
                                       (i*40+20, j*40+20), 15)
                elif(map[i][j].id) == "BB":
                    pygame.draw.circle(SCREEN, YELLOW,
                                       (i*40+20, j*40+20), 15)
                elif(map[i][j].id) == "PP":
                    pygame.draw.circle(SCREEN, ORANGE,
                                       (i*40+20, j*40+20), 15)
                elif(map[i][j].id) == "CC":
                    pygame.draw.circle(SCREEN, BROWN,
                                       (i*40+20, j*40+20), 15)
                elif(map[i][j].id) != "  ":
                    pygame.draw.circle(SCREEN, RED,
                                       (i*40+20, j*40+20), 15)
        clock.tick(10)
        pygame.display.update()


def doTurn(game):
    r1 = randint(0, 3)
    game.addMove(100, "1", r1*2)
    r2 = randint(0, 3)
    game.addMove(101, "1", r2*2)
    r3 = randint(0, 3)
    game.addMove(102, "1", r3*2)
    r4 = randint(0, 3)
    game.addMove(103, "1", r4*2)
    r5 = randint(0, 3)
    game.addMove(104, "1", r5*2)

    game.doNextRound()


if __name__ == "__main__":
    game = game_logic.Game(1)
    game.join(f'Sheran', 100)
    game.join(f'Joran', 101)
    game.join(f'Matthias', 102)
    game.join(f'Pacifico', 103)
    game.join(f'Philipp', 104)

    doTurn(game)
