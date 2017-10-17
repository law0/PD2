#!/usr/bin/python

from panda3d.core import QueuedConnectionManager
from panda3d.core import ConnectionWriter
from panda3d.core import QueuedConnectionReader

from direct.task import Task

from panda3d.core import PointerToConnection
from panda3d.core import NetAddress
from panda3d.core import SocketAddress

from panda3d.core import NetDatagram

from direct.distributed.PyDatagram import PyDatagram
from direct.distributed.PyDatagramIterator import PyDatagramIterator

from time import sleep
from data import Data
from readingthread import ReadingThread
from datapool import DataPool

"""
Classe servant a la communication avec le server distant de party
Note le port udp local et distant sera le meme que le port distant tcp
pour changer le port local udp utilise reconnect(udpLocalPort = autreport)
(particulierement utile si le server est local, puisque le port udp local sera alors deja pris par le server)
"""
class PartyServer:
	def __init__(self, ip, port, timeout):
		self.id = 0
		self.address = NetAddress()
		self.address.setHost(ip, port)
		self.timeout = timeout
		self.coManager = QueuedConnectionManager()
		self.coWriter = ConnectionWriter(self.coManager,0)
		self.coReader = QueuedConnectionReader(self.coManager, 0)
		self.data = Data("info", 0)
		self.udpSocket = None
		self.tcpSocket = None
		self.dataPool = DataPool()
		self.readingThread = ReadingThread(self.coReader, self.dataPool)

	def __del__(self):
		self.stop()

	def __enter__(self):
		return self

	def __exit__(self, exc_type, exc_value, traceback):
		self.stop()

	def stop(self):
		if self.readingThread.isAlive():
			self.readingThread.stop()
			self.readingThread.join()
		self.coReader.removeConnection(self.tcpSocket)
		self.coReader.removeConnection(self.udpSocket)
		self.dataPool.clear()

	def printErrorCo(self, ip=None, port=None):
		if ip is None:
			ip = self.address.getIpString()
		if port is None:
			port = self.address.getPort()
		print("unable to connect to {} {}".format(ip, port))


	def reconnect(self, ip=None, port=None, timeout=None, udpLocalPort=None):
		if ip is not None:
			self.address.setHost(ip)

		if port is not None:
			self.address.setPort(port)

		if udpLocalPort is None:
			udpLocalPort = self.address.getPort()

		if timeout is not None:
			self.timeout = timeout

		self.udpSocket = self.coManager.openUDPConnection(udpLocalPort)
		if self.udpSocket is None:
			print("reconnect : can't open port udpPort for udp socket on port {}".format(self.address.getPort()))

		self.tcpSocket = self.coManager.openTCPClientConnection(self.address, self.timeout)
		if self.tcpSocket is None:
			self.printErrorCo()
		else:
			if self.tcpSocket.getAddress() is not None:
				self.coReader.addConnection(self.tcpSocket)
				if self.readingThread.isAlive():
					self.readingThread.stop()
					self.readingThread.join()
					self.readingThread = ReadingThread(self.coReader, self.dataPool)
				self.readingThread.start()

				if self.id == 0:
					self.data.reset("query")
					self.data.setData("id","")
					self.coWriter.send(self.data, self.tcpSocket)

				while self.id == 0:
					self.getData() #set Id via server responses

				if self.udpSocket is not None:
					self.coReader.addConnection(self.udpSocket)
					self.data.reset("info")
					self.data.setData("udpLocalPort", str(udpLocalPort))
					self.coWriter.send(self.data, self.tcpSocket)


	def send(self, data):
		if self.tcpSocket is None:
			print("connection is None")
			self.printErrorCo()
		else:
			self.coWriter.send(data, self.tcpSocket)

	def sendUdp(self, data):
		if self.udpSocket is None:
			print("connection is None")
			print("sendUdp : can't open port udpPort for udp socket on port {}".format(self.address.getPort()))
		else:
			self.coWriter.send(data, self.udpSocket, self.address)


	def sendData(self, typeStr, *args):
		if typeStr is not None:
			if typeStr != self.data.mtype:
				self.data.reset(typeStr)
			self.data.setData(*args)
			self.send(self.data)


	def sendDataUdp(self, typeStr, *args):
		if typeStr is not None:
			if typeStr != self.data.mtype:
				self.data.reset(typeStr)
			self.data.setData(*args)
			self.sendUdp(self.data)

	def getData(self): #must be called the most often possible
		buffer = self.dataPool.get()
		self.__filter(buffer)
		return buffer

	def __filter(self, buffer):
		to_delete=[]
		for i, payload in enumerate(buffer):
			if payload is None:
				to_delete.append(payload)
			elif payload["type"] == "id":
				self.id = payload["list"][0];
				self.data.setId(self.id)
				del buffer[i]
				to_delete.append(payload)


class connectToPartyServer:
	def __init__(self, ip, port, timeout=3000, retry=3, udpLocalPort=None):
		assert type(ip).__name__ == "str"
		assert type(port).__name__ == "int"
		assert type(timeout).__name__ == "int"
		self.udpLocalPort = port
		if udpLocalPort is not None:
			assert type(udpLocalPort).__name__ == "int"
			self.udpLocalPort = udpLocalPort
		self.ps = PartyServer(ip, port, timeout)
		self.retry = retry

	def __enter__(self):
		i = 0
		while self.ps.tcpSocket is None and i < self.retry:
			sleep(1)
			self.ps.reconnect(udpLocalPort=self.udpLocalPort)
			i = i+1

		return self.ps

	def __exit__(self, exc_type, exc_value, traceback):
		self.ps.stop()
		pst = self.ps
		self.ps = None
		del pst


####################################################################################################################


