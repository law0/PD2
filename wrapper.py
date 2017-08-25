#!/usr/bin/python

class Wrapper(object):

	def __init__(self):
		pass

	#load and set translate and rotation
	def load(self):
		print "ahem, you somehow called the base class wrapper load function"

	#unload
	def unload(self):
		print "ahem, you somehow called the base class wrapper unload function"

	#basically call reparentTo
	def launch(self):
		print "ahem, you somehow called the base class wrapper launch function"

