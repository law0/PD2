#!/usr/bin/python

class Wrapper(object):

	def __init__(self):
		pass

	#load and set translate and rotation
	def load(self):
		print "ahem, you somehow called the base class wrapper load function"
		self.loadPhysics()
		self.loadCollisions()

	#unload
	def unload(self):
		print "ahem, you somehow called the base class wrapper unload function"

	#basically call reparentTo
	def launch(self):
		print "ahem, you somehow called the base class wrapper launch function"


	def loadPhysics(self):
		print "ahem, you somehow called the base class wrapper loadPhysics function"

	def loadCollisions(self):
		print "ahem, you somehow called the base class wrapper loadCollisions function"
