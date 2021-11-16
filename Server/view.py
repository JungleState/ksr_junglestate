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
                    pygame.draw.circle(SCREEN, BROWN,
                                       (i*40+20, j*40+20), 15)
        clock.tick(30)
        pygame.display.update()


def doTurn(game):
    return game.matrix


if __name__ == "__main__":
    game = game_logic.Game(1)
    view(game)
