#!/usr/bin/python

import app
from direct.showbase import DirectObject
from direct.task import Task
from panda3d.core import LPoint3f
from panda3d.core import NodePath
from panda3d.core import CollisionNode
from panda3d.core import CollisionSphere
from panda3d.physics import ActorNode
from panda3d.physics import ForceNode
from wrapper import Wrapper

from panda3d.core import KeyboardButton

from direct.actor.Actor import Actor

from usercontroller import UserController

#is not actor but a model (I don't have any animation yet)
class Champion(DirectObject.DirectObject, Wrapper):

	def __init__(self):
		pass

	def load(self):
		print "loading body"

		self.nodePath = NodePath("champion_base_node_path")

		self.body = base.loader.loadModel("teapot")
		self.body.setScale(0.5,0.5,0.5)

		self.loadPhysics()

		self.loadCollisions()

		#make the physics node path parent of body
		self.body.reparentTo(self.actorNodePath)

		print "loaded body"

	def unload(self):

		#unfinished.....

		print "unloading body"
		self.body.removeNode()
		base.loader.unloadModel(self.body)
		self.body = None
		self.ignoreAll()

	def launch(self):
		if self.controller is not None:
			print "lauching champion"
			self.nodePath.reparentTo(base.render)
		else:
			print "can't launch: controller missing"
		
	def setControllerType(self, name):
		self.controller = None
		self.controllerType = name
		if name == "user_controller":
			self.controller = UserController(target=self.actorNodePath, camera=base.camera)
		elif name == "net_controller":
			#in the future : controller from internet here
			#self.controller = NetController(...)
			pass
		else:
			pass

	def loadPhysics(self):
		#for physics
		self.actorNode = ActorNode("champion_physics_node")
		self.actorNodePath = self.nodePath.attachNewNode(self.actorNode)
		base.physicsMgr.attachPhysicalNode(self.actorNode)

	def loadCollisions(self):
		#for collision
		self.collisionSolidNodePath = self.actorNodePath.attachNewNode(CollisionNode("champion_collision_node"))
                self.collisionSolidNodePath.node().addSolid(CollisionSphere(0, 0, 0.5, 1))
		self.collisionSolidNodePath.show()

		self.colDeathSolid = self.actorNodePath.attachNewNode(CollisionNode("champion_death_detector"))
		intangibleSphere = CollisionSphere(0, 0, 0.5, 1)
		intangibleSphere.setTangible(False)
                self.colDeathSolid.node().addSolid(intangibleSphere)
