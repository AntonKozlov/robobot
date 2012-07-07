#!/usr/bin/env python2

import sys
import socket
import logging

import getopt

root_logger = logging.getLogger()
root_logger.setLevel(logging.DEBUG)
root_logger.addHandler(logging.StreamHandler(sys.stderr))

log = logging.getLogger(__name__)

def echo_server(host, port):
    # Create a TCP/IP socket
    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

    try:
        # Bind the socket to the port
        server_address = (host, port)
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
                        connection.sendall(data)
                    else:
                        log.info('no more data from %s', client_address)
                        break

            finally:
                # Clean up the connection
                connection.close()
    finally:
        sock.close()

if __name__ == '__main__':
    try:
        opts, args = getopt.gnu_getopt(sys.argv[1:], 'p:')
    except getopt.GetoptError, err:
        print str(err)
    params = ["host", "port"]
    host = "" # represents INADDR_ANY
    port = 10000
    #for o, a in opts:
    #    if o == '-p':
    #        port = int(a)
    

    with open('connection.conf') as f:
        for line in f:
            all = line.split('=', 1)
            if all[0] in params:
                exec(line)

    log.info("starting robobot simulator at \'%s:%d\'" % (host, port))

    echo_server(host, port)

