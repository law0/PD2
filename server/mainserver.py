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
from serverthread import ServerThread
from serverthread import response


#############################################################################################################################

if __name__ == "__main__":
	with ServerThread(9099, 1000) as threadServer:
		datagram = Data("alive", 0)
		while True:
			data_list = threadServer.get()
			for data in data_list:
				print("{} {} {}".format(data["id"], data["type"], data["list"]))
				if data["type"] == "position":
					datagram.reset("info")
					datagram.setData("received", "position")
					newData = response(pro="udp", datagram=datagram, id=data["id"])
					threadServer.append(newData)
