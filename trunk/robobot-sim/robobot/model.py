
import socket

from itertools import imap

class vector:
    def __init__(self, x, y):
        self.val = [x, y]
    def __init__(self, val):
        self.val = val
    def __plus__(self, another):
        return vector(map(lambda a,b: a + b, self, another ))

# Math model of robot (speed now)
class model:
    def __init__(self):
        pass
    def speed(self):
        return self.spd
    def update_control(self, control):
        self.spd = control.spd
        #print "speed updated to %s", str(control.spd)

class ball_model(model):
	def __init__(self, ball):
	    self.ball = ball	
	def update_control(self, control):
	    self.ball.post_set_speed(float(control.spd[0])/100, float(control.spd[1])/100)

# Control passed
class control:
    def __init__(self, spd=[0, 0]):
        self.spd = spd

# Provides an iterator to sockets data with separation on \n
def linesplit(socket):
    buffer = socket.recv(4096)
    done = False
    while not done:
        if "\n" in buffer:
            (line, buffer) = buffer.split("\n", 1)
            yield line+"\n"
        else:
            more = socket.recv(4096)
            if not more:
                done = True
            else:
                buffer = buffer+more
    if buffer:
        yield buffer

# Transalte raw input to control
class proto:
    def __init__(self, socket):
        self.socket = socket
    def start(self):
        return imap(lambda str: control(map(int, str.split())), 
                linesplit(self.socket))


# Split stream to pieces of n
def nsplit(socket, n):
    buffer = socket.recv(4096)
    done = False
    while not done:
        if len(buffer) < n:
            more = socket.recv(4096)
            if not more:
                    done = True
            buffer += more
        else:
            yield buffer[0:n]
            buffer = buffer[n+1:]
    if buffer:
        yield buffer

def car_proto_map(str):
    return control()

class car_proto(proto):
    def start(self):
        yield car_proto_map(nsplit(self.socket))

class robot:
    def __init__(self, model, proto):
        self.model = model
        self.proto = proto
    def start(self):
        for c in self.proto.start():
            self.model.update_control(c)
