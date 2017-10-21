#!/usr/bin/python

from client import connectToPartyServer
from client import Data
from time import sleep

with connectToPartyServer("127.0.0.1", 9099, retry=6) as partyServer:
	partyServer.reconnect(udpLocalPort = 9100) #change local udpPort -> useful if server is local
	partyServer.sendData("message", "to someone", "the message")
	partyServer.sendData("position", 1.0, 2.0, 3.0)

	xyz = [0.0, 3.3, -1.34567]
	partyServer.sendData("position", *xyz)
	partyServer.sendData("spell", "I curses you")
	partyServer.sendData("alive", "3")
	partyServer.sendData("destination", 18.45678, 678.89)

	for i in xrange(0, 3):
	        partyServer.sendDataUdp("position", 1.0, 2.0, 42.4242)

	sleep(0.001)
	theList = partyServer.getData()
	for d in theList:
		print("type: {}", format(d["type"]))
		print(d["list"])

	partyServer.sendData("alive", "3")
