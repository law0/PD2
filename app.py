#!/usr/bin/python

from math import pi, sin, cos
from direct.showbase.ShowBase import ShowBase

from party import PartyWorld
from champion import Champion

from panda3d.core import CollisionTraverser

class App(ShowBase, object):
	
	def __init__(self):
		ShowBase.__init__(self)

		#few import init:
		#disable cam control by mouse
		self.disableMouse()
		
		self.__currentWorldLoader = None
		self.__worldLoaders = {}	

	def addWorldLoader(self, name, worldLoader):
		self.__worldLoaders[name] = worldLoader

	def loadWorld(self, name):
		if self.__worldLoaders[name] is not None :
			if self.__currentWorldLoader is not None:
				self.__currentWorldLoader.unload()
			self.__currentWorldLoader = self.__worldLoaders[name]
			self.__currentWorldLoader.load()
		else:
			print "world loader " + name + "unknown"
		
	def launchWorld(self, name):
		if self.__worldLoaders[name] is not None :
			self.__currentWorldLoader.launch()
			print "launched"
		


