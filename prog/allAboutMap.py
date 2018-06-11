from direct.showbase.ShowBase import ShowBase
import os
 
 
existingMap= ["dracula","frankesintein", "pirateage"] #liste des maps existantent peut etre faire des dictionnaires afin de les lie a un id ? 


def mapChoice(): #fonction qui permet de choisir la map 
		
	mapID = input("choix de la carte:")
	
	if mapID not in existingMap:	
		print("erreur, carte non existante")
		return 
	else : 
		typeMapID = type(mapID)

		print("la carte choisi est la carte numero ", mapID , "et son type est :",typeMapID)
		return mapID
	
def addMap():

		
'''class myMap(ShowBase):
	mapID=0
	if mapID == 1:
	
		def __init__(self):
			ShowBase.__init__(self)
			# Load the environment model.
			self.scene = self.loader.loadModel("models/environment")
			# Reparent the model to render.
			self.scene.reparentTo(self.render)
			# Apply scale and position transforms on the model.
			self.scene.setScale(0.1, 0.1, 0.1)
			self.scene.setPos(-8, 42, 0)
 

		
		
app = myMap()
app.run()'''