#!/usr/bin/python

from panda3d.core import QueuedConnectionManager
from panda3d.core import QueuedConnectionListener
from panda3d.core import QueuedConnectionReader
from panda3d.core import ConnectionWriter

from direct.task import Task

from panda3d.core import PointerToConnection
from panda3d.core import NetAddress
 
from panda3d.core import NetDatagram

from direct.distributed.PyDatagram import PyDatagram
from direct.distributed.PyDatagramIterator import PyDatagramIterator

from time import sleep

cManager = QueuedConnectionManager()
cListener = QueuedConnectionListener(cManager, 0)
tcpReader = QueuedConnectionReader(cManager, 0)
udpReader = QueuedConnectionReader(cManager, 0)
cWriter = ConnectionWriter(cManager,0)

activeConnections=[] # We'll want to keep track of these later

end = True

def listenerPolling():
	if cListener.newConnectionAvailable(): 
		rendezvous = PointerToConnection()
		netAddress = NetAddress()
		newConnection = PointerToConnection() 
		if cListener.getNewConnection(rendezvous,netAddress,newConnection):
			newConnection = newConnection.p()
			activeConnections.append(newConnection) # Remember connection
			tcpReader.addConnection(newConnection)    # Begin reading connection


def readerPolling(reader):
	if reader.dataAvailable():
		datagram = NetDatagram()
		if reader.getData(datagram):
			processDatagram(datagram)

def processDatagram(data):
	iterator = PyDatagramIterator(data)
	s = iterator.getString()
	print(s)
	s2 = iterator.getString()
	print(s2)
	if iterator.getUint8() == 1:
		endItAll()

def endItAll():
	global activeConnections
	global end
	for aClient in activeConnections:
		tcpReader.removeConnection(aClient)

	activeConnections=[]

 
	# close down our listener
	cManager.closeConnection(tcpSocket)
	print("done")
	end = False

########################################################################

port_address=9099 #No-other TCP/IP services are using this port
backlog=1000 #If we ignore 1,000 connection attempts, something is wrong!
tcpSocket = cManager.openTCPServerRendezvous(port_address,backlog)
udpSocket = cManager.openUDPConnection(port_address + 1)
 
cListener.addConnection(tcpSocket)

udpReader.addConnection(udpSocket)

while(end):
	listenerPolling()
	readerPolling(tcpReader)
	readerPolling(udpReader)
	sleep(0.05)

