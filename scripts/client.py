from twisted.internet import reactor
from twisted.internet.protocol import Protocol
from twisted.internet.endpoints import TCP4ClientEndpoint, connectProtocol

import json
import socket

class GameClient(Protocol):
    def dataReceived(self, data):
        data = json.loads(data)
        print(data)

host = socket.gethostbyname(socket.gethostname())
port = 4242
point = TCP4ClientEndpoint(reactor, host, port)
d = connectProtocol(point, GameClient())

reactor.run()
