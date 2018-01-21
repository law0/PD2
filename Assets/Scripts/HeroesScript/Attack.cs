using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Attack : NetworkBehaviour {

	//animator
	private Animator anim;
	private float anim_float = 0.0F; //triggers doesn't work online
	//you may want to wait some time before spawning the bullet (example end of animation)
	public float waitFor = 0.0F;
	//bullet prefab
	public GameObject bullet;
	public float fireRate = 0.5F;

	//transform de l'emetteur (parce que pas forcement le Player ex: le bout d'un canon)
	public Transform emitterTransform;

	private float nextFire = 0.0F;

	// Use this for initialization
	void Start () 
	{
		anim = transform.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!isLocalPlayer)
			return;
		if (Input.GetKeyDown(KeyCode.A) && Time.time > nextFire)
		{
			anim_float = 1.0F;
			nextFire = Time.time + fireRate;
			CmdAttack();
		}
		//because networkanimator doesn't fuckin sync triggers automatically... and it still bugs
		anim.SetFloat("attack", anim_float);
		anim_float = anim_float <= 0 ? 0 : anim_float - 0.10F;
	}

	//[Command] functions are Asked by client but Called on the Server only
	[Command]
	public void CmdAttack()
	{
		if (bullet != null)
		{
			RpcActualAttack();
		}
	}

	[ClientRpc]
	public void RpcActualAttack()
	{
		StartCoroutine(actualAttack());
	}

	public IEnumerator actualAttack()
	{
		yield return new WaitForSeconds(waitFor);
		var bullet_clone = Instantiate(bullet, emitterTransform.position + Vector3.up, emitterTransform.rotation) as GameObject;
		var bullet_script = bullet_clone.GetComponent<dummy_bullet>();
		if (bullet_script != null)
			bullet_script.setOriginGameObject(gameObject);

		if(isServer)
			NetworkServer.Spawn(bullet_clone); //need to make the network server spawn everywhere (on each client) the bullet_clone
	}
}
