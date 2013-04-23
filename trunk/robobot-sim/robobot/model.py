
import sys
import socket
import logging
import OptionMessage_pb2

log = logging.getLogger(__name__)

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
            yield line
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
    conf_sent = False
    def det_conf_msg (self):
        conf_msg = OptionMessage_pb2.OptionMessageEntity()
        conf_msg.id = 666
        conf_msg.type = 100
        conf_msg.sensors = 0
        conf_msg.commands |= 0x1
        conf_msg.commands |= 0x10
        return conf_msg.SerializeToString()

    cmd_dict = {
        0 : lambda v : control(map(int, v)),
        1 : lambda v : log.info('Additional command test') and None
    }

    def proto_cmd(self, v, sock):
        if not self.conf_sent:
            self.conf_sent = True
            return sock.send("\0" + self.det_conf_msg()) and None
        elif int(v[0]) == 1:
            return self.cmd_dict[int(v[1])](v[2:])
        else:
            log.info('Unknown command')

    def __init__(self, socket):
        self.socket = socket

    def start(self):
	    return imap(lambda str: self.proto_cmd(str.split('#'), self.socket),
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
	        if c:
		        self.model.update_control(c)
