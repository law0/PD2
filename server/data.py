#!/usr/bin/python

from panda3d.core import QueuedConnectionReader
from panda3d.core import PointerToConnection
from panda3d.core import NetAddress
from panda3d.core import NetDatagram
from direct.distributed.PyDatagram import PyDatagram
from direct.distributed.PyDatagramIterator import PyDatagramIterator
from time import sleep

unactivity_ttl = 5000 # 5 seconds

datadict = {"query": ("str", "str",),
		"alive" : ("str",),
		"id" : ("int",),
		"info" : ("str", "str",),
		"message": ("str", "str",),
		"position" : ("float", "float", "float",),
		"destination" : ("float", "float",),
		"spell" : ("str",) #forgetting ending comma in one-element tuple makes it a non-tuple!
		}

def checkAssert(str, id):
	assert datadict[str] is not None, "message type " + str + " unfound in datadict"
	assert type(id).__name__ == "int", "id must be an int"


class Data(PyDatagram):
	def __init__(self, str, id, *args, **kwargs):
		PyDatagram.__init__(self, *args, **kwargs)
		checkAssert(str, id)
		self.mtype = str
		self.datatypes = datadict[str]
		self.id = id

	def reset(self, str, id=None):
		self.clear()
		if id is None:
			id = self.id
		checkAssert(str, id)
		self.mtype = str
		self.datatypes = datadict[str]

	def setId(self, id):
		if id is not None:
			checkAssert(self.mtype, id)
			self.id = id

	def setData(self, *args):
		self.clear()
		self.addString(self.mtype)
		self.addUint64(self.id)
		for i, arg in enumerate(args):
			assert type(arg).__name__ == self.datatypes[i], 'addData argument: {} is of wrong type for message type {}\
									\nPossible types are: {}\
									\nwhile arguments types are {}'\
									.format(arg, self.mtype, self.datatypes, [type(a).__name__ for a in args])

			if self.datatypes[i] == "str":
				self.addString(arg)
			elif self.datatypes[i] == "int":
				self.addInt64(arg)
			elif self.datatypes[i] == "float":
				self.addFloat64(arg)


	@staticmethod
	def getDataFromDatagram(datagram):
		assert type(datagram).__name__ == "NetDatagram"
		iterator = PyDatagramIterator(datagram)
		retdic = None
		try:
			theType = iterator.getString()
			theId = iterator.getUint64()
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

			retdic = {"type": theType, "id": theId, "connection": datagram.getConnection(), "address": datagram.getAddress(), "list": ret_list}
		except AssertionError as e:
			print("AssertionError caught: {} {}".format(e.errno, e.strerror))
		finally:
			return retdic
