using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void StatCallback(float v);

/**
 * D'abord va voir StatSystem.cs
 * et apres reviens ici
 * ...
 * T'es revenu? T'as lu? bon!
 * Et bien cette class ne fait pas grand chose, c'est juste un wrapper d'une valeur <- la stat en elle meme
 * Bon elle est pratique puisqu'elle associe aussi le min, le max, le nom de la stat (statname) et les callback
 * a appelé à chaque changement de cette valeur.
 * Ha et surtout! :
 * C'est un MonoBehaviour. Contrairement à StatSystem qui est un NetworkBehaviour.
 * Ce qui permet une chose: l'ajout dynamique de Stat.
 * Car les NetworkBehaviour ne Peuvent Pas Etre Ajouter AT RUNTIME. Oui fais chier.
 * Il fallut donc séparer toute la partie network des comportements dynamique at runtime
 * Voila pourquoi tout est aussi compliqué
 * */
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
