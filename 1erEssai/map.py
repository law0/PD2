#!/usr/bin/python

import app
from direct.showbase import DirectObject
from direct.task import Task
from panda3d.core import Point3
from panda3d.core import NodePath
from panda3d.core import CollisionNode
from panda3d.core import CollisionBox
from panda3d.physics import ActorNode
from panda3d.physics import ForceNode
from wrapper import Wrapper

from panda3d.core import KeyboardButton

from direct.actor.Actor import Actor

from usercontroller import UserController

#is not actor but a model (I don't have any animation yet)
class Map(DirectObject.DirectObject, Wrapper):

	def __init__(self):
		pass

	def load(self):
		print "loading map"

                self.map = base.loader.loadModel("1")
                self.map.setPos(0, 0, 0)
		#no physics
		#self.loadPhysics()

		self.loadCollisions()

		print "loaded map"

	def unload(self):

		#unfinished.....

		print "unloading map"
		self.map.removeNode()
		base.loader.unloadModel(self.map)
		self.map = None
		self.ignoreAll()

	def launch(self):
		self.map.reparentTo(base.render)
		
	def loadCollisions(self):
		#for collision
		self.collisionSolidNodePath = self.map.attachNewNode(CollisionNode("map_collision_node"))
                self.collisionSolidNodePath.node().addSolid(CollisionBox(Point3(0, 0, -1), 10, 10, 1))
		self.collisionSolidNodePath.show()
