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
 * d'infliger des degats ou de chercher la cible, dans les attaques on se contente de les instancier et les init
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

public abstract class Attack : MonoBehaviour
{
	public abstract string attackName
	{
		get;
	}

	//type de l'attaque: CAST, DASH ou MELEE
	public abstract AttackType type
	{
		get;
	}

	public float cooldown;

	//delai de charge au moment du lancement de l'attaque (exemple : animation avant explosion)
	public float chargeCooldown;

	public float damage;

	//moment ou le cooldown sera ecoule 
	protected float _nextFireTime;
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
	public string animFloatName; //on doit bouger les animations autre part

	public float animFloat; //on doit bouger les animations autre part

	//fonction de lancement de l'attaque
	/*
	 *
	 * L'heure est grave... le joueur a demandé une attaque...
	 * C'est terrible!!! Ou pas =D A BAS L'ENNEMI!
	 * Mais comment lancer l'attaque ?
	 * Primo, tu remarqueras que cette fonction retourne un IEnumerator
	 * non pas que c'est fun, mais parce que cette fonction est lancée en coroutine.
	 * Deucio, hum deuxio! concretement cette fonction est overrided dans les classes filles 
	 * Selon le type d'attaque, le comportement change obviously
	 * */
	public abstract IEnumerator fire(GameObject emitter);
}
