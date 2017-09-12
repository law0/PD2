#!/usr/bin/python

import app
from direct.showbase import DirectObject
from wrapper import Wrapper
from champion import Champion
from map import Map

from panda3d.physics import ForceNode
from panda3d.physics import LinearVectorForce
from panda3d.physics import PhysicsCollisionHandler
from panda3d.core import Plane
from panda3d.core import CollisionPlane
from panda3d.core import Vec3
from panda3d.core import Point3
from panda3d.core import CollisionTraverser
from panda3d.core import LPoint3f
from panda3d.core import NodePath
from panda3d.core import CollisionNode
from panda3d.core import CollisionHandlerEvent

class PartyWorld(DirectObject.DirectObject, Wrapper):

	def __init__(self):
		pass
	
	def load(self):
		print "loading world"

		#enable physics & collisions
                base.enableParticles()
                base.cTrav = CollisionTraverser('traverser')
                base.cTrav.showCollisions(base.render) #show debug

		self.skybox = base.loader.loadModel("skybox")
		self.skybox.set_two_sided(True)
		self.skybox.setPos(0, 0, 0)

		self.map = Map()
		self.map.load()		

		self.champion = Champion()
		self.champion.load()

		self.loadPhysics()

		self.loadCollisions()

		print "loaded world"

	def unload(self):

		#unfinished.......

		print "unloading world"
		self.champion.unload()
		self.map.removeNode()
		self.skybox.removeNode()
		base.loader.unloadModel(self.map)
		base.loader.unloadModel(self.skybox)
		self.map = None

	def launch(self):
		print "lauching world"
		self.skybox.reparentTo(base.render)

		self.map.launch()
		
		self.champion.setControllerType("user_controller")
		self.champion.launch()

	def loadPhysics(self):

		#build gravity
		self.gravity = LinearVectorForce(0, 0, -9.81)
		self.gravityForceNode = ForceNode("gravity")
		self.gravityForceNode.addForce(self.gravity)

		#makes the gravity relative to the entire world (attach the node to render)
		self.gravityForceNodePath = base.render.attachNewNode(self.gravityForceNode)

		#apply it to every ActorNode by adding it to the base physicsMgr rather than just one actorNode
		base.physicsMgr.addLinearForce(self.gravity)

	def loadCollisions(self):

		print "loading world collisions"

		#set deathFloor
		self.deathPlaneNodePath = render.attachNewNode(CollisionNode('deathPlane'))
		self.deathPlaneNodePath.node().addSolid(CollisionPlane(Plane(Vec3(0, 0, 1), Point3(0, 0, -10))))
		self.deathPlaneNodePath.show()

		#set reaction when collide with map
		self.physicsPusher = PhysicsCollisionHandler()
		self.physicsPusher.addCollider(self.champion.collisionSolidNodePath, self.champion.actorNodePath)

		#set reaction when collide with deathFloor
		self.eventPusher = CollisionHandlerEvent()
		self.eventPusher.addInPattern("%fn-into-%in")
		self.accept("champion_death_detector-into-deathPlane", self.onChampionDead)

		base.cTrav.addCollider(self.champion.collisionSolidNodePath, self.physicsPusher)
		base.cTrav.addCollider(self.champion.colDeathSolid, self.eventPusher)

	def onChampionDead(self, dafuk):
		self.champion.controller.resetPos()
		print dafuk
