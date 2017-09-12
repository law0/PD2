#!/usr/bin/python

from direct.showbase.ShowBase import ShowBase

class APP(ShowBase):

	def __init__(self):
		ShowBase.__init__(self)
	
		self.scene = self.loader.loadModel("1")
		self.scene.reparentTo(self.render)
		self.scene.setPos(0,0,0)

app = APP()
app.run()
