#!/usr/bin/python

from app import App
from party import PartyWorld

def run():
	app = App()
	app.addWorldLoader("dummy", PartyWorld())
	app.loadWorld("dummy")
	app.launchWorld("dummy")
	app.run()

if __name__ == "__main__" :
	run()
