using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_SimpleMelee : Attack
{
	public override string attackName
	{
		get { return "Attack_SimpleMelee"; }
	}

	public override AttackType type
	{
		get { return AttackType.MELEE; }
	}

	public float damageRadius = 0.5F; //on doit bouger les animations autre part

	public override IEnumerator fire(GameObject emitter)
	{
		_nextFireTime = Time.time + cooldown;
		yield return new WaitForSeconds(chargeCooldown);
		var moveScript = emitter.GetComponent<Move>() as Move;
		AttackSystem attackSystem = GetComponent<AttackSystem>();

		// devine ce que ca fait...
		// bref si tu es dans le radius tu te prends des degats... ouf hein :)

		List<GameObject> melee_playersInRadius = PlayerUtils.GetPlayerIn3DRadius(emitter.transform.position, damageRadius);
		foreach (GameObject player in melee_playersInRadius)
		{
			player.GetComponent<StatSystem>().substract("health", damage);
		}
	}
}
