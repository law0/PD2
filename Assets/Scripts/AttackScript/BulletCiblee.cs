using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BulletCiblee : NetworkBehaviour {

	public float damage = 5f;
	private GameObject originGameObject = null;
	private GameObject _target = null;

	public void setOriginGameObject(GameObject obj) //vu que cette fonction existe aussi dans dummy_bullet ca serait cool de faire une interface
	{
		originGameObject = obj;
	}

	public void setTarget(GameObject target)
	{
		_target = target;
		GoToObject script = GetComponent<GoToObject>();
		if (null != script)
		{
			script.setCallbackOnReached(onTargetHit);
			script.goToTarget(_target);
		}
	}

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	void onTargetHit()
	{
		var target_statSystem = _target.GetComponent<StatSystem>() as StatSystem;
		if (null != target_statSystem)
		{
			target_statSystem.substract("health", damage);
		}
	}

	public void spawn()
	{
		if (isServer)
			NetworkServer.Spawn(gameObject);
	}
}
