#!/usr/bin/env python2

import sys
import socket
import logging
from threading import Thread
from robobot.graphics import BounceBall

import getopt

root_logger = logging.getLogger()
root_logger.setLevel(logging.DEBUG)
root_logger.addHandler(logging.StreamHandler(sys.stderr))

log = logging.getLogger(__name__)

class EchoServer(Thread):

    def __init__(self, ball, host='localhost', port=10000):
        super(EchoServer, self).__init__()
        self.host = host
        self.port = port
        self.ball = ball

    def run(self):
        # Create a TCP/IP socket
        sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

        try:
            # Bind the socket to the port
            server_address = (self.host, self.port)
            log.info('starting up on %s port %s', *server_address)
            sock.bind(server_address)

            # Listen for incoming connections
            sock.listen(1)

            while True: # Wait for a connection
                log.info('waiting for a connection')
                connection, client_address = sock.accept()
                try:
                    log.info('connection from %s', client_address)

                    # Receive the data in small chunks and retransmit it
                    while True:
                        data = connection.recv(16)
                        log.info('received "%s"', data)
                        if data:
                            log.info('sending data back to the client')
                            self.ball.post_invert_speed(len(data) % 2)
                            connection.sendall(data)
                        else:
                            log.info('no more data from %s', client_address)
                            break

                finally:
                    print 'closing connection'
                    # Clean up the connection
                    connection.close()
        finally:
            print 'closing socket'
            sock.close()

host = "localhost"
port = 10001

if __name__ == '__main__':
    try:
        opts, args = getopt.gnu_getopt(sys.argv[1:], 'p:')
    except getopt.GetoptError, err:
        print str(err)

    host = "" # represents INADDR_ANY
    port = 10000

    params = ["host", "port"]
    with open('connection.conf') as f:
        for line in f:
            param = line.split('=', 1)
            if param[0] in params:
                exec(line)

    for o, a in opts:
        if o == '-p':
            port = int(a)

    log.info("starting robobot simulator on %s port %d", host, port)

    ball = BounceBall()
    server = EchoServer(ball, host, port)

    ball.setDaemon(True)
    server.setDaemon(True)

    ball.start()
    server.start()

    while True:
        ball.join(1)
        server.join(1)


