import pygame
import neat
import os
import time
import random

pygame.init()
WIN_WIDTH = 500
WIN_HEIGHT = 600
BASE_DIST = 75
GEN = 0

STAT_FONT = pygame.font.SysFont("comicsans", 50)
win = pygame.display.set_mode((WIN_WIDTH, WIN_HEIGHT))
pygame.display.set_caption("AI Flappy Bird")

anofunc_loadImg = lambda image, scale2x=True, dir='imgs': pygame.transform.scale2x(
    pygame.image.load(os.path.join(dir, "{}".format(image)))) if scale2x else \
    pygame.transform.scale(pygame.image.load(os.path.join(dir, "{}".format(image))), (1280, 720))

BIRD_IMGS = [anofunc_loadImg("bird{}.png".format(i)) for i in range(1, 4)]
PIPE_IMG, BASE_IMG, BG_IMG = anofunc_loadImg("pipe.png"), anofunc_loadImg("base.png"), anofunc_loadImg("bg.png", False)


class Bird:
    IMGS = BIRD_IMGS
    MAX_ROTATION = 25
    ROT_VEL = 20
    ANIMATION_TIME = 5

    def __init__(self, x, y):
        self.x = x
        self.y = y
        self.tilt = 0
        self.tick_count = 0
        self.vel = 0
        self.height = self.y
        self.img_count = 0
        self.img = self.IMGS[0]

    def jump(self):
        self.vel = -10.5
        self.tick_count = 0
        self.height = self.y

    def move(self):
        self.tick_count += 1
        d = self.vel * self.tick_count + 1.5 * self.tick_count ** 2
        if d >= 16:
            d = 16
        elif d < 0:
            d -= 2
        self.y += d

        if d < 0 or self.y < self.height + 50:
            if self.tilt < self.MAX_ROTATION:
                self.tilt = self.MAX_ROTATION
        elif self.tilt > -90:
            self.tilt -= self.ROT_VEL

    def draw(self, win):
        self.img_count += 1
        if self.tilt <= -80:
            self.img = self.IMGS[1]
        elif self.img_count < self.ANIMATION_TIME:
            self.img = self.IMGS[0]
        elif self.img_count < self.ANIMATION_TIME * 2:
            self.img = self.IMGS[1]
        elif self.img_count < self.ANIMATION_TIME * 3:
            self.img = self.IMGS[2]
        elif self.img_count < self.ANIMATION_TIME * 4:
            self.img = self.IMGS[1]
        else:
            self.img_count = 0

        rotate_img = pygame.transform.rotate(self.img, self.tilt)
        rect = rotate_img.get_rect(center=self.img.get_rect(topleft=(self.x, self.y)).center)

        win.blit(rotate_img, rect.topleft)

    def get_mask(self):
        return pygame.mask.from_surface(self.img)


class Pipe:
    GAP = 200
    VEL = 5

    def __init__(self, x):
        self.x = x
        self.height = random.randrange(50, WIN_HEIGHT - self.GAP - BASE_DIST)
        self.PIPE_TOP = pygame.transform.flip(PIPE_IMG, False, True)
        self.PIPE_BOTTOM = PIPE_IMG

        self.top = self.height - self.PIPE_TOP.get_height()
        self.bottom = self.height + self.GAP

        self.passed = False

    def move(self):
        self.x -= self.VEL

    def draw(self, win):
        win.blit(self.PIPE_TOP, (self.x, self.top))
        win.blit(self.PIPE_BOTTOM, (self.x, self.bottom))

    def collide(self, bird):
        get_mask = pygame.mask.from_surface
        bird_mask = bird.get_mask()
        top_mask = get_mask(self.PIPE_TOP)
        bottom_mask = get_mask(self.PIPE_BOTTOM)

        top_offset = (self.x - bird.x, self.top - round(bird.y))
        bottom_offset = (self.x - bird.x, self.bottom - round(bird.y))

        col = True if (bird_mask.overlap(bottom_mask, bottom_offset) or
                       bird_mask.overlap(top_mask, top_offset)) else False

        return True if col else False


class Base:
    VEL = 5
    IMG = BASE_IMG
    WIDTH = IMG.get_width()

    def __init__(self, y):
        self.y = y
        self.x1 = 0
        self.x2 = self.WIDTH

    def move(self):
        self.x1 -= self.VEL
        self.x2 -= self.VEL

        if self.x1 + self.WIDTH < 0:
            self.x1 = self.x2 + self.WIDTH
        elif self.x2 + self.WIDTH < 0:
            self.x2 = self.x1 + self.WIDTH

    def draw(self, win):
        win.blit(self.IMG, (self.x1, self.y))
        win.blit(self.IMG, (self.x2, self.y))


def draw_window(win, birds, base, pipes, score, gen, pipe_ind):
    win.blit(BG_IMG, (0, 0))
    for pipe in pipes:
        pipe.draw(win)
    for bird in birds:
        try:
            pygame.draw.line(win, (255, 0, 0), (bird.x + bird.img.get_width() / 2, bird.y + bird.img.get_height() / 2),
                             (pipes[pipe_ind].x + pipes[pipe_ind].PIPE_TOP.get_width() / 2, pipes[pipe_ind].height), 5)
            pygame.draw.line(win, (255, 0, 0), (bird.x + bird.img.get_width() / 2, bird.y + bird.img.get_height() / 2),
                             (pipes[pipe_ind].x + pipes[pipe_ind].PIPE_BOTTOM.get_width() / 2, pipes[pipe_ind].bottom), 5)
        except:
            pass
        finally:
            bird.draw(win)
    base.draw(win)

    text = STAT_FONT.render("Score: {}".format(score), 1, (255, 255, 255))
    win.blit(text, (WIN_WIDTH - 10 - text.get_width(), 10))

    text = STAT_FONT.render("Gen: {}".format(gen), 1, (255, 255, 255))
    win.blit(text, (5, 10))

    text = STAT_FONT.render("Birds: {}".format(len(birds)), 1, (255, 255, 255))
    win.blit(text, (5, 50))

    pygame.display.update()


def main(genomes, config):
    global GEN
    GEN += 1
    nets = []
    ge = []
    birds = []

    for _, g in genomes:
        net = neat.nn.FeedForwardNetwork.create(g, config)
        nets.append(net)
        birds.append(Bird(int(WIN_WIDTH / 2 - 100), int(WIN_HEIGHT / 2)))
        g.fitness = 0
        ge.append(g)

    base = Base(WIN_HEIGHT - BASE_DIST)

    score = 0

    add_pipe = False
    pipes = [Pipe(WIN_WIDTH)]
    clock = pygame.time.Clock()
    run = True
    while run:
        clock.tick(30)
        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                run = False
                pygame.quit()
                quit()
        pipe_ind = 0
        if len(birds) > 0:
            if len(pipes) > 1 and birds[0].x > pipes[0].x + pipes[0].PIPE_TOP.get_width():
                pipe_ind = 1
        else:
            run = False
            break

        for x, bird in enumerate(birds):
            bird.move()
            ge[x].fitness += 0.01

            output = nets[x].activate(
                (bird.y, abs(bird.y - pipes[pipe_ind].height), abs(bird.y - pipes[pipe_ind].bottom)))
            if output[0] > 0.5:
                bird.jump()

        pipes_to_del = []
        for pipe in pipes:
            for x, bird in enumerate(birds):
                if pipe.collide(bird):
                    ge[x].fitness -= 1
                    birds.pop(x)
                    nets.pop(x)
                    ge.pop(x)

                if not pipe.passed and pipe.x < bird.x:
                    try:
                        ge[x].fitness += 1
                        pipe.passed = True
                        add_pipe = True
                    except:
                        pass

            if pipe.x + PIPE_IMG.get_width() < 0:
                pipes_to_del.append(pipe)

            pipe.move()

        if add_pipe:
            score += 1
            add_pipe = False
            for g in ge:
                g.fitness += 1
            pipes.append(Pipe(WIN_WIDTH))

        for pipe in pipes_to_del:
            pipes.remove(pipe)
        for x, bird in enumerate(birds):
            if (bird.y + bird.img.get_height() >= WIN_HEIGHT - BASE_DIST or
                    bird.y + bird.img.get_height() < 0):
                ge[x].fitness -= 10
                birds.pop(x)
                nets.pop(x)
                ge.pop(x)

        base.move()

        draw_window(win, birds, base, pipes, score, GEN, pipe_ind)


def run(config_path):
    config = neat.config.Config(neat.DefaultGenome, neat.DefaultReproduction, neat.DefaultSpeciesSet,
                                neat.DefaultStagnation, config_path)

    p = neat.Population(config)

    p.add_reporter(neat.StdOutReporter(True))
    stats = neat.StatisticsReporter()
    p.add_reporter(stats)

    winner = p.run(main, 50)


if __name__ == "__main__":
    local_dir = os.path.dirname(__file__)
    config_path = os.path.join(local_dir, "CONFIG.TXT")
    run(config_path)
