using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum AttackType
{
	CAST,
	DASH,
	MELEE,
};

public class Attack
{
	//type de l'attaque: CAST, DASH ou MELEE
	private AttackType _type;

	//delai entre 2 attaques
	private float _cooldown;
	public float Cooldown { get { return _cooldown; } }

	//delai de charge au moment du lancement de l'attaque (exemple : animation avant explosion)
	private float _chargeCooldown;
	public float ChargeCooldown { get { return _chargeCooldown; } }

	//moment ou le cooldown sera ecoule 
	private float _nextFireTime;
	public float NextFireTime { get { return _nextFireTime; } }

	//touche clavier de lancement de l'attaque
	private KeyCode _key;
	public KeyCode Key{ get { return _key; } }

	//name of animation float trigger
	private string _animFloatName;
	public string AnimFloatName { get { return _animFloatName; } }

	//si l'attaque est un cast alors on a besoin de la balle
	private GameObject _bullet;

	//constructeur
	public Attack(AttackType type, float cooldown, KeyCode key, string animFloat, float chargeCooldown = 0.0F)
	{
		_type = type;
		_cooldown = cooldown;
		_key = key;
		_animFloatName = animFloat;
	}

	//fonction de lancement de l'attaque
	public IEnumerator fire(GameObject emitter)
	{
		_nextFireTime = Time.time + _cooldown;
		yield return new WaitForSeconds(_chargeCooldown);
		if (_type == AttackType.CAST)
		{
			var bullet_clone = Object.Instantiate(_bullet, emitter.transform.position + Vector3.up + emitter.transform.forward * 2, emitter.transform.rotation) as GameObject;
			//un peu plus haut qu'au sol, et un peu plus en avant par rapport au perso  
			var bullet_script = bullet_clone.GetComponent<dummy_bullet>();
			if (bullet_script != null)
				bullet_script.setOriginGameObject(emitter);

			NetworkServer.Spawn(bullet_clone); //need to make the network server spawn everywhere (on each client) the bullet_clone
		}
		else if (_type == AttackType.DASH)
		{//not an actual dash attack, just an acceleration during 1 sec, need to change that
			var moveScript = emitter.GetComponent<Move>() as Move;
			float origspeed = moveScript.speed;
			moveScript.speed = 12.0F;
			yield return new WaitForSeconds(1.0F);
			moveScript.speed = origspeed;
		}
	}

	public void setBullet(GameObject bullet)
	{
		if (AttackType.CAST != _type)
			return;

		_bullet = bullet;
	}
}
