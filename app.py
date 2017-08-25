#!/usr/bin/python

from math import pi, sin, cos
from direct.showbase.ShowBase import ShowBase

from party import PartyWorld
from champion import Champion

class App(ShowBase, object):
	
	def __init__(self):
		ShowBase.__init__(self)
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
			self.scene = self.loader.loadModel("/home/law/Documents/tests/PD2/1")
			self.scene.reparentTo(self.render)
			self.scene.setPos(0, 0, 0)
			print "launched"
		


