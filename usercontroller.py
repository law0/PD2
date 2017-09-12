#!/usr/bin/python

from math import pi, cos, sin

import app
from direct.showbase import DirectObject
from direct.task import Task
from panda3d.core import LPoint3f
from panda3d.core import NodePath
from panda3d.physics import ActorNode
from panda3d.physics import ForceNode
from panda3d.physics import LinearVectorForce

from panda3d.core import KeyboardButton

#use to control movements of an object and camera on it
class UserController(DirectObject.DirectObject, object):

	def __init__(self, target, camera):

		#position de depart
		self.__x = 2
		self.__y = 3

		#keybindings
		self.forwardKey = KeyboardButton.ascii_key('z')
		self.backwardKey = KeyboardButton.ascii_key('s')
		self.rotLeftKey = KeyboardButton.ascii_key('q')
		self.rotRightKey = KeyboardButton.ascii_key('d')		

		#speed values
		self.speed = 0.05
		self.rotSpeed = 1.0

		#target and cam
		self.target = target
		self.camera = camera

		try: 
			self.target.setPos(self.__x, self.__y, 10)
			self.camera.reparentTo(self.target)
		except Exception as e:
			print "---> Exception! in __init__ user controller, maybe target or camera is None?"
			print str(e)
			exit()

		self.jumpNode = NodePath("jump")
		self.jumpNode.reparentTo(self.target)
		self.jumpNode.setPos(0,0,0)

                self.jumpForce = LinearVectorForce(0, 0, 300)
                self.jumpForceNode = ForceNode("jumpNode")
                self.jumpForceNode.addForce(self.jumpForce)
                jumpNodePath = self.jumpNode.attachNewNode(self.jumpForceNode)

		taskMgr.add(self.taskOnEachFrame, "taskOnEachFrame")
		taskMgr.add(self.taskOnFirstFrame, "taskOnFirstFrame")

		self.jumpStep = 0
		self.accept('space', self.jump)

	def taskOnFirstFrame(self, task):
		self.camera.setPos(0, -20, 10)
		self.camera.setHpr(0, -25, 0)
		taskMgr.remove("taskOnFirstFrame")
		return Task.done
		
	def taskOnEachFrame(self, task):
		self.__controlMove()
		self.target.setPos(self.__x, self.__y, self.target.getZ())

		if self.jumpStep == 1:
			self.target.node().getPhysical(0).addLinearForce(self.jumpForce)
			self.jumpStep = 2
		elif self.jumpStep == 2:
			self.target.node().getPhysical(0).removeLinearForce(self.jumpForce)
			self.jumpStep = 0

                return Task.cont

        def __controlMove(self):

		pressedKey = base.mouseWatcherNode.is_button_down

		#si 'droite' est presse
                if pressedKey(self.rotRightKey) :
			point = self.target.getHpr()
			point.addX(-self.rotSpeed)
			self.target.setHpr(point)
		
		#si 'gauche' est presse
                if pressedKey(self.rotLeftKey) :
			point = self.target.getHpr()
			point.addX(self.rotSpeed)
			self.target.setHpr(point)

		#si 'avant' est presse
                if pressedKey(self.forwardKey) :
			angle = self.target.getHpr().getX() * (pi / 180.0)
			self.__x -= self.speed * sin(angle)
			self.__y += self.speed * cos(angle)

		#si 'arriere' est presse
                if pressedKey(self.backwardKey) :
			angle = self.target.getHpr().getX() * (pi / 180.0)
			self.__x += self.speed * sin(angle)
			self.__y -= self.speed * cos(angle)

	def resetPos(self):		
		self.__x = 2
		self.__y = 3
		self.target.setPos(self.__x, self.__y, 10)


        def jump(self):
		self.jumpStep = 1

