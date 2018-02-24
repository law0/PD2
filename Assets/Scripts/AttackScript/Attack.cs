using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public delegate void ApplyToOther(GameObject other);

public struct AttackData //données spécifique à chaque attaque
{
	public bool interruptible;
	public GameObject bullet; //a renseigner si type cast
	public float accelSpeed; //a renseigner si type acceleration
	public float dashSpeed, dashDistance, dashDamageRadius;//a renseigner si type dash
	public float meleeDamageRadius; //a renseigner si type melee
	public float tpDistance; //a renseigner si type TP
	public ApplyToOther applyToOther; //fonction qui s'appliquera sur l'objet ciblé ou atteint (e.g pour appliquer les dégats)
									  //pourquoi une fonction? c'est compliqué... pour par exemple les damages over time ou pour les knockbacks : pour pouvoir appliquer
									  //des choses plus compliqué que des dégats
	public float cibleeRadius; //si ciblee (auto_attack) -> rayon dans lequel l'autoattack marche
};

public enum AttackType
{
	CAST,
	DASH,
	ACCEL,
	MELEE,
	TP,
	CIBLEE,
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

	private float _animFloat;
	public float AnimFloat { get { return _animFloat; } set { _animFloat = value; } }

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
				{
					bullet_script.setOriginGameObject(emitter);
					bullet_script.spawn();
				}
				NetworkServer.Spawn(bullet_clone);
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
					moveScript.lockMove();
					Vector3 origPos = emitter.transform.position; 
					while(dashNotFinish)
					{
						List<GameObject> dash_playersInRadius = PlayerUtils.getPlayerIn3DRadius(emitter.transform.position, _attackData.dashDamageRadius);
						foreach (GameObject player in dash_playersInRadius)
						{
							_attackData.applyToOther(player);
						}

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

			case AttackType.MELEE:
				List<GameObject> melee_playersInRadius = PlayerUtils.getPlayerIn3DRadius(emitter.transform.position, _attackData.meleeDamageRadius);
				foreach (GameObject player in melee_playersInRadius)
				{
					_attackData.applyToOther(player);
				}
				break;

			case AttackType.TP:
				if(null != moveScript)
				{
					emitter.transform.position += emitter.transform.forward * _attackData.tpDistance;
					moveScript.stopMove(); //sinon il va tenter d'aller a la position precedemment demandee 
				}
				break;

			case AttackType.CIBLEE:
				GameObject target = PlayerUtils.clickedOnPlayer(_key);
				if(null != target && Vector3.Distance(emitter.transform.position, target.transform.position) < _attackData.cibleeRadius)
				{
					GameObject bullet_ciblee = _attackData.bullet;
					var bullet_ciblee_clone = Object.Instantiate(bullet_ciblee, emitter.transform.position + Vector3.up + emitter.transform.forward * 2, emitter.transform.rotation) as GameObject;
					//un peu plus haut qu'au sol, et un peu plus en avant par rapport au perso  
					var bullet_ciblee_script = bullet_ciblee_clone.GetComponent<BulletCiblee>();
					if (bullet_ciblee_script != null)
					{
						bullet_ciblee_script.setOriginGameObject(emitter);
						bullet_ciblee_script.setTarget(target);
					}
					//comment faire spawn sur le server sachant que sur le server ces conditions ne sont pas vrai?
				}
				break;
				
				
		}
	}
}
