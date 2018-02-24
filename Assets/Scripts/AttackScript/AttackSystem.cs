using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AttackSystem : NetworkBehaviour {

	//animator
	private Animator anim;
	public GameObject[] bullets;

	//code en dur a remplacer
	//les attaques sont ici, a remplacer pour chargement "dynamique"
	public List<Attack> attacks = new List<Attack>(); 

	// Use this for initialization
	void Start () 
	{
		//Faudra degager tout ça de la
		var castData = new AttackData(); castData.bullet = bullets[0];
		var accelData = new AttackData(); accelData.accelSpeed = 12.0F;
		var dashData = new AttackData(); dashData.dashSpeed = 18.0F; dashData.dashDistance = 5.0F; dashData.dashDamageRadius = 0.5F;

		dashData.applyToOther = other =>
		{ //lambda function that apply 10 damage within dashData.dashDamageRadius
			var otherStatSystem = other.GetComponent<StatSystem>() as StatSystem;
			if (otherStatSystem != null && other != gameObject)
			{
				otherStatSystem.stats["health"].substract(10);
			}
		};

		var meleeData = new AttackData(); meleeData.meleeDamageRadius = 0.5F;
		meleeData.applyToOther = dashData.applyToOther; //juste la flemme de reecrire une deuxieme lambda function

		var tpData = new AttackData(); tpData.tpDistance = 5.0F;

		var cibleeData = new AttackData(); cibleeData.cibleeRadius = 20.0F; cibleeData.bullet = bullets[1];

		attacks.Add(new Attack(AttackType.CAST, 0.5F, KeyCode.A, "cast_attack", castData));
		//attacks.Add(new Attack(AttackType.ACCEL, 1.0F, KeyCode.Z, "accel_attack", accelData));
		//attacks.Add(new Attack(AttackType.DASH, 1.0F, KeyCode.E, "dash_attack", dashData));
		//attacks.Add(new Attack(AttackType.MELEE, 1.0F, KeyCode.R, "melee_attack", meleeData));
		//attacks.Add(new Attack(AttackType.TP, 5.0F, KeyCode.T, "tp_attack", tpData));
		attacks.Add(new Attack(AttackType.CIBLEE, 1.0F, KeyCode.Mouse0, "cast_attack", cibleeData));

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
				attacks[index].AnimFloat = 1.0F;
				CmdAttack(index);
			}
			anim.SetFloat(attacks[index].AnimFloatName, attacks[index].AnimFloat);
			attacks[index].AnimFloat = attacks[index].AnimFloat < 0 ? 0.0F : attacks[index].AnimFloat - 0.1F;
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
