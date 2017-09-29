#!/usr/bin/python

from math import pi, sin, cos
from direct.showbase.ShowBase import ShowBase
from direct.task import Task
from direct.actor.Actor import Actor

class App(ShowBase):
	
	def __init__(self):
		ShowBase.__init__(self)
	
		# Load the environment model.
	        self.scene = self.loader.loadModel("1")
        	# Reparent the model to render.
	        self.scene.reparentTo(self.render)
	        # Apply scale and position transforms on the model.
        	#self.scene.setScale(0, 0, 0)
	        self.scene.setPos(0, 0, 0)

		self.taskMgr.add(self.cameraTask, "CameraTask")

		self.panda = Actor("models/panda-model", {"anim1" : "models/panda-walk4"})
		self.panda.setScale(0.005, 0.005, 0.005)
		self.panda.reparentTo(self.render)

		self.panda.loop("anim1")

	def cameraTask(self, task):
		angleDeg = task.time * 6.0
		angleRad = angleDeg * (pi / 180.0)
		self.camera.setPos(20.0 * sin(angleRad), -20.0 * cos(angleRad), 3)
		self.camera.setHpr(angleDeg, 0, 0)
		return Task.cont

dir(App)

app = App()
app.run()
