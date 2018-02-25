using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//class permettant de lister les stats d'un joueur : permet de faire statSystem.stats["health"].add(10) par exemple
//voir exemple dans Respawn.cs
public class StatSystem : NetworkBehaviour
{
	private Dictionary<string, Stat> stats = new Dictionary<string, Stat>();

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
		substract("health", 3000.0F);
	}

	[Server]
	public void resync()
	{
		foreach(KeyValuePair<string, Stat> entry in stats)
		{
			Stat stat = entry.Value;
			RpcNewStat(stat.statName, stat.min, stat.max, stat.val);
		}
	}

	public void newStat(string statname, float min, float max, float val)
	{
		CmdNewStat(statname, min, max, val);
	}

	[Command]
	public void CmdNewStat(string statname, float min, float max, float val)
	{
		RpcNewStat(statname, min, max, val);
	}

	[ClientRpc]
	public void RpcNewStat(string statname, float min, float max, float val)
	{
		if (!stats.ContainsKey(statname))
		{
			Stat new_Stat = gameObject.AddComponent<Stat>();
			new_Stat.statName = statname;
			new_Stat.min = min;
			new_Stat.max = max;
			new_Stat.setValue(val);
			stats.Add(statname, new_Stat);
		}
	}

	[Server]
	protected void assignCheckLimits(string statname, float v)
	{
		if (stats.ContainsKey(statname))
		{
			Stat stat = stats[statname];
			if (v < stat.min)
				v = stat.min;
			if (v > stat.max)
				v = stat.max;
			PD2Debug.Log("assignCheckLimits(" + v + ") called on server");
			RpcUpdateValue("health", v);
			PD2Debug.Log("RpcUpdateValue(" + v + ") calling from server");
		}
	}

	[ClientRpc]
	private void RpcUpdateValue(string statname, float val)
	{
		PD2Debug.Log("RpcUpdateValue(" + val + ") called from server");
		if (stats.ContainsKey(statname))
		{
			stats[statname].setValue(val);
		}
	}

	public float getValue(string statname)
	{
		if (stats.ContainsKey(statname))
		{
			Stat stat = stats[statname];
			return stat.val;
		}
		return Mathf.NegativeInfinity;
	}

	public void setValue(string statname, float v)
	{
		if (!isServer)
			return;
		PD2Debug.Log("setValue(" + v + ") called on server");
		assignCheckLimits(statname, v);
	}

	public void add(string statname, float v)
	{
		if (!isServer)
			return;
		PD2Debug.Log("add(" + v + ") called on server");

		if (stats.ContainsKey(statname))
		{
			Stat stat = stats[statname];
			assignCheckLimits(statname, stat.val + v);
		}
	}

	public void substract(string statname, float v)
	{
		if (!isServer)
			return;
		PD2Debug.Log("substract(" + v + ") called on server");

		if (stats.ContainsKey(statname))
		{
			Stat stat = stats[statname];
			assignCheckLimits(statname, stat.val - v);
		}
	}

	public void addCallback(string statname, StatCallback callback)
	{
		if (stats.ContainsKey(statname))
		{
			stats[statname].callbackList.Add(callback);
		}
	}
}

