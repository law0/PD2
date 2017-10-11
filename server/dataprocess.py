#!/usr/bin/python

from panda3d.core import QueuedConnectionReader
from panda3d.core import PointerToConnection
from panda3d.core import NetAddress
from panda3d.core import NetDatagram
from direct.distributed.PyDatagram import PyDatagram
from direct.distributed.PyDatagramIterator import PyDatagramIterator
from time import sleep

unactivity_ttl = 5000 # 5 seconds

datadict = {"connect": ("str"),
		"alive" : ("str"),
		"id" : ("int"),
		"info" : ("str"),
		"message": ("str", "str"),
		"position" : ("float", "float", "float"),
		"destination" : ("float", "float", "float"),
		"spell" : ("str"),
		}

class Data(PyDatagram):
	def __init__(self, str, *args, **kwargs):
		PyDatagram.__init__(self, *args, **kwargs)
		assert datadict[str] is not None, "message type " + str + " unfound in datadict"
		self.mtype = str
		self.datatypes = datadict[str]

	def reinit(self, str):
		self.clear()
		assert datadict[str] is not None, "message type " + str + " unfound in datadict"
		self.mtype = str
		self.datatypes = datadict[str]

	def setData(self, *args):
		self.clear()
		self.addString(self.mtype)
		i = 0
		for arg in args:
			assert type(arg).__name__ == self.datatypes[i], "addData argument: " + arg + " is of wrong type for message type " + self.mtype
			if self.datatypes[i] == "str":
				self.addString(arg)
			elif self.datatypes[i] == "int":
				self.addInt64(arg)
			elif self.datatypes[i] == "float":
				self.addFloat64(arg)
			i = i+1

	@staticmethod
	def getDataFromDatagram(datagram):
		iterator = PyDatagramIterator(datagram)
		theType = iterator.getString()
		assert datadict[theType] is not None, "message type " + theType + " unfound in datadict"
		datatypes = datadict[theType]
		ret_list = []
		for t in datatypes:
			if t == "str":
				ret_list.append(iterator.getString())
			elif t == "int":
				ret_list.append(iterator.getInt64())
			elif t == "float":
				ret_list.append(iterator.getFloat64())

		return ret_list
