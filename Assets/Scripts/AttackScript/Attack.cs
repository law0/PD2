using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/**
 * Et bien et bien...
 * bienvenue au coeur du champs de bataille! : les attaques
 * Attaquons nous au sujet... ho ho ho...
 *
 **/

/**
 * AttackData! Chaque attaque ayant des besoins spécifiques (elles sont si capricieuses)
 * Il aurait ete compliqué de faire une classe par Attaque avec chacune des attributs différents...
 * Donc allez tout le monde partage la meme structure!
 * AttackData est donc une structure pouvant contenir des données pour les différentes attaques
 * Chaque attaque ne regardera que ce qui l'intéresse. Il n'est donc pas nécéssaire de renseigner tpDistance pour
 * une attaque de type MELEE...
 * NOTE: par convention le nom de chaque champs doit commencer par le nom du type d'attaque auquel il sert
 * Si il en sert 2 ou plus ce n'est pas obligatoire (genre bulletIndex sert à CAST et CIBLEE) mais il vaut 
 * mieux que le nom reste explicite!
 * */
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

/**
 * Class représentant une attaque
 * une attaque peut etre de 6 sortes
 * CAST, DASH, ACCEL, MELEE, TP, et CIBLEE
 * CAST et CIBLEE sont les attaques de lancer
 * CAST lance juste une bullet, CIBLEE lance une bullet qui cherchera une cible
 * NOTE: les "bullets" sont des prefabs à part. Ce sont donc ces bullets qui s'occuperont
 * d'infliger des degats ou de chercher la cible, ici on se contente de les instancier et les init
 * DASH permet de déplacer le Player d'un point A à un point B rapidos, sans que le Player
 * puisse modifier sa trajectoire en cours de route
 * NOTE: Degat de dash a faire
 * ACCEL: Accelere (ou ralentit tiens) les déplacements du Player pendant une courte durée
 * MELEE: typiquement une attaque au corps à corps. C'est juste une animation et des dégats dans une zone pres du corps
 * TP: devine!
 * 
 * NOTE: Attack est un monobehaviour qui peut etre mis des le départ dans le prefab Player ou instancier dynamiquement
 * via AttackSystem
 *
 * Chaque Attack a plusieurs attributs essentiels:
 * string attackName <- son nom. Doit etre unique pour chaque joueur, car les attaques sont rangées dans un dico
 * AttackType type <- (voir enum plus haut) type de l'attaque, DASH, CAST etc.
 * float cooldown <- devine
 * float chargeCooldown <- délai d'attente ayant lieu après l'evenement déclencheur de l'attaque, avant ladite attaque
 * 			   permet par exemple de faire des grosses attaques qui nécéssitent un temps de chargement
 * float damage <- devine encore! NOTE: pour CAST et CIBLEE, les scripts de bullet correspondant se verront attribué
 * 		   cette valeur damage! Il FAUT donc qu'ils aient un attribute public damage
 * Keycode key <- Space, A, Mouse0... entrée clavier ou souris qui déclenche l'attaque
 * string AnimFloatName <- nom de l'animation à joueur. IMPORTANT: les animations doivent avoir des transitions de type 
 * 			float! Si tu comprends pas, demande à tonton law0
 * AttackData attackData <- voir AttackData structure plus haut
 * */

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
	/*
	 *
	 * L'heure est grave... le joueur a demandé une attaque...
	 * C'est terrible!!! Ou pas =D A BAS L'ENNEMI!
	 * Mais comment lancer l'attaque ?
	 * Primo, tu remarqueras que cette fonction retourne un IEnumerator
	 * non pas que c'est fun, mais parce que cette fonction est lancée en coroutine.
	 * Deucio, hum deuxio! concretement ca fait juste un switch sur le type de l'attaque, 
	 * stocké dans la variable...
	 * -> type <-
	 * Incroyable :)
	 * Et selon le type d'attaque, le comportement change obviously
	 * */
	public IEnumerator fire(GameObject emitter)
	{
		_nextFireTime = Time.time + cooldown;
		yield return new WaitForSeconds(chargeCooldown);
		var moveScript = emitter.GetComponent<Move>() as Move;
		AttackSystem attackSystem = GetComponent<AttackSystem>();
		switch (type)
		{
			case AttackType.CAST: //si on est en cast!
				// Alors 1 on récupere la bullet
				// La bonne bullet est choisie grace au bulletIndex que tu n'auras pas
				// oublié de renseigner dans attackData
				// L'index bulletIndex doit correspondre à l'index de la bullet que tu veux
				// dans le tableau bullets de l'attackSystem
				// (Tu n'auras donc pas non plus oublier de renseigner les bullets dans ledit tableau...)
				GameObject bullet = attackSystem.bullets[attackData.bulletIndex];
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
				break;


				//Je sais pas en quoi c'est une attaque... d'ailleurs dash tp etc
				//peuvent ne pas etre des attaques non plus
				//Mais fallait bien mettre ces skills quelque part...
				//Oh... j'aurai ptetre du nommé SkillSystem...
				//rofff flemme de tout changer
				//Ah oui ci dessous bah, on change la speed du joueur dans le script move
				//on attend ensuite... AARGH une valeur en dur!!!
				//NOTE: changer la valeur 1.0F ci dessous par attackData.accelDuration
				//NOTE: et créer attackData.accelDuration et le renseigner!
			case AttackType.ACCEL:
				if (null != moveScript)
				{
					float origspeed = moveScript.speed;
					moveScript.speed = attackData.accelSpeed;
					yield return new WaitForSeconds(1.0F);
					moveScript.speed = origspeed;
				}
				break;

				//Globalement le point de départ c'est la ou t'es
				//Il y pas de point d'arrivée mais plutot une distance a parcourir
				//(attackData.dashDistance) dans une direction (devant le joueur)
				//Pourquoi devant et pas vers la ou tu cliques? benh le joueur se tourne
				//des que tu cliques quelque part anyway... donc devant c'est bon ;)
				//
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

			// devine ce que ca fait...
			// bref si tu es dans le radius tu te prends des degats... ouf hein :)
			case AttackType.MELEE:
				List<GameObject> melee_playersInRadius = PlayerUtils.getPlayerIn3DRadius(emitter.transform.position, attackData.meleeDamageRadius);
				foreach (GameObject player in melee_playersInRadius)
				{
					player.GetComponent<StatSystem>().substract("health", damage);
				}
				break;

			// devine!!!	
			case AttackType.TP:
				if (null != moveScript)
				{
					emitter.transform.position += emitter.transform.forward * attackData.tpDistance;
					moveScript.stopMove(); //sinon il va tenter d'aller a la position precedemment demandee 
				}
				break;

			// Alors la... c'est compliqué.
			// De base ca fait la meme chose qu'en cast
			// du coup si t'as pas vu CAST, va le voir
			// Sinon... lis petit à petit
			case AttackType.CIBLEE:
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
					if (Vector3.Distance(emitter.transform.position, target.transform.position) < attackData.cibleeRadius && target != gameObject)
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
							bullet_ciblee_script.setTarget(target); // en plus des damages et du player
							//qui a emis la bullet; on passe aussi la cible, c'est logique banane...
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
