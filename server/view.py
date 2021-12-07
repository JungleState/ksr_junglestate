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

FIELD = (17, 17)


def view(game):
    run = True
    map = game.matrix
    while run:

        for event in pygame.event.get():
            if(event.type == pygame.QUIT):
                pygame.quit()
                sys.exit()

        keys = pygame.key.get_pressed()
        if keys[pygame.K_ESCAPE]:
            break

        elif keys[pygame.K_w]:
            doSpecificTurn(game, "1", 0)
        elif keys[pygame.K_s]:
            doSpecificTurn(game, "1", 4)
        elif keys[pygame.K_a]:
            doSpecificTurn(game, "1", 6)
        elif keys[pygame.K_d]:
            doSpecificTurn(game, "1", 2)
        elif keys[pygame.K_k]:
            doSpecificTurn(game, "2", 0)

        SCREEN.fill(BACKGROUND)
        for i in range(len(map)):
            for j in range(len(map[i])):
                if(map[i][j].id) == "FF":
                    pygame.draw.circle(SCREEN, GREEN,
                                       (j*40+20, i*40+20), 15)
                elif(map[i][j].id) == "BB":
                    pygame.draw.circle(SCREEN, YELLOW,
                                       (j*40+20, i*40+20), 15)
                elif(map[i][j].id) == "PP":
                    pygame.draw.circle(SCREEN, ORANGE,
                                       (j*40+20, i*40+20), 15)
                elif(map[i][j].id) == "CC":
                    pygame.draw.circle(SCREEN, BROWN,
                                       (j*40+20, i*40+20), 15)
                elif(map[i][j].id) != "  ":
                    pygame.draw.circle(SCREEN, RED,
                                       (j*40+20, i*40+20), 15)
        pygame.time.delay(100)
        pygame.display.flip()
        pygame.display.update()
    pygame.quit()

def doSpecificTurn(game, move, dir):
    game.addMove(100, move, dir)
    game.doNextRound()

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
    game = game_logic.Game(1, FIELD)
    game.join(f'Pacifico', 100)
    game.join("Phillip", 101)
    view(game)
