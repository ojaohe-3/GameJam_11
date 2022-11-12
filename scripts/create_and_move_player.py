import socket
import json
import time
import sys

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

name = sys.argv[1] if len(sys.argv) > 1 else "dummy"
n_iter = int(sys.argv[2]) if len(sys.argv) > 2 else 10

host = socket.gethostbyname(socket.gethostname())
port = 4242
s.connect((host, port))

msg = json.dumps({"type": "playerInfo", "name": name}) + "\n"
s.send(msg.encode('utf-8'))

time.sleep(2)

for i in range(n_iter):
    msg = json.dumps({"type": "move", "target": name, "x": -1, "y": 0}) + "\n"
    s.send(msg.encode('utf-8'))
    time.sleep(0.01)

time.sleep(2)
