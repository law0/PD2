using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AttackSystem : NetworkBehaviour {

	//animator
	private Animator anim;
	private float anim_float = 0.0F; //triggers doesn't work online
	public GameObject[] bullets;

	//code en dur a remplacer
	//les attaques sont ici, a remplacer pour chargement "dynamique"
	private List<Attack> attacks = new List<Attack>(); 

	// Use this for initialization
	void Start () 
	{
		AttackData castData = new AttackData(); castData.bullet = bullets[0];
		AttackData accelData = new AttackData(); accelData.accelSpeed = 12.0F;
		AttackData dashData = new AttackData(); dashData.dashSpeed = 18.0F; dashData.dashDistance = 5.0F;

		attacks.Add(new Attack(AttackType.CAST, 0.5F, KeyCode.A, "cast_attack", castData));
		attacks.Add(new Attack(AttackType.ACCEL, 1.0F, KeyCode.Z, "accel_attack", accelData));
		attacks.Add(new Attack(AttackType.DASH, 1.0F, KeyCode.E, "dash_attack", dashData));

		anim = transform.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!isLocalPlayer)
			return;
		for (int index = 0; index < attacks.Count; ++index) //et pas un foreach parce qu'on a besoin de l'index
		{//puisqu'on ne peut transmettre que des types basique a travers les Cmd et Rpc
			if (Input.GetKeyDown(attacks[index].Key) && Time.time > attacks[index].NextFireTime)
			{
				Vector3 attackDir = gameObject.transform.position; //init attackDir to current Position so that if the value is not updated in GetMouse3DPos... the player will not rotate nor move
				PlayerUtils.GetMouse3DPosition(ref attackDir); //may update attackDir to the 3D MousePosition
				PlayerUtils.LookAtY(gameObject, ref attackDir); //make the passed gameObject (here the player) look at where we click
				//because networkanimator doesn't fuckin sync triggers automatically... and it still bugs
				anim.SetFloat(attacks[index].AnimFloatName, 1.0F);
				CmdAttack(index);
				anim.SetFloat(attacks[index].AnimFloatName, 0.0F);
			}
		}
	}

	//[Command] functions are Asked by client but Called on the Server only
	[Command]
	public void CmdAttack(int attackIndex)
	{
		RpcActualAttack(attackIndex);
	}

	[ClientRpc]
	public void RpcActualAttack(int attackIndex)
	{
		StartCoroutine(attacks[attackIndex].fire(gameObject));
	}
}
