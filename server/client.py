#!/usr/bin/python

from panda3d.core import QueuedConnectionManager
from panda3d.core import ConnectionWriter

from direct.task import Task

from panda3d.core import PointerToConnection
from panda3d.core import NetAddress

from panda3d.core import NetDatagram

from direct.distributed.PyDatagram import PyDatagram
from direct.distributed.PyDatagramIterator import PyDatagramIterator

from time import sleep

import constants

port_address=9099  # same for client and server
 
 # a valid server URL. You can also use a DNS name
 # if the server has one, such as "localhost" or "panda3d.org"
ip_address="127.0.0.1"
 
 # how long until we give up trying to reach the server?
timeout_in_miliseconds=3000  # 3 seconds
 
cManager = QueuedConnectionManager()
cWriter = ConnectionWriter(cManager,0)

myConnection=cManager.openTCPClientConnection(ip_address,port_address,timeout_in_miliseconds)
if myConnection:
	datagram = PyDatagram()
	datagram.addUint8(constants.dict_s_i["message"]) #unauthenticated should not work
	datagram.addString("test destinataire 0")
	datagram.addString("test message 0")
	cWriter.send(datagram, myConnection)
	datagram.clear()

	sleep(2)

	datagram.addUint8(constants.dict_s_i["alive"]) #bad username should not work
	datagram.addString("law")
	cWriter.send(datagram, myConnection)
	datagram.clear()

sleep(2)

myConnection=cManager.openTCPClientConnection(ip_address,port_address,timeout_in_miliseconds)
if myConnection:
	datagram.addUint8(constants.dict_s_i["alive"])
	datagram.addString("testuser")	
	cWriter.send(datagram, myConnection)
	datagram.clear()

	sleep(2)

	datagram.addUint8(constants.dict_s_i["message"])
	datagram.addString("test destinataire 1")
	datagram.addString("test message 1")
	cWriter.send(datagram, myConnection)
	datagram.clear()
	
