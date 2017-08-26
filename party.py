#!/usr/bin/python

import app
from direct.showbase import DirectObject
from wrapper import Wrapper
from champion import Champion

from panda3d.physics import ForceNode
from panda3d.physics import LinearVectorForce


class PartyWorld(DirectObject.DirectObject, Wrapper):

	def __init__(self):
		pass
	
	def load(self):
		print "loading world"

		self.map = base.loader.loadModel("1")
		self.map.setPos(0, 0, 0)

		self.skybox = base.loader.loadModel("skybox")
		self.skybox.set_two_sided(True)
		self.skybox.setPos(0, 0, 0)		

		self.champion = Champion()
		self.champion.load()

		#makes the gravity relative to the entire world (attach to render)
		self.gravityForceNode = ForceNode("gravity")
		self.gravityForceNodePath = base.render.attachNewNode(self.gravityForceNode)
		self.gravity = LinearVectorForce(0, 0, -9.81)
		self.gravityForceNode.addForce(self.gravity)

		#apply it to everyone (every actorNode)
		base.physicsMgr.addLinearForce(self.gravity)

		print "loaded world"

	def unload(self):
		print "unloading world"
		self.champion.unload()
		self.map.removeNode()
		self.skybox.removeNode()
		base.loader.unloadModel(self.map)
		base.loader.unloadModel(self.skybox)
		self.map = None

	def launch(self):
		print "lauching world"
		self.map.reparentTo(base.render)
		self.skybox.reparentTo(base.render)
		
		self.champion.setControllerType("user_controller")
		self.champion.launch()
 
