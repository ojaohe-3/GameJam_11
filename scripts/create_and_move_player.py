import socket
import json
import time

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

host = socket.gethostbyname(socket.gethostname())
port = 4242
s.connect((host, port))

msg = json.dumps({"type": "playerInfo", "name": "dummy"}) + "\n"
s.send(msg.encode('utf-8'))

time.sleep(2)

for i in range(100):
    msg = json.dumps({"type": "move", "target": "dummy", "x": -1, "y": 0}) + "\n"
    s.send(msg.encode('utf-8'))
    time.sleep(0.01)

time.sleep(2)
