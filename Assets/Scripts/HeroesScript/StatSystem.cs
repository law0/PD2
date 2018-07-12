using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//class permettant de lister les stats d'un joueur : permet de faire statSystem.add("health", 10) par exemple
//voir exemple dans Respawn.cs
//Note: il est franchement préférable de lire le tuto Unity sur le networking et une bonne dizaine d'autre trucs
//dessus au préalable
// Notamment les notions de Command et ClientRpc!
/**
 * La class StatSystem permet
 * De synchro les stats entre server et clients
 * D'enregistrer les stats ajoutée at compile time (bref les stats directement présente dans le préfab)
 * D'ajouteur des stats dynamiquement at runtime (bref pendant le jeu... c'est fou ce que les informaticiens
 * invente des mots pour se la peter...)
 * */
public class StatSystem : NetworkBehaviour
{
	private Dictionary<string, Stat> stats = new Dictionary<string, Stat>();

	public int networkID;

	void Awake()
	{
		//on ajoute les stats présentes de bases dans le player
		Stat[] statsint = GetComponents<Stat>();

		foreach (Stat stat in statsint)
		{
			stats.Add(stat.statName, stat);
		}
	}

	[Server]
	public void die()
	{
		if (!isServer)
			return;
		RpcDie();
		Debug.Log("RpcDie called");
	}

	[ClientRpc]
	public void RpcDie()
	{
		Debug.Log("RpcDie executed");
		Respawn respawn = GetComponent<Respawn>();
		respawn.respawn();
	}

	/** 
	 * Fonction appelées dans Init.cs
	 * Permet de resync les Stats locales avec nos avatars présents chez les autres
	 * Concretement il appelle RpcNewStat avec en parametre les stats qui existent deja localement
	 * RpcNewStat va donc s'executer chez chaque client et tenter de creer les stats passée en parametre
	 * sur nos avatars distants SI ces stats n'existent pas deja
	 * */
	[Server]
	public void resync()
	{
		foreach(KeyValuePair<string, Stat> entry in stats)
		{
			Stat stat = entry.Value;
			RpcNewStat(stat.statName, stat.min, stat.max, stat.val);
		}
	}

	/**
	 * Permet de créer une nouvelle Stat
	 * newStat ne fait qu'appeler CmdNewStat
	 * Le circuit entier est :
	 * appel: Player(Local):newStat()
	 * execution: Player(Local):newStat()
	 * appel: Player(Local):CmdNewStat()
	 * execution: Player(SERVER):CmdNewStat()
	 * appel: Player(SERVER):RpcNewStat()
	 * execution Player(SERVER):RpcNewStat()
	 * execution Player(SERVER):RpcNewStat()
	 *
	 * Un player peut donc s'ajouter n'importe quelle stat
	 * Le but ici est justement de pouvoir dynamiquement s'ajouter des stats
	 * ce qui est essentiel par exemple si on veut changer de Champion entre 2 partie
	 * ou changer les stats du Champion entre 2 partie...
	 * */
	public void newStat(string statname, float min, float max, float val)
	{
		CmdNewStat(statname, min, max, val);
	}

	/**
	 * Puisque c'est une command, cette fonction est 
	 * appelée sur le client
	 * Mais executée sur le serveur
	 */
	[Command]
	public void CmdNewStat(string statname, float min, float max, float val)
	{
		RpcNewStat(statname, min, max, val);
	}

	/**
	 * Fonction créeant réellement la new Stat
	 * Appelée sur le server
	 * Mais executée sur chaque client (y compris sur le server)
	 * Vérifie quand meme que la stat n'existe pas deja
	 * */
	[ClientRpc]
	public void RpcNewStat(string statname, float min, float max, float val)
	{
		if (!stats.ContainsKey(statname))
		{
			Stat new_Stat = gameObject.AddComponent<Stat>();
			new_Stat.statName = statname;
			new_Stat.min = min;
			new_Stat.max = max;
			new_Stat.setValue(val);
			stats.Add(statname, new_Stat);
		}
	}

	/** 
	 * Ici le coeur du StatSystem... boum boum, boum boum, boum booum...
	 * Bref,
	 * Comment ca marche:
	 * Chaque action effectuée sur une stat (add, substract ou setValue)
	 * Peut et sera effectuée à de multiples endroits:
	 * Soit sur le serveur, soit chez un client ou il y a ton avatar, soit chez toi (qui est un client) dear LocalPlayer.
	 * Le serveur et les clients reproduisent la meme chose par principe
	 * Ici l'idée est que add, substract et setValue sont totalement ignorée si elles sont appelées
	 * autre part que sur le server.
	 * Ce qui permet de laisser l'autoritée au server.
	 * add, substract et setValue n'appelle donc assignCheckLimits ci-dessous que sur le server
	 * (L'attribute [Server] permet de s'assurer que la fonction n'est appelée que sur le server)
	 * Quand a la fonction elle meme: pour une Stat nommée statname, on veut lui assigner la valeur v
	 * La fonction va alors vérifiée si stat.min < v < stat.max. Si ce n'est pas le cas v prendra la valeur
	 * de stat.min ou stat.max
	 * Enfin cette fonction appellera RpcUpdateValue avec v, qui permettra de mettre a jour
	 * la valeur sur tous les clients
	 * woila
	 * */
	[Server]
	protected void assignCheckLimits(string statname, float v)
	{
		if (stats.ContainsKey(statname))
		{
			Stat stat = stats[statname];
			if (v < stat.min)
				v = stat.min;
			if (v > stat.max)
				v = stat.max;
			PD2Debug.Log("assignCheckLimits(" + v + ") called on server");
			RpcUpdateValue("health", v);
			PD2Debug.Log("RpcUpdateValue(" + v + ") calling from server");
		}
	}

	/**
	 * Appelée par assignCheckLimits
	 * Mets a jour la valeur de la Stat appelée statname à val sur tous les clients
	 * */
	[ClientRpc]
	private void RpcUpdateValue(string statname, float val)
	{
		PD2Debug.Log("RpcUpdateValue(" + val + ") called from server");
		if (stats.ContainsKey(statname))
		{
			stats[statname].setValue(val);
		}
	}

	public float getValue(string statname)
	{
		if (stats.ContainsKey(statname))
		{
			Stat stat = stats[statname];
			return stat.val;
		}
		return Mathf.NegativeInfinity;
	}

	/**
	 * permet de tenter d'assigner directement une valeur a la stat
	 * voir assignCheckLimits
	 * */
	public void setValue(string statname, float v)
	{
		if (!isServer)
			return;
		PD2Debug.Log("setValue(" + v + ") called on server");
		assignCheckLimits(statname, v);
	}

	/**
	 * permet d'ajouter un certain montant à la valeur de la stat
	 * voir assignCheckLimits
	 * */
	public void add(string statname, float v)
	{
		if (!isServer)
			return;
		PD2Debug.Log("add(" + v + ") called on server");

		if (stats.ContainsKey(statname))
		{
			Stat stat = stats[statname];
			assignCheckLimits(statname, stat.val + v);
		}
	}

	/**
	 * permet de soustraire un certain montant à la valeur de la stat
	 * Pour ne rien cacher c'est surtout celle la qui sera utilisée j'imagine
	 * Parce qu'on ne sait pas faire autre chose que de s'enlever de la vie hein! bande de sauvages!
	 * va voir assignCheckLimits
	 * */
	public void substract(string statname, float v)
	{
		if (!isServer)
			return;
		PD2Debug.Log("substract(" + v + ") called on server");

		if (stats.ContainsKey(statname))
		{
			Stat stat = stats[statname];
			assignCheckLimits(statname, stat.val - v);
		}
	}

	/**
	 * ha! J'avais presque oublié
	 * Cette fonction permet d'enregistré un callback sur la stat -> fonction qui sera appelée chaque fois
	 * que la valeur de la stat change.
	 * Utile pour genre hum... la barre de vie par exemple
	 * Ou pour vérifier qu'un gars est mort et qu'il faut le respawn
	 * des trucs du genre quoi
	 * */
	public void addCallback(string statname, StatCallback callback)
	{
		if (stats.ContainsKey(statname))
		{
			stats[statname].callbackList.Add(callback);
		}
	}

	public void removeCallbackByName(string statname)
	{
		if (stats.ContainsKey(statname))
		{
			stats[statname].callbackList.Clear();
		}
	}
}

