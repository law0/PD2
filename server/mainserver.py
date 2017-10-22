#!/usr/bin/python
# coding: utf8

from panda3d.core import QueuedConnectionManager
from panda3d.core import QueuedConnectionListener
from panda3d.core import QueuedConnectionReader
from panda3d.core import ConnectionWriter
from panda3d.core import PointerToConnection
from panda3d.core import NetAddress
from panda3d.core import NetDatagram
from direct.distributed.PyDatagram import PyDatagram
from direct.distributed.PyDatagramIterator import PyDatagramIterator
from time import sleep
import copy
from data import Data
from datapool import DataPool
from threading import Thread
from threading import Lock
from serverthread import ServerThread
from serverthread import response
from random import randint

#data : {"type","id","connection","address","list"}
#response(pro="tcp", datagram=Data("alive", 0), idToSendTo=1)
#############################################################################################################################

playerMax = 1
datagram = Data("alive", 0)
idToPlayerId = {} #interieurement ServerThread gere des id de connection. Cependant par securite ces id ne doivent pas etre communiquer aux autres player
playerIdToId = {} #il faut donc faire une traduction entre un nouveau playerId qui sera envoyé et désignera les autres players et les Id de connexion qui sont privés
lastPlayerIdTaken = 0 #server is 0, other player id from 1 to n
allready = False

def processdata(data):

	global playerMax
	global datagram
	global idToPlayerId
	global playerIdToId
	global lastPlayerIdTaken
	global allready

	currentPlayerId = 1
	theId = data["id"]
	theType = data["type"]
	theList = data["list"]
	theConnection = data["connection"]
	theAddress = data["address"]

	if theId not in idToPlayerId: #je connais pas cet id
		if theType == "ready" and lastPlayerIdTaken < playerMax: #ready == nouveau joueur and if not max player atteint
			lastPlayerIdTaken += 1
			idToPlayerId[data["id"]] = lastPlayerIdTaken
			playerIdToId[lastPlayerIdTaken] = data["id"]
			currentPlayerId = lastPlayerIdTaken
			allready = (lastPlayerIdTaken == playerMax)
			#print("player added")
		else:
			return
	else:
		currentPlayerId = idToPlayerId[data["id"]]

		if theType == "position":
			for i in range(1, lastPlayerIdTaken + 1):
				if i != currentPlayerId or playerMax == 1:
					datagram.reset("position", currentPlayerId)
					datagram.setData(*data["list"])
					threadServer.append(response(pro="udp", datagram=datagram, idToSendTo=playerIdToId[i]))

#		print("{} {} {}".format(theType, theId, theList))


if __name__ == "__main__":
	with ServerThread(9099, 1000) as threadServer:
		while True:
			data_list = threadServer.get()

			for data in data_list:
				processdata(data)

			if allready:
				#print("maxPlayer reached, sending ready")
				allready = False #on renvoie ready qu'une seule fois
				for id in idToPlayerId:
					datagram.reset("ready", 0) #zero = server
					datagram.setData(playerMax, idToPlayerId[id])
					threadServer.append(response(pro="tcp", datagram=datagram, idToSendTo=id))

			sleep(0.001) #no non-RT OS will be that precise, but it's just to be nice to other process
