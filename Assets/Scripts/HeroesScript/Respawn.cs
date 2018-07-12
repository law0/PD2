using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Respawn : NetworkBehaviour {

	public StatSystem statSystem;

	void Start()
	{
		if (statSystem)
		{
			statSystem.addCallback("health", healthWatcher);
		}
	}

	
	// Update is called once per frame
	void Update () 
    {
        if (transform.position.y < -2.5)
		{
			respawn();
        }
	}

	public void respawn()
	{
		MatchManager matchManager = MatchManager.instance;
		if (matchManager != null && matchManager.hasTeams && matchManager.teamASpawnPoint != null && matchManager.teamBSpawnPoint != null)
		{
			if (matchManager.isInTeamA(gameObject))
				transform.position = matchManager.teamASpawnPoint.transform.position + Vector3.up * 2.0F;
			else if (matchManager.isInTeamB(gameObject))
				transform.position = matchManager.teamBSpawnPoint.transform.position + Vector3.up * 2.0F;
			else
				Debug.LogWarning("Ca marche toujours pas");
		}
		else
		{
			transform.position = new Vector3(0.0F, 2.0F, 0.0F);
		}	
		var hisStatSystem = gameObject.GetComponent<StatSystem>();
		if (hisStatSystem != null)
			hisStatSystem.setValue("health", 100); //en cas de respawn remet la valeur a 100
		var move = gameObject.GetComponent<Move>();
		if(null != move)
			move.stopMove();
	}

	public void healthWatcher(float v)
	{
		if (v <= 0)
			respawn();
	}

	[Server]
	public static void respawnEveryone()
	{
		if (!NetworkServer.active)
			return;

		Debug.LogWarning("yahoo");
		//alors oui, ça laisse un gros avantage au gars qui run le server, il peut faire respawn tout le monde...
		//trouver comment regler ca
		NetworkInit.reinit();
	}

}
