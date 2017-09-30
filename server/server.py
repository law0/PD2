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
cReader = QueuedConnectionReader(cManager, 0)
cWriter = ConnectionWriter(cManager,0)

activeConnections=[] # We'll want to keep track of these later

taskMgr = Task.TaskManager()

end = True

def tskListenerPolling(taskdata):
	if cListener.newConnectionAvailable(): 
		rendezvous = PointerToConnection()
		netAddress = NetAddress()
		newConnection = PointerToConnection() 
		if cListener.getNewConnection(rendezvous,netAddress,newConnection):
			newConnection = newConnection.p()
			activeConnections.append(newConnection) # Remember connection
			cReader.addConnection(newConnection)    # Begin reading connection
	c = Task.cont if end else Task.done
	return c


def tskReaderPolling(taskdata):
	if cReader.dataAvailable():
		datagram = NetDatagram()
		if cReader.getData(datagram):
			processDatagram(datagram)
	c = Task.cont if end else Task.done
	return c


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
		cReader.removeConnection(aClient)

	activeConnections=[]

 
	# close down our listener
	cManager.closeConnection(tcpSocket)
	print("done")
	end = False

########################################################################

port_address=9099 #No-other TCP/IP services are using this port
backlog=1000 #If we ignore 1,000 connection attempts, something is wrong!
tcpSocket = cManager.openTCPServerRendezvous(port_address,backlog)
 
cListener.addConnection(tcpSocket)

#taskMgr.add(tskListenerPolling,"Poll the connection listener",-39)
#taskMgr.add(tskReaderPolling,"Poll the connection reader",-40)

while(end):
	tskListenerPolling(None)
	tskReaderPolling(None)
	sleep(0.05)

