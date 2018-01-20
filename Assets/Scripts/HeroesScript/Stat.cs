using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public delegate void StatCallback(float v);


//class Stat. Le respect des limites entre min et max est automatique : add(25) sur une stat qui est a 100 et ayant pour max 120 ne montera la stat que jusqu'a 120
//+ system de callback. Pour ajouter une fonction callback a appeler a chaque fois que la valeur change, il suffit de faire stat.callbackList.Add(function_name)
//la signature de la fonction de callback doit etre "public void function_name(float v)"
//voir exemple dans Respawn.cs
public class Stat : NetworkBehaviour 
{
	public float val;

	public string statName;

	public float min;
	public float max;

	public List<StatCallback> callbackList = new List<StatCallback>();

	[ClientRpc] //called on server; executed on client
	public void RpcUpdateValue(float v) //used to propagate value back to clients (note it is also called on server);
	{
		if(isServer)
			PD2Debug.Log("RpcUpdateValue(" + v + ") called on server");

		if(isClient)
			PD2Debug.Log("RpcUpdateValue(" + v + ") called on client");
		val = v;
		onValueChange(v);
	}

	public void onValueChange(float v)
	{
		if(isServer)
			PD2Debug.Log("onValueChange(" + v +") called on server");

		if (isClient)
			PD2Debug.Log("onValueChange(" + v + ") called on client");
		foreach (StatCallback callback in callbackList)
		{
			callback(v);
		}
	}

	[Server]
	protected void assignCheckLimits(float v)
	{
		if (v < min)
			v = min;
		if (v > max)
			v = max;
		PD2Debug.Log("assignCheckLimits(" + v + ") called on server");
		RpcUpdateValue(v);
		PD2Debug.Log("RpcUpdateValue(" + v + ") calling from server");
	}

	public float getValue()
	{
		return val;
	}

	public void setValue(float v)
	{
		if (!isServer)
			return;
		PD2Debug.Log("setValue(" + v + ") called on server");
		assignCheckLimits(v);
	}

	public void add(float v)
	{
		if (!isServer)
			return;
		PD2Debug.Log("add(" + v + ") called on server");
		assignCheckLimits(val + v);
	}

	public void substract(float v)
	{
		if (!isServer)
			return;
		PD2Debug.Log("substract(" + v + ") called on server");
		assignCheckLimits(val - v);
	}
}