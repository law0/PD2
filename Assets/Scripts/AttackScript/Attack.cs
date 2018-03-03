using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public struct AttackData //données spécifique à chaque attaque
{
	public bool interruptible;
	public int bulletIndex; //a renseigner si type cast
	public float accelSpeed; //a renseigner si type acceleration
	public float dashSpeed, dashDistance, dashDamageRadius;//a renseigner si type dash
	public float meleeDamageRadius; //a renseigner si type melee
	public float tpDistance; //a renseigner si type TP
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

public class Attack : MonoBehaviour
{
	public string attackName;

	//type de l'attaque: CAST, DASH ou MELEE
	public AttackType type;

	public float cooldown;

	//delai de charge au moment du lancement de l'attaque (exemple : animation avant explosion)
	public float chargeCooldown;

	public float damage;

	//moment ou le cooldown sera ecoule 
	private float _nextFireTime;
	public float NextFireTime
	{
		get
		{
			return _nextFireTime;
		}
		set
		{

			_nextFireTime = value;
		}
	}

	//touche clavier de lancement de l'attaque
	public KeyCode key;

	//name of animation float trigger
	public string animFloatName;

	public float animFloat;

	//contient differentes donnees selon le type d'attaque
	public AttackData attackData;

	//fonction de lancement de l'attaque
	public IEnumerator fire(GameObject emitter)
	{
		_nextFireTime = Time.time + cooldown;
		yield return new WaitForSeconds(chargeCooldown);
		var moveScript = emitter.GetComponent<Move>() as Move;
		AttackSystem attackSystem = GetComponent<AttackSystem>();
		switch (type)
		{
			case AttackType.CAST:
				GameObject bullet = attackSystem.bullets[attackData.bulletIndex];
				var bullet_clone = Object.Instantiate(bullet, emitter.transform.position + Vector3.up + emitter.transform.forward * 2, emitter.transform.rotation) as GameObject;
				//un peu plus haut qu'au sol, et un peu plus en avant par rapport au perso  
				var bullet_script = bullet_clone.GetComponent<dummy_bullet>();
				if (bullet_script != null)
				{
					bullet_script.damage = damage;
					bullet_script.setOriginGameObject(emitter);
				}
				break;

			case AttackType.ACCEL:
				if (null != moveScript)
				{
					float origspeed = moveScript.speed;
					moveScript.speed = attackData.accelSpeed;
					yield return new WaitForSeconds(1.0F);
					moveScript.speed = origspeed;
				}
				break;

			case AttackType.DASH:
				if (null != moveScript)
				{
					bool dashNotFinish = true;
					moveScript.lockMove();
					Vector3 origPos = emitter.transform.position;
					while (dashNotFinish)
					{
						List<GameObject> dash_playersInRadius = PlayerUtils.getPlayerIn3DRadius(emitter.transform.position, attackData.dashDamageRadius);
						foreach (GameObject player in dash_playersInRadius)
						{
							if(player != gameObject) //manquerait plus qu'on se fasse des dommages soit meme
								player.GetComponent<StatSystem>().substract("health", damage);
						}

						emitter.transform.position += emitter.transform.forward * attackData.dashSpeed * Time.deltaTime;
						if (Vector3.Distance(origPos, emitter.transform.position) < attackData.dashDistance)
							yield return null;
						else
							dashNotFinish = false;
					}
					moveScript.unlockMove();
					moveScript.stopMove(); //sinon il va tenter d'aller a la position precedemment demandee
				}
				break;

			case AttackType.MELEE:
				List<GameObject> melee_playersInRadius = PlayerUtils.getPlayerIn3DRadius(emitter.transform.position, attackData.meleeDamageRadius);
				foreach (GameObject player in melee_playersInRadius)
				{
					player.GetComponent<StatSystem>().substract("health", damage);
				}
				break;

			case AttackType.TP:
				if (null != moveScript)
				{
					emitter.transform.position += emitter.transform.forward * attackData.tpDistance;
					moveScript.stopMove(); //sinon il va tenter d'aller a la position precedemment demandee 
				}
				break;

			case AttackType.CIBLEE:
				Debug.Log("here at least!");
				int index = GetComponent<AttackSystem>().PlayerClickedIndex;
				if (-1 != index)
				{
					GameObject target = PlayerUtils.PlayerList[index];
					if (Vector3.Distance(emitter.transform.position, target.transform.position) < attackData.cibleeRadius)
					{
						Debug.Log("here");
						GameObject bullet_ciblee = attackSystem.bullets[attackData.bulletIndex];
						var bullet_ciblee_clone = Object.Instantiate(bullet_ciblee, emitter.transform.position + Vector3.up + emitter.transform.forward * 2, emitter.transform.rotation) as GameObject;
						//un peu plus haut qu'au sol, et un peu plus en avant par rapport au perso  
						var bullet_ciblee_script = bullet_ciblee_clone.GetComponent<BulletCiblee>();
						if (bullet_ciblee_script != null)
						{
							bullet_ciblee_script.damage = damage;
							bullet_ciblee_script.setOriginGameObject(emitter);
							bullet_ciblee_script.setTarget(target);
						}
					}
				}
				else
				{
					Debug.Log("over here");
				}
				break;


		}
	}
}
