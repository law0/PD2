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
from datapool import DataPool
from threading import Thread
from threading import Lock
from random import randint


##################################################################################

class ServerThread(Thread):
	def __init__(self, port, backlog):
		Thread.__init__(self)
		self.port = port
		self.backlog = backlog

		self.coManager = QueuedConnectionManager()

		self.coListener = QueuedConnectionListener(self.coManager, 0)
		self.tcpReader = QueuedConnectionReader(self.coManager, 0)
		self.udpReader = QueuedConnectionReader(self.coManager, 0)
		self.coWriter = ConnectionWriter(self.coManager,0)

		self.tcpRdvSocket = self.coManager.openTCPServerRendezvous(self.port,self.backlog)
		self.udpSocket = self.coManager.openUDPConnection(self.port)

		self.coListener.addConnection(self.tcpRdvSocket)
		self.udpReader.addConnection(self.udpSocket)

		self.end = True

		self.actives = []
		self.dataPoolIn = DataPool()
		self.dataPoolOut = DataPool()
		self.functions = []

		self.idToConnection = {}

	def __enter__(self):
		self.start()
		return self

	def __exit__(self, exc_type, exc_value, traceback):
		self.stop()
		self.join()

	def get(self):
		data = self.dataPoolIn.get()
		self.__filter(data)
		return data

	def append(self, data):
		self.dataPoolOut.append(data)

	def stop(self):
		self.end = False

	def run(self):
		while(self.end):
			self.listenToNewConnections()
			self.readConnections()
			self.writeConnections()
			sleep(0.001)


	def __filter(self, data_list):
		to_delete = []
		for i, data in enumerate(data_list):
			if data is None:
				to_delete.append(data)
			elif data["type"] == "query" and data["list"][0] == "id": #identification
				newData = Data("id", 0)
				xid = randint(11, 10000000000) # todo
				print("setting id -> {}".format(xid))
				newData.setData(xid)
				if xid not in self.idToConnection:
					self.idToConnection[xid]={}
				self.idToConnection[xid]["tcp"] = data["connection"]
				self.coWriter.send(newData, data["connection"])
				to_delete.append(data)
			elif data["type"] == "info" and data["list"][0] == "udpLocalPort": #udpLocalPort (client side)
				xid = data["id"]
				assert xid in self.idToConnection, "udpLocalPort received before id set?"
				address = NetAddress(data["address"].getAddr())
				address.setPort(int(data["list"][1]))
				self.idToConnection[xid]["udp"] = address
				to_delete.append(data)

		for d in to_delete:
			data_list.remove(d)


	def listenToNewConnections(self):
		if self.coListener.newConnectionAvailable():
			rendezvous = PointerToConnection()
			netAddress = NetAddress()
			newConnection = PointerToConnection()
			if self.coListener.getNewConnection(rendezvous, netAddress, newConnection):
				newConnection = newConnection.p()
				print("new connection from {}".format(newConnection.getAddress().getIpString()))
				self.actives.append(newConnection)
				self.tcpReader.addConnection(newConnection)


	def readConnections(self):
		if self.tcpReader.dataAvailable():
			datagram = NetDatagram()
			if self.tcpReader.getData(datagram):
				self.processTcpDatagram(datagram)

		if self.udpReader.dataAvailable():
			datagram = NetDatagram()
			if self.udpReader.getData(datagram):
				self.processUdpDatagram(datagram)

	def processTcpDatagram(self, datagram):
		data = Data.getDataFromDatagram(datagram)
		if data is not None:
			self.dataPoolIn.append(data)

	def processUdpDatagram(self, datagram):
		data = Data.getDataFromDatagram(datagram)
		if data is not None:
			self.dataPoolIn.append(data)

	def writeConnections(self):
		data_list = self.dataPoolOut.get()
		for data in data_list:
			if data is None:
				continue
			elif data["pro"] == "udp":
				print("writing udp, port{} {}".format(self.idToConnection[data["id"]]["udp"].getPort(), data))
				self.coWriter.send(data["datagram"], self.udpSocket, self.idToConnection[data["id"]]["udp"])
			elif data["pro"] == "tcp":
				print("writing tcp : {}".format(data))
				self.coWriter.send(data["datagram"], self.idToConnection[data["id"]]["tcp"])





#############################################################################################################################

#respond with dict("pro" : "udp" or "tcp", "datagram": Data(...), "id" : id_of_player)

def response(pro="tcp", datagram=Data("alive", 0), id=1):
	return {"pro":pro, "datagram": datagram, "id": id}


