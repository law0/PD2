using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Attack_SimpleAccel : Attack
{
	public override string attackName
	{
		get { return "Attack_SimpleAccel"; }
	}

	public override AttackType type
	{
		get { return AttackType.ACCEL; }
	}

	public float speed = 12.0F;

	public override IEnumerator fire(GameObject emitter)
	{
		_nextFireTime = Time.time + cooldown;
		yield return new WaitForSeconds(chargeCooldown);
		var moveScript = emitter.GetComponent<Move>() as Move;
		AttackSystem attackSystem = GetComponent<AttackSystem>();

		//Je sais pas en quoi c'est une attaque... d'ailleurs dash tp etc
		//peuvent ne pas etre des attaques non plus
		//Mais fallait bien mettre ces skills quelque part...
		//Oh... j'aurai ptetre du nommé SkillSystem...
		//rofff flemme de tout changer
		//Ah oui ci dessous bah, on change la speed du joueur dans le script move
		//on attend ensuite... AARGH une valeur en dur!!!
		//NOTE: changer la valeur 1.0F ci dessous par attackData.accelDuration
		//NOTE: et créer attackData.accelDuration et le renseigner!

		if (null != moveScript)
		{
			float origspeed = moveScript.speed;
			moveScript.speed = speed;
			yield return new WaitForSeconds(1.0F);
			moveScript.speed = origspeed;
		}
	}
}
