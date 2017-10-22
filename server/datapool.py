#!/usr/bin/python
# coding: utf8

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
			for data in self.buffer:
				bufferOut.append(data)
			del self.buffer[:]

		return bufferOut

	def clear(self):
		with self.lock:
			del self.buffer[:]
