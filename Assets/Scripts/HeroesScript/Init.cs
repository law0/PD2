using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Init : NetworkBehaviour {

	public override void OnStartLocalPlayer()
	{
		tag = "LocalPlayer";
	}

	// Use this for initialization
	void Start () 
	{
		PlayerUtils.PlayerList.Add(gameObject);
		StatSystem statSystem = GetComponent<StatSystem>();
		if (null != statSystem)
		{
			statSystem.die(); // pour synchro toutes les vies a 100
		}
	}
}
