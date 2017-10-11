#!/usr/bin/python

from panda3d.core import QueuedConnectionManager
from panda3d.core import ConnectionWriter
from panda3d.core import QueuedConnectionReader

from direct.task import Task

from panda3d.core import PointerToConnection
from panda3d.core import NetAddress

from panda3d.core import NetDatagram

from direct.distributed.PyDatagram import PyDatagram
from direct.distributed.PyDatagramIterator import PyDatagramIterator

from time import sleep

import dataprocess as services
from dataprocess import Data

#port_address=9099  # same for client and server

 # a valid server URL. You can also use a DNS name
 # if the server has one, such as "localhost" or "panda3d.org"
#ip_address="127.0.0.1"

 # how long until we give up trying to reach the server?
#timeout_in_miliseconds=3000  # 3 seconds



#myConnection=cManager.openTCPClientConnection(ip_address,port_address,timeout_in_miliseconds)
#if myConnection:
#	datagram = Data("message")
#	datagram.setData("to Bob", "testing message")
#	cWriter.send(datagram, myConnection)

#	sleep(2)

#	datagram.reinit("position")
#	datagram.setData(0.0, 1.0, 3.0)
#	cWriter.send(datagram, myConnection)

#	sleep(2)

#	datagram.setData(0.0, -1.0, 3.0)
#	cWriter.send(datagram, myConnection)

#	sleep(2)


class PartyServer:
	def __init__(self, ip, port, timeout=3000):
		self.ip = ip
		self.port = port
		self.timeout = timeout
		self.coManager = QueuedConnectionManager()
		self.coWriter = ConnectionWriter(self.coManager,0)
		self.coReader = QueuedConnectionReader(self.coManager,0)
		self.co = self.coManager.openTCPClientConnection(self.ip, self.port, self.timeout)


	def send(self, data):
		if not self.co:
			self.co = self.coManager.openTCPClientConnection(self.ip, self.port, self.timeout)

		if self.co:
			self.coWriter.send(data, self.co)
		else:
			print("unable to connect to {} {}".format(self.ip, self.port))


party = PartyServer("127.0.0.1", 9099)
packet = Data("message")
packet.setData("to someone", "the message")
party.send(packet)

packet.reinit("position")
packet.setData(1.0, 2.0, 3.0)
party.send(packet)

packet.setData(0.0, 2.0, 3.0)
party.send(packet)
