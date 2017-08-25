#!/usr/bin/python

import app
from direct.showbase import DirectObject
from wrapper import Wrapper
from champion import Champion

class PartyWorld(DirectObject.DirectObject, Wrapper):

	def __init__(self):
		pass
	
	def load(self):
		print "loading world"
		self.world = base.loader.loadModel("1")
		self.world.setPos(0, 0, 0)
		self.champion = Champion()
		self.champion.load()
		print "loaded world"

	def unload(self):
		print "unloading world"
		self.champion.unload()
		self.world.removeNode()
		base.loader.unloadModel(self.world)
		self.world = None

	def launch(self):
		print "lauching world"
		self.world.reparentTo(base.render)
		self.champion.launch()
 
