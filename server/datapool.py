#!/usr/bin/python

from panda3d.core import QueuedConnectionReader
from panda3d.core import NetAddress
from panda3d.core import NetDatagram
from direct.distributed.PyDatagram import PyDatagram
from direct.distributed.PyDatagramIterator import PyDatagramIterator
from time import sleep
from data import Data
from threading import Thread
from threading import Lock

class DataPool():
	def __init__(self):
		self.buffer = []
		self.lock = Lock()

	def append(self, arg):
		with self.lock:
			self.buffer.append(arg)

	def get(self):
		bufferOut = []
		with self.lock:
			for i, data in enumerate(self.buffer):
				bufferOut.append(data)
				del self.buffer[i]
		return bufferOut

	def clear(self):
		with self.lock:
			del self.buffer[:]


class ReadingThread(Thread):
	def __init__(self, reader, dataPool):
		Thread.__init__(self)
		self.dataPool = dataPool
		self.reader = reader
		self.loop = True

	def run(self):
		self.loop = True
		while self.loop:
			if self.reader.dataAvailable():
				datagram = NetDatagram()
				if self.reader.getData(datagram):
					data_list = Data.getDataFromDatagram(datagram)
					self.dataPool.append(data_list)

	def stop(self):
		self.loop = False