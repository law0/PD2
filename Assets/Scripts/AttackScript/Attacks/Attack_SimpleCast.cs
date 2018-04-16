using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_SimpleCast : Attack
{

	public override string attackName
	{
		get { return "Attack_SimpleCast"; }
	}

	public override AttackType type
	{
		get { return AttackType.CAST; }
	}

	public int bulletIndex = 0; //ne pas oublier de mettre la bullet dans l'attackSystem!

	public override IEnumerator fire(GameObject emitter)
	{
		_nextFireTime = Time.time + cooldown;
		yield return new WaitForSeconds(chargeCooldown);
		var moveScript = emitter.GetComponent<Move>() as Move;
		AttackSystem attackSystem = GetComponent<AttackSystem>();

		GameObject bullet = attackSystem.bullets[bulletIndex];
		var bullet_clone = Object.Instantiate(bullet, emitter.transform.position + Vector3.up + emitter.transform.forward, emitter.transform.rotation) as GameObject;
		//un peu plus haut qu'au sol, et un peu plus en avant par rapport au perso  
		var bullet_script = bullet_clone.GetComponent<dummy_bullet>();
		if (bullet_script != null)
		{
			bullet_script.damage = damage; //On lui dit quelle dégats faire
			bullet_script.setOriginGameObject(emitter); //et la qui a tiré, toi en l'occurence
		}
		// PS! si tu regardes les tutoriaux (ou tutoriels ? ou tutorials? enfin bref les tutos)
		// de Unity sur le networking, tu verras qu'ils utilisent NetworkServer.Spawn
		// pour faire spawn un objet sur le réseau
		// L'ennui c'est que :
		// d'1 ici on est dans un monobehavior et ca ne changera pas parce qu'il faut que ce 
		// component soit ajoutable at runtime
		// de 2 cette fonction est executée sur le server et les clients, du coup
		// Object.Instantiate suivit de NetworkSpawn créera 2 bullets sur la machine qui
		// est a la fois cliente et server
	}
}
