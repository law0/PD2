#!/usr/bin/python

import app
from direct.showbase import DirectObject
from direct.task import Task
from panda3d.core import LPoint3f
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
		self.body = base.loader.loadModel("teapot")
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
		self.body.reparentTo(base.render)
		
		self.controller = UserController(self.body, base.camera)
	
