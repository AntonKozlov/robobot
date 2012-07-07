#!/usr/bin/env python2

import sys
import socket
import logging

root_logger = logging.getLogger()
root_logger.setLevel(logging.DEBUG)
root_logger.addHandler(logging.StreamHandler(sys.stderr))

log = logging.getLogger(__name__)

def echo_server(port):
    # Create a TCP/IP socket
    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

    try:
        # Bind the socket to the port
        server_address = ('localhost', port)
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
    echo_server(10000)

