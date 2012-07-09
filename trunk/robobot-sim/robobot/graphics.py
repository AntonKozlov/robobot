'''
Created on Jul 7, 2012

@author: Eldar Abusalimov
'''

import sys
import time
import math
import operator
from threading import Thread

try:
    import pygame
    from pygame import event
    from pygame import fastevent
    from pygame import display
    from pygame import image

except ImportError:
    print >>sys.stderr, '"pygame" not found'
    raise

class BounceBall(Thread):

    BLACK = 0, 0, 0
    INVERT_SPEED_EVENT = pygame.USEREVENT + 1
    SET_SPEED_EVENT = pygame.USEREVENT + 2

    def __init__(self, width=640, height=480):
        super(BounceBall, self).__init__()

        pygame.init()

        fastevent.init()

        self.width, self.height = width, height
        self.speed = [0.5, 0.5]
	self.speed_state = [0, 0]

        self.screen = display.set_mode((width, height))

        self.ball = image.load("robobot/ball.gif")
        self.ballrect = self.ball.get_rect()

    def post_set_speed(self, x=1, y=1):
        args = {"x" : x, "y" : y}
        fastevent.post(event.Event(self.SET_SPEED_EVENT, args))

    def post_invert_speed(self, x=True, y=True):
        args = {"x" : x, "y" : y}
        fastevent.post(event.Event(self.INVERT_SPEED_EVENT, args))

    def invert_speed(self, x=True, y=True):
        if x:
            self.speed[0] = -self.speed[0]
        if y:
            self.speed[1] = -self.speed[1]

    def set_speed(self, x = 1, y = 1):
	self.speed[0] = x
	self.speed[1] = y
	self.speed_state[0] = 0
	self.speed_state[1] = 0

    def run(self):
        while True:
            for e in fastevent.get():
                if e.type == pygame.QUIT:
                    sys.exit()
                elif e.type == self.INVERT_SPEED_EVENT:
                    self.invert_speed(e.x, e.y)
                elif e.type == self.SET_SPEED_EVENT:
                    self.set_speed(e.x, e.y)

	    self.speed_state = map(operator.add, self.speed_state, self.speed)
	    offset = map(lambda f: int(f / abs(f)) if abs(f) > 1 else 0, self.speed_state) 
	    self.speed_state = map(operator.sub, self.speed_state, offset)
	   
	    rect = self.ballrect
	    offset = [0 if (rect.left < 0 and offset[0] < 0) or 
			   (rect.right > self.width and offset[0] > 0)
			else offset[0],
		      0 if (rect.top < 0 and offset[1] < 0) or 
			   (rect.bottom > self.height and offset[1] > 0)
			else offset[1]]
            #rect = 
	    self.ballrect = self.ballrect.move(offset)
            #self.invert_speed(rect.left < 0 or rect.right > self.width,
			     #rect.top < 0 or rect.bottom > self.height)

            self.screen.fill(self.BLACK)
            self.screen.blit(self.ball, rect)
            pygame.display.flip()

            time.sleep(0.01)
