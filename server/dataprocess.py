#!/usr/bin/python

from panda3d.core import QueuedConnectionReader
from panda3d.core import PointerToConnection
from panda3d.core import NetAddress
from panda3d.core import NetDatagram
from direct.distributed.PyDatagram import PyDatagram
from direct.distributed.PyDatagramIterator import PyDatagramIterator
from time import sleep

import constants

def isAuthenticated(data, players):
	p = players.get(data.getConnection())
	if p is not None:
		return p.username != ""
	else:
		return False


def isAuthorized(username):
        return username == "testuser"

def aliveTcp(data, iterator, players, tcpReader):
	print("alive tcp")
	user = iterator.getString()
	found = isAuthenticated(data, players)
	
	if found: #just an "alive"
		p = players.get(data.getConnection())
		p.ttl = constants.unactivity_ttl
	else: #authentication
		if isAuthorized(user):
			print("authenticating user {}".format(user))
			print(len(players))
			p = players.get(data.getConnection())
			if p is not None:
				p.username = user
				p.ttl = constants.unactivity_ttl
		else:
			print("unauthorized user: {}".format(user))
			co = data.getConnection()
			print("deleting {}".format(data.getConnection().getAddress().getIpString()))
			xmanager = co.getManager()
			tcpReader.removeConnection(co)
			xmanager.closeConnection(co)
			del players[co]
			print(len(players))


def messageTcp(data, iterator, players):
	print("message tcp")
	dest = iterator.getString()
	mesg = iterator.getString()
	if isAuthenticated(data, players):
		print("MESSAGE TO: {}".format(dest))
		print("MESSAGE CONTENT:")
		print(mesg)
		print("")
	else:
		print("unauthenticated message datagram")
		print("MESSAGE TO: {}".format(dest))
		print("MESSAGE CONTENT:")
		print(mesg)
		print("")
