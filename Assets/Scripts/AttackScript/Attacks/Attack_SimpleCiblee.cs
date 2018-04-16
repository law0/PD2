using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_SimpleCiblee : Attack
{
	public override string attackName
	{
		get { return "Attack_SimpleCiblee"; }
	}

	public override AttackType type
	{
		get { return AttackType.CIBLEE; }
	}

	public float radius = 20.0F;

	public int bulletIndex = 1; //ne pas oublier de mettre la bullet dans l'attackSystem!

	public override IEnumerator fire(GameObject emitter)
	{
		_nextFireTime = Time.time + cooldown;
		yield return new WaitForSeconds(chargeCooldown);
		var moveScript = emitter.GetComponent<Move>() as Move;
		AttackSystem attackSystem = GetComponent<AttackSystem>();

		// Alors la... c'est compliqué.
		// De base ca fait la meme chose qu'en cast
		// du coup si t'as pas vu CAST, va le voir
		// Sinon... lis petit à petit

		//Alors AttackSystem.PlayerClickedIndex
		// Je t'avais promis que je t'expliquerai...
		// PlayerClickedIndex est l'index du dernier joueur sur lequel tu as cliqué
		// Chaque joueur est répertorié dans le tableau dans PlayerUtils.PlayerList
		// Il s'agit donc de l'index dans ce tableau
		// Pour voir comment cet index est placé, va voir PlayerUtils... allez va...
		int index = GetComponent<AttackSystem>().PlayerClickedIndex;
		if (-1 != index)
		{
			GameObject target = PlayerUtils.PlayerList[index];
			//Les attaques ciblees ne doivent marcher que dans un certain rayon aussi
			if (Vector3.Distance(emitter.transform.position, target.transform.position) < radius && target != gameObject)
			{
				Debug.Log("here");
				GameObject bullet_ciblee = attackSystem.bullets[bulletIndex];
				var bullet_ciblee_clone = Object.Instantiate(bullet_ciblee, emitter.transform.position + Vector3.up + emitter.transform.forward * 2, emitter.transform.rotation) as GameObject;
				//un peu plus haut qu'au sol, et un peu plus en avant par rapport au perso  
				var bullet_ciblee_script = bullet_ciblee_clone.GetComponent<BulletCiblee>();
				if (bullet_ciblee_script != null)
				{
					bullet_ciblee_script.damage = damage;
					bullet_ciblee_script.setOriginGameObject(emitter);
					bullet_ciblee_script.setTarget(target); // en plus des damages et du player
															//qui a emis la bullet; on passe aussi la cible, c'est logique banane...
				}
			}
		}
		else
		{
			Debug.Log("over here");
		}
	}
}
