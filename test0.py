#!/usr/bin/python

from math import pi, sin, cos
from direct.showbase.ShowBase import ShowBase
from direct.task import Task
from direct.actor.Actor import Actor

class App(ShowBase, object):
	
	def __init__(self):
		ShowBase.__init__(self)

		self.__model = self.loader.loadModel("models/teapot")
		self.__model.reparentTo(self.render)
		self.__model.setScale(0.5, 0.5, 0.5)
		self.__ax = 2.0
		self.__ay = 3.0
		self.__model.setPos(self.__ax, self.__ay, 0)
	
		# Load the environment model.
	        self.scene = self.loader.loadModel("1")
        	# Reparent the model to render.
	        self.scene.reparentTo(self.render)
	        # Apply scale and position transforms on the model.
        	#self.scene.setScale(0, 0, 0)
	        self.scene.setPos(0, 0, 0)

		#self.taskMgr.add(self.cameraTask, "CameraTask")
		self.taskMgr.add(self.__updatePos, "updatePos")

		#self.panda = Actor("models/panda-model", {"anim1" : "models/panda-walk4"})
		#self.panda.setScale(0.005, 0.005, 0.005)
		#self.panda.reparentTo(self.render)

		#self.panda.loop("anim1")
	

		self.accept('z', self.__inputKeyboard, ['z'])
		self.accept('s', self.__inputKeyboard, ['s'])
		self.accept('q', self.__inputKeyboard, ['q'])
		self.accept('d', self.__inputKeyboard, ['d'])
		self.accept('z-repeat', self.__inputKeyboard, ['z'])
		self.accept('s-repeat', self.__inputKeyboard, ['s'])
		self.accept('q-repeat', self.__inputKeyboard, ['q'])
		self.accept('d-repeat', self.__inputKeyboard, ['d'])

		self.camera.reparentTo(self.__model)

	def __inputKeyboard(self, key):
		if key == 'd':
			self.__ax = self.__ax + 0.5
		elif key == 'q':
			self.__ax = self.__ax - 0.5
		elif key == 'z':
			self.__ay = self.__ay + 0.5
		elif key == 's':
			self.__ay = self.__ay - 0.5
		else:
			pass

	def __updatePos(self, task):
		self.__model.setPos(self.__ax, self.__ay, 0)
		self.camera.setPos(0, -20, 7)
		self.camera.setHpr(0, -10, 0)
		print "doing : {} {}".format(self.__ax, self.__ay)
		return Task.cont

	def cameraTask(self, task):
		#angleDeg = task.time * 50.0
		#angleRad = angleDeg * (pi / 180.0)
		#self.camera.setPos(20.0 * sin(angleRad), -20.0 * cos(angleRad), 3)
		#print self.camera.getPos()
		#self.camera.setHpr(angleDeg, 0, 0)

		self.camera.setPos(-6, -18, 3)
		return Task.cont

app = App()
app.run()
