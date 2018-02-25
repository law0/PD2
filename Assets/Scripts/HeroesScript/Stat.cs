using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void StatCallback(float v);


//class Stat. Le respect des limites entre min et max est automatique : add(25) sur une stat qui est a 100 et ayant pour max 120 ne montera la stat que jusqu'a 120
//+ system de callback. Pour ajouter une fonction callback a appeler a chaque fois que la valeur change, il suffit de faire stat.callbackList.Add(function_name)
//la signature de la fonction de callback doit etre "public void function_name(float v)"
//voir exemple dans Respawn.cs
public class Stat : MonoBehaviour 
{
	public float val;

	public string statName;

	public float min;
	public float max;

	public List<StatCallback> callbackList = new List<StatCallback>();

	public void onValueChange()
	{
		foreach (StatCallback callback in callbackList)
		{
			callback(val);
		}
	}

	public void setValue(float v)
	{
		val = v;
		PD2Debug.Log("setValue(" + v + ") called on server");
		onValueChange();
	}
}