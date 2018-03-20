using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Attack_SimpleTp : Attack
{
	public override string attackName
	{
		get{ return "Attack_SimpleTp"; }
	}

	//type de l'attaque: CAST, DASH ou MELEE
	public override AttackType type
	{
		get { return AttackType.TP;}
	}

	public float tpDistance = 5.0f;

	public override IEnumerator fire(GameObject emitter)
	{
		_nextFireTime = Time.time + cooldown;
		yield return new WaitForSeconds(chargeCooldown);
		var moveScript = emitter.GetComponent<Move>() as Move;

		if (null != moveScript)
		{
			emitter.transform.position += emitter.transform.forward * tpDistance;
			moveScript.stopMove(); //sinon il va tenter d'aller a la position precedemment demandee 
		}
	}
}
