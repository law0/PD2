#!/usr/bin/python

from math import pi, cos, sin

import app
from direct.showbase import DirectObject
from direct.task import Task
from panda3d.core import LPoint3f
from wrapper import Wrapper

from panda3d.core import KeyboardButton

from direct.actor.Actor import Actor

#is not actor but a model (I don't have any animation yet)
class Champion(DirectObject.DirectObject, Wrapper):

	def __init__(self):
		pass

	def load(self):
		print "loading body"
		self.body = base.loader.loadModel("models/teapot")
		self.__ax = 2
		self.__ay = 3
		self.body.setPos(self.__ax, self.__ay, 0)
		self.body.setScale(0.5,0.5,0.5)
		print "loaded body"

	def unload(self):
		print "unloading body"
		self.body.removeNode()
		base.loader.unloadModel(self.body)
		self.body = None
		self.ignoreAll()

	def launch(self):
		print "lauching body"
		self.fixCamera()

		self.forwardKey = KeyboardButton.ascii_key('z')
		self.backwardKey = KeyboardButton.ascii_key('s')
		self.rotLeftKey = KeyboardButton.ascii_key('q')
		self.rotRightKey = KeyboardButton.ascii_key('d')		

		taskMgr.add(self.taskOnEachFrame, "taskOnEachFrame")
		taskMgr.add(self.taskOnFirstFrame, "taskOnFirstFrame")

		self.body.reparentTo(base.render)
	
	def fixCamera(self):
		print "fixing camera on dummy body"
		self.camera = base.camera
		self.camera.reparentTo(self.body)
		taskMgr.add(self.taskOnFirstFrame, "taskOnFirstFrame")

	def unfixCamera(self):
		print "unfix camera"
		pass

	def taskOnFirstFrame(self, task):
		print "once"
		self.camera.setPos(0, -20, 15)
		self.camera.setHpr(0, -34, 0)
		taskMgr.remove("taskOnFirstFrame")
		return Task.done
		
	def taskOnEachFrame(self, task):
		self.__controlMove()
		self.body.setPos(self.__ax, self.__ay, 0)
                return Task.cont

        def __controlMove(self):
		speed = 0.05
		rotSpeed = 1.0

		pressedKey = base.mouseWatcherNode.is_button_down

		#si 'droite' est presse
                if pressedKey(self.rotRightKey) :
			point = self.body.getHpr()
			point.addX(-rotSpeed)
			self.body.setHpr(point)
		
		#si 'gauche' est presse
                if pressedKey(self.rotLeftKey) :
			point = self.body.getHpr()
			point.addX(rotSpeed)
			self.body.setHpr(point)

		#si 'avant' est presse
                if pressedKey(self.forwardKey) :
			angle = self.body.getHpr().getX() * (pi / 180.0)
			self.__ax -= speed * sin(angle)
			self.__ay += speed * cos(angle)

		#si 'arriere' est presse
                if pressedKey(self.backwardKey) :
			angle = self.body.getHpr().getX() * (pi / 180.0)
			self.__ax += speed * sin(angle)
			self.__ay -= speed * cos(angle)
