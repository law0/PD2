#!/usr/bin/python

from client import connectToPartyServer
from client import Data

partyServer = connectToPartyServer("127.0.0.1", 9099, retry=6)
partyServer.reconnect(udpLocalPort = 9100) #change local udpPort -> useful if server is local
packet = Data("message")
packet.setData("to someone", "the message")
partyServer.send(packet)

packet.reinit("position")
packet.setData(1.0, 2.0, 3.0)
partyServer.send(packet)

for i in xrange(0, 3):
        packet.setData(0.0, 2.0, 3.0)
        partyServer.sendUdp(packet)
