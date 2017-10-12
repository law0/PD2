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

import dataprocess as services
from dataprocess import Data

"""
Classe servant a la communication avec le server distant de party
Note le port udp local et distant sera le meme que le port distant tcp
pour changer le port local udp utilise reconnect(udpLocalPort = autreport)
(particulierement utile si le server est local, puisque le port udp local sera alors deja pris par le server)
"""
class PartyServer:
	def __init__(self, ip, port, timeout):
		self.address = NetAddress()
		self.address.setHost(ip, port)
		self.timeout = timeout
		self.coManager = QueuedConnectionManager()
		self.coWriter = ConnectionWriter(self.coManager,0)
		self.coReader = QueuedConnectionReader(self.coManager,0)
		self.socket = self.coManager.openTCPClientConnection(self.address, self.timeout)
		self.udpSocket = self.coManager.openUDPConnection(self.address.getPort())

	def printErrorCo(self, ip=None, port=None):
		if ip is None:
			ip = self.address.getIpString()
		if port is None:
			port = self.address.getPort()
		print("unable to connect to {} {}".format(ip, port))

	def send(self, data):
		if self.socket is None:
			print("connection is None")
			self.printErrorCo()
		else:
			self.coWriter.send(data, self.socket)

	def sendUdp(self, data):
		if self.udpSocket is None:
			print("connection is None")
			print("sendUdp : can't open port udpPort for udp socket on port {}".format(self.address.getPort()))
		else:
			self.coWriter.send(data, self.udpSocket, self.address)


	def reconnect(self, ip=None, port=None, timeout=None, udpLocalPort=None):
		if ip is not None:
			self.address.setHost(ip)

		if port is not None:
			self.address.setPort(port)

		if udpLocalPort is None:
			udpLocalPort = self.address.getPort()

		if timeout is not None:
			self.timeout = timeout

		self.socket = self.coManager.openTCPClientConnection(self.address, self.timeout)
		if self.socket is None:
			self.printErrorCo()

		self.udpSocket = self.coManager.openUDPConnection(udpLocalPort)
		if self.udpSocket is None:
			print("reconnect : can't open port udpPort for udp socket on port {}".format(self.address.getPort()))



def connectToPartyServer(ip, port, timeout=3000, retry=3):
	assert type(ip).__name__ == "str"
	assert type(port).__name__ == "int"
	assert type(timeout).__name__ == "int"

	ps = PartyServer(ip, port, timeout)

	i = 0
	while ps.socket is None and i < retry:
		sleep(1)
		ps.reconnect()
		i = i+1

	return ps


####################################################################################################################


