#!/usr/bin/python

from math import pi, cos, sin

import app
from direct.showbase import DirectObject
from direct.task import Task
from panda3d.core import LPoint3f

from panda3d.core import KeyboardButton

#use to control movements of an object and camera on it
class UserController(DirectObject.DirectObject, object):

	def __init__(self, body, camera):

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

		#body and cam
		self.body = body
		self.camera = camera

		try: 
			self.body.getScale()
			self.camera.reparentTo(self.body)
		except Exception as e:
			print "---> Exception! in __init__ user controller, maybe body or camera is None?"
			print str(e)
			exit()

		taskMgr.add(self.taskOnEachFrame, "taskOnEachFrame")
		taskMgr.add(self.taskOnFirstFrame, "taskOnFirstFrame")


	def taskOnFirstFrame(self, task):
		self.camera.setPos(0, -20, 10)
		self.camera.setHpr(0, -25, 0)
		taskMgr.remove("taskOnFirstFrame")
		return Task.done
		
	def taskOnEachFrame(self, task):
		self.__controlMove()
		self.body.setPos(self.__x, self.__y, 0)
                return Task.cont

        def __controlMove(self):

		pressedKey = base.mouseWatcherNode.is_button_down

		#si 'droite' est presse
                if pressedKey(self.rotRightKey) :
			point = self.body.getHpr()
			point.addX(-self.rotSpeed)
			self.body.setHpr(point)
		
		#si 'gauche' est presse
                if pressedKey(self.rotLeftKey) :
			point = self.body.getHpr()
			point.addX(self.rotSpeed)
			self.body.setHpr(point)

		#si 'avant' est presse
                if pressedKey(self.forwardKey) :
			angle = self.body.getHpr().getX() * (pi / 180.0)
			self.__x -= self.speed * sin(angle)
			self.__y += self.speed * cos(angle)

		#si 'arriere' est presse
                if pressedKey(self.backwardKey) :
			angle = self.body.getHpr().getX() * (pi / 180.0)
			self.__x += self.speed * sin(angle)
			self.__y -= self.speed * cos(angle)
