using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public struct AttackData
{
	public GameObject bullet; //a renseigner si type cast
	public float accelSpeed; //a renseigner si type acceleration
	public float dashSpeed, dashDistance; //a renseigner si type dash
};

public enum AttackType
{
	CAST,
	DASH,
	ACCEL,
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

	//contient differentes donnees selon le type d'attaque
	private AttackData _attackData;

	//constructeur (et damage?)
	public Attack(AttackType type, float cooldown, KeyCode key, string animFloat, AttackData attackData, float chargeCooldown = 0.0F)
	{
		_type = type;
		_cooldown = cooldown;
		_key = key;
		_animFloatName = animFloat;
		_attackData = attackData;
	}

	//fonction de lancement de l'attaque
	public IEnumerator fire(GameObject emitter)
	{
		_nextFireTime = Time.time + _cooldown;
		yield return new WaitForSeconds(_chargeCooldown);
		var moveScript = emitter.GetComponent<Move>() as Move;
		switch(_type)
		{       
			case AttackType.CAST:
				GameObject bullet = _attackData.bullet;
				var bullet_clone = Object.Instantiate(bullet, emitter.transform.position + Vector3.up + emitter.transform.forward * 2, emitter.transform.rotation) as GameObject;
				//un peu plus haut qu'au sol, et un peu plus en avant par rapport au perso  
				var bullet_script = bullet_clone.GetComponent<dummy_bullet>();
				if (bullet_script != null)
					bullet_script.setOriginGameObject(emitter);

				NetworkServer.Spawn(bullet_clone); //need to make the network server spawn everywhere (on each client) the bullet_clone
				break;
		
			case AttackType.ACCEL:
				if(null != moveScript)
				{
					float origspeed = moveScript.speed;
					moveScript.speed = _attackData.accelSpeed;
					yield return new WaitForSeconds(1.0F);
					moveScript.speed = origspeed;
				}
				break;
		
			case AttackType.DASH:
				if(null != moveScript)		
				{
					bool dashNotFinish = true;
					//moveScript"2" parce qu'en C# les "case" sont apparement dans le meme scope... --'
					moveScript.lockMove();
					Vector3 origPos = emitter.transform.position; 
					while(dashNotFinish)
					{
						emitter.transform.position += emitter.transform.forward * _attackData.dashSpeed *Time.deltaTime;
						if(Vector3.Distance(origPos, emitter.transform.position) < _attackData.dashDistance)
							yield return null;
						else
							dashNotFinish = false;
					}
					moveScript.unlockMove();
					moveScript.stopMove(); //sinon il va tenter d'aller a la position precedemment demandee
				}
				break;
		}
	}
}
