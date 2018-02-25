using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Respawn : NetworkBehaviour {

	void Start()
	{
		
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (transform.position.y < -10)
		{
			respawn();
        }
	}

	void respawn()
	{
		transform.position = new Vector3(0.0F, 2.0F, 0.0F);
		var statSystem = gameObject.GetComponent<StatSystem>();
		if (statSystem != null)
			statSystem.setValue("health", 100); //en cas de respawn remet la valeur a 100
		var move = gameObject.GetComponent<Move>();
		if(null != move)
			move.stopMove();

	}

	public void healthWatcher(float v)
	{
		if (v <= 0)
			respawn();
	}

}
