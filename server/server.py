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
from data import Data

xmanager = QueuedConnectionManager()
xlistener = QueuedConnectionListener(xmanager, 0)
tcpReader = QueuedConnectionReader(xmanager, 0)
udpReader = QueuedConnectionReader(xmanager, 0)
xwriter = ConnectionWriter(xmanager,0)

port_address=9099
backlog=1000
tcpSocket = xmanager.openTCPServerRendezvous(port_address,backlog)
udpSocket = xmanager.openUDPConnection(port_address)

xlistener.addConnection(tcpSocket)
udpReader.addConnection(udpSocket)

end = True

actives = []
data_list = []

def listenToNewConnections():
	if xlistener.newConnectionAvailable():
		rendezvous = PointerToConnection()
		netAddress = NetAddress()
		newConnection = PointerToConnection()
		if xlistener.getNewConnection(rendezvous, netAddress, newConnection):
			newConnection = newConnection.p()
			print("new connection from {}".format(newConnection.getAddress().getIpString()))
			actives.append(newConnection)
			tcpReader.addConnection(newConnection)


def readConnections():
	if tcpReader.dataAvailable():
		datagram = NetDatagram()
		if tcpReader.getData(datagram):
			processTcpDatagram(datagram)

	if udpReader.dataAvailable():
		datagram = NetDatagram()
		if udpReader.getData(datagram):
			processUdpDatagram(datagram)

def isAuthorized(username):
	return username == "testuser"

def processTcpDatagram(datagram):
	print("process tcp datagram")
	data = Data.getDataFromDatagram(datagram)
	print("from id: {} -> {}: {}".format(data["id"], data["type"], data["list"]))
	data_list.append(data)

def processUdpDatagram(datagram):
	print("process udp datagram")
	data = Data.getDataFromDatagram(datagram)
	print("from id: {} -> {}: {}".format(data["id"], data["type"], data["list"]))
	data_list.append(data)

def writeConnections():
	for d in data_list:
		if d["type"] == "query" and d["list"][0] == "id":
			print("WRITING DATA")
			newData = Data("id", 0)
			newData.setData(42)
			xwriter.send(newData, d["connection"])

def deleteHistory():
	del data_list[:]


########################################################################


while(end):
	listenToNewConnections()
	readConnections()
	writeConnections()
	deleteHistory()
#	checkInactives()
	sleep(0.01)

