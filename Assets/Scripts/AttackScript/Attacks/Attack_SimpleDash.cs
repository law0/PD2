using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Attack_SimpleDash : Attack
{
	public override string attackName
	{
		get { return "Attack_SimpleDash"; }
	}

	public override AttackType type
	{
		get { return AttackType.DASH; }
	}

	public float speed = 18.0f;

	public float distance = 5.0f;

	//public float damageRadius = 0.5f;

	public override IEnumerator fire(GameObject emitter)
	{
		_nextFireTime = Time.time + cooldown;
		yield return new WaitForSeconds(chargeCooldown);
		var moveScript = emitter.GetComponent<Move>() as Move;
		AttackSystem attackSystem = GetComponent<AttackSystem>();


		//Globalement le point de départ c'est la ou t'es
		//Il y pas de point d'arrivée mais plutot une distance a parcourir
		//(attackData.dashDistance) dans une direction (devant le joueur)
		//Pourquoi devant et pas vers la ou tu cliques? benh le joueur se tourne
		//des que tu cliques quelque part anyway... donc devant c'est bon ;)
		//

		if (null != moveScript)
		{
			bool dashNotFinish = true;
			moveScript.lockMove();
			Vector3 origPos = emitter.transform.position;
			while (dashNotFinish)
			{
				/*List<GameObject> dash_playersInRadius = PlayerUtils.getPlayerIn3DRadius(emitter.transform.position, damageRadius);
				foreach (GameObject player in dash_playersInRadius)
				{
					if(player != gameObject) //manquerait plus qu'on se fasse des dommages soit meme
						player.GetComponent<StatSystem>().substract("health", damage);
				}*/

				emitter.transform.position += emitter.transform.forward * speed * Time.deltaTime;
				if (Vector3.Distance(origPos, emitter.transform.position) < distance)
					yield return null;
				else
					dashNotFinish = false;
			}
			moveScript.unlockMove();
			moveScript.stopMove(); //sinon il va tenter d'aller a la position precedemment demandee
		}
	}
}
