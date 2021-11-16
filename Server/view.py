from random import randint
import pygame
import sys
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

P1 = (120, 60, 10)
P2 = (135, 75, 30)
P3 = (150, 90, 50)


def view(game):

    clock = pygame.time.Clock()
    run = True

    while run:
        for event in pygame.event.get():
            if(event.type == pygame.QUIT):
                pygame.quit()
                sys.exit()

        SCREEN.fill(BACKGROUND)
        map = doTurn(game)
        for i in range(len(map)):
            for j in range(len(map[i])):
                if(map[i][j]) == 1:
                    pygame.draw.circle(SCREEN, GREEN,
                                       (i*40+20, j*40+20), 20)
                elif(map[i][j]) == 2:
                    pygame.draw.circle(SCREEN, YELLOW,
                                       (i*40+20, j*40+20), 10)
                elif(map[i][j]) > 99:
                    if map[i][j] == 100:
                        pygame.draw.circle(SCREEN, P1,
                                           (i*40+20, j*40+20), 15)
                    if map[i][j] == 101:
                        pygame.draw.circle(SCREEN, P2,
                                           (i*40+20, j*40+20), 15)
                    if map[i][j] == 102:
                        pygame.draw.circle(SCREEN, P3,
                                           (i*40+20, j*40+20), 15)
        clock.tick(10)
        pygame.display.update()


def doTurn(game):
    game.doNextRound()
    r1 = randint(0, 3)
    game.addMove(100, 1, r1*2)
    r2 = randint(0, 3)
    game.addMove(101, 1, r2*2)
    r3 = randint(0, 3)
    game.addMove(102, 1, r3*2)
    return game.matrix


if __name__ == "__main__":
    game = game_logic.Game(1)
    game.join(f'Sheran', 100)
    game.join(f'Joran', 101)
    game.join(f'Matthias', 102)
    view(game)
