﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

/**
 * Et oui les attaques sont tellement compliquées qu'elles méritent leur propre dossier déjà
 * Ahem
 * La class AttackSystem, gère les attaques.
 * Elles permet notamment:
 * La synchro des attaques entre le server et les clients
 * L'ajout dynamique (pendant le jeu) et statique (avant le jeu dans le préfab) d'attaque
 * */
public class AttackSystem : NetworkBehaviour {

	//animator
	private Animator anim;
	public GameObject[] bullets;

	public GameObject attackList;

	//code en dur a remplacer
	//les attaques sont ici, a remplacer pour chargement "dynamique"
	private Dictionary<string, Attack> attacks = new Dictionary<string, Attack>();

	/** Huh? WADISTHAT? VASISTAS?
	 * Dur d'expliquer la... va voir dans Attack.cs c'est expliqué
	 * Si si allez vas y, je te proooomet
	 * */
	private int playerClickedIndex = -1;
	public int PlayerClickedIndex
	{
		get
		{
			return playerClickedIndex;
		}
		set
		{
			playerClickedIndex = value;
		}
	}

	/**
	 * Ca c'est Awake. Awake je te présente le crétin qui lit, crétin qui lit voici Awake...
	 * Bonjour, enchanté. Je fais quoi dans la vie? Bah je m'execute quelque temps apres l'instantiation
	 * de l'objet à laquelle j'appartiens...
	 * Ha je fais quoi ici? La j'ajoute les components de type Attack déjà présent dans le préfab
	 * à la liste d'attaque, qui d'ailleurs est un dico plutot
	 * ... j'ai l'impression que ce jeu est un peu sauvage non?
	 * */ 
	void Awake()
	{
		//on ajoute les attaques placées de base dans le player
		Attack[] attackarray = GetComponents<Attack>();

		foreach(Attack attack in attackarray)
		{
			attacks.Add(attack.attackName, attack);
		}		
	}

	// Use this for initialization
	void Start () 
	{
		anim = transform.GetComponent<Animator>();
		attackList = GameObject.Find("AttackList");
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!isLocalPlayer)
			return;

		//Wow c'est moche!!!
		// Traduction rapide: pour chaque attaque
		// Si on appuie sur la bonne touche et que le cooldown est passé appel CmdAttack
		// avec le nom de l'attaque
		// Et d'autre truc genre tourne le player dans la bonne direction
		// et fais l'animation
		// NOTE: bug! l'animation d'attaque ciblée est joué chaque fois qu'on clique...
		foreach (KeyValuePair<string, Attack> entry in attacks)
		{//puisqu'on ne peut transmettre que des types basique a travers les Cmd et Rpc
			if (Input.GetKeyDown(entry.Value.key) && Time.time > entry.Value.NextFireTime)
			{
				Vector3 attackDir = gameObject.transform.position; //init attackDir to current Position so that if the value is not updated in GetMouse3DPos... the player will not rotate nor move
				PlayerUtils.GetMouse3DPosition(ref attackDir); //may update attackDir to the 3D MousePosition
				PlayerUtils.LookAtY(gameObject, ref attackDir); //make the passed gameObject (here the player) look at where we click

				//because networkanimator doesn't fuckin sync triggers automatically... and it still bugs
				entry.Value.animFloat = 1.0F;
				CmdAttack(entry.Key);
			}
			anim.SetFloat(entry.Value.animFloatName, entry.Value.animFloat);
			entry.Value.animFloat = entry.Value.animFloat < 0 ? 0.0F : entry.Value.animFloat - 0.1F;
		}
	}

	/** DO ATTACK NOW!!!
	 * JE TE COMMMANDE!!! (avec un accent allemand)
	 * bref toi, dear local player, pour avoir le mot sur tous tes avatars présent chez les autres clients
	 * Tu dois invoquer cette fonction qui sera:
	 * Appelée localement
	 * Mais executée sur le server
	 * Cette fonction, sur le server, appelera ensuite RpcActualAttack, qui commandera à tes 
	 * avatars chez les clients de faire l'attaque que tu as demandée
	 */
	//[Command] functions are Asked by client but Called on the Server only
	[Command]
	public void CmdAttack(string attackName)
	{
		if(Time.time > attacks[attackName].NextFireTime) //empeche les clients de hacker les cooldown et spammer les attack
			RpcActualAttack(attackName);
	}

	/**
	 * Start la coroutine d'attaque
	 * "Coroutine" pour dire simplement qu'on va pas attendre que l'attaque soit fini pour faire autre chose
	 * quand meme!
	 * C'est la moindre des choses.
	 * Mais j'avoue qu'une fonctionnalité comme ça est pratique
	 * */
	[ClientRpc]
	public void RpcActualAttack(string attackName)
	{
		StartCoroutine(attacks[attackName].fire(gameObject));
	}

	/** Hum!!!
	 * Honnetement resync fait la meme chose que le resync de Statsystem mais cette fois pour les attaques
	 * et en beaucoup plus compliqué
	 * On doit notamment sérialiser les objets AttackData avant des les distribuer online
	 * puis les deserialiser.
	 * Bref pour l'idée voir StatSystem.resync 
	 * */
	[Server]
	public void resync()
	{
		foreach (KeyValuePair<string, Attack> entry in attacks)
		{
			Attack attack = entry.Value;

			RpcNewAttack(attack.attackName, attack.cooldown, attack.damage, attack.key, attack.animFloatName, attack.chargeCooldown);
		}
	}

	/**
	 * La encore meme principe que newStat dans StatSystem
	 * */
	public void newAttack(string attackName, float cooldown, float damage, KeyCode key, string animFloat, float chargeCooldown = 0.0F)
	{
		CmdNewAttack(attackName, cooldown, damage, key, animFloat, chargeCooldown);
	}

	/**
	 * meme principe que CmdNewStat dans StatSystem
	 * Oui j'ai la flemme!
	 * Mais en meme temps c'est Exactement la meme chose que dans StatSystem
	 * Sauf que dans StatSystem y a pas le truc de serialisation
	 * donc va voir StatSystem jeune Padawan... allez va voir
	 * */
	[Command]
	public void CmdNewAttack(string attackName, float cooldown, float damage, KeyCode key, string animFloat, float chargeCooldown)
	{
		RpcNewAttack(attackName, cooldown, damage, key, animFloat, chargeCooldown);
	}

	/**
	 * Jeune PadouéWan, StatSystem tu ira voir
	 * pour voir l'exemple RpcNewStat qui fait la meme chose pour les stats
	 * Sans tout le coté compliqué
	 * */
	[ClientRpc]
	public void RpcNewAttack(string attackName, float cooldown, float damage, KeyCode key, string animFloat, float chargeCooldown)
	{
		if (!attacks.ContainsKey(attackName))
		{
			if (attackList)
			{
				Attack[] availableAttacks = attackList.GetComponents<Attack>();
				Attack addMe = null;
				foreach (Attack attack in availableAttacks)
				{
					if (attack.attackName == attackName)
					{
						addMe = attack;
						break;
					}
				}

				if (addMe)
				{
					System.Type type = addMe.GetType();
					Attack new_attack = gameObject.AddComponent(type) as Attack;

					//copie des valeurs des attributs du Component attack pris dans attackList
					System.Reflection.FieldInfo[] fields = type.GetFields();
					foreach (System.Reflection.FieldInfo field in fields)
					{
						field.SetValue(new_attack, field.GetValue(addMe));
					}

					//les attributs "communs" sont ensuite settés
					new_attack.cooldown = cooldown;
					new_attack.key = key;
					new_attack.animFloatName = animFloat;
					new_attack.chargeCooldown = chargeCooldown;
					new_attack.damage = damage;
					attacks.Add(attackName, new_attack);
				}
			}
		}
	}
}
