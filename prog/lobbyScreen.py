from direct.showbase.ShowBase import ShowBase
from direct.showbase.DirectObject import DirectObject
from direct.gui.OnscreenText import OnscreenText, TextNode
from direct.gui.DirectGui import *
from gameScreen import *

 
class launchScreen(ShowBase,DirectObject):
	'''all about lobbyscreen'''


	gameStart = 0
		
	def __init__(self):
		'''POC version of the lobby , only have button that launch the game screen'''
		ShowBase.__init__(self)
		
		# Callback function to set  text
		self.bk_text = "This is my Demo"
		
		self.textObject = OnscreenText(text = self.bk_text, pos = (0.95,-0.95), 
		scale = 0.07,fg=(1,0.5,0.5,1),align=TextNode.ACenter,mayChange=1)
		
		self.b = DirectButton(text = ("OK", "click!", "rolling over", "disabled"),
		scale=(0.5),text_scale=(0.05,0.05), frameSize = (-0.5, 0.5, -0.1, 0.1),
		command = self.setText)
		
	def setText(self):
		self.bk_text = "Button Clicked"
		self.textObject.setText(self.bk_text)
		gameStart = 1
		return self.gameStart
		newGame = MyApp()
		newGame.run()

	
	

 