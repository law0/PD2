using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//class permettant de lister les stats d'un joueur : permet de faire statSystem.stats["health"].add(10) par exemple
//voir exemple dans Respawn.cs
public class StatSystem : NetworkBehaviour
{
	public Dictionary<string, Stat> stats = new Dictionary<string, Stat>();

	public int networkID;

	void Awake()
	{
		Stat[] statsint = GetComponents<Stat>();

		foreach (Stat stat in statsint)
		{
			stats.Add(stat.statName, stat);
		}
	}

	public void die()
	{
		Stat health = stats["health"];
		if (null != health)
		{
			health.substract(3000.0F);
		}
		else
		{
			Debug.LogError("can't die : no health!");
		}
	}
}

