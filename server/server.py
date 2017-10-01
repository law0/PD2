#!/usr/bin/python

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
import dataprocess as funcs
import constants

class Player():
	def __init__(self):
		self.username = ""
		self.id = 0
		self.ttl = constants.unactivity_ttl

xmanager = QueuedConnectionManager() 
xlistener = QueuedConnectionListener(xmanager, 0) 
tcpReader = QueuedConnectionReader(xmanager, 0)
udpReader = QueuedConnectionReader(xmanager, 0) 
xwriter = ConnectionWriter(xmanager,0)

port_address=9099
backlog=1000 
tcpSocket = xmanager.openTCPServerRendezvous(port_address,backlog) 
udpSocket = xmanager.openUDPConnection(port_address + 1)

xlistener.addConnection(tcpSocket)
udpReader.addConnection(udpSocket)

np = 2
players = {}
end = True

def listenerPolling():
	if xlistener.newConnectionAvailable(): 
		rendezvous = PointerToConnection()
		netAddress = NetAddress()
		newConnection = PointerToConnection() 
		if xlistener.getNewConnection(rendezvous, netAddress, newConnection):
			newConnection = newConnection.p()
			print("new connection from {}".format(newConnection.getAddress().getIpString()))
			players[newConnection] = Player()
			print(len(players))
			tcpReader.addConnection(newConnection)


def readerPolling(reader):
	if reader.dataAvailable():
		datagram = NetDatagram()
		if reader.getData(datagram):
			if reader == tcpReader:
				processTcpDatagram(datagram)
			elif reader == udpReader:
				processUdpDatagram(datagram)

def isAuthorized(username):
	return username == "testuser"

def processTcpDatagram(data):
	print("process tcp datagram")
	iterator = PyDatagramIterator(data)
	data_type = constants.dict_i_s.get(iterator.getUint8())

	if data_type is not None:
		if data_type == "alive":
			funcs.aliveTcp(data, iterator, players, tcpReader)
		elif data_type == "message":
			funcs.messageTcp(data, iterator, players)		
	else:
		print("unknown data_type for datagram: ")
		print(data)
	
def processUdpDatagram(data):
	pass

def endItAll():
	global players
	global end
	for p in players:
		tcpReader.removeConnection(p.connection)

	players = []


	# close down our listener
	xmanager.closeConnection(tcpSocket)
	xmanager.closeConnection(udpSocket)
	print("done")
	end = False

def checkInactives():
	global players
	for item in players.items():
		co = item[0]
		p = item[1]
		if p.ttl <= 0:
			print(len(players))
			print("deleting player : {}".format(p.username))
			tcpReader.removeConnection(co)
			del players[co]
		else:		
			p.ttl -= 10
		

########################################################################


while(end):
	listenerPolling()
	readerPolling(tcpReader)
	readerPolling(udpReader)
	checkInactives()
	sleep(0.01)

