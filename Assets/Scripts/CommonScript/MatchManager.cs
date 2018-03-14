using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using cakeslice;



/*
 * J'ai fait ce script un peu vite fait, mais ca marche bien
 * MatchManager répartit les joueurs en 2 équipe, un peu basiquement pour l'instant (toi dans une, toi dans l'autre, toi dans l'une...)
 * Il gere aussi (surtout) "le début du match"
 * Basiquement on fait respawn tout le monde pour l'instant
 * */
public class MatchManager : NetworkBehaviour
{
	public bool hasTeams = true;

	public List<int> teamA; //BLEU
	public List<int> teamB; //ROUGE

	private float startTime = -1.0F;

	[SyncVar] //alors oui j'ai dit "JAMAIS de SyncVar!" mais il la... c'est juste pour afficher le délai...
	public float delayBeforeStart = -1.0F;

	public GameObject teamASpawnPoint = null;
	public GameObject teamBSpawnPoint = null;

	[SyncVar] //et la pour dire que le match a démarré...
	private bool matchStarted = false;
	public bool MatchStarted
	{
		get
		{
			return matchStarted;
		}
	}
	 
	[SyncVar] //Bon d'accord je l'utilise beaucoup ici! Mais SyncVar est pratique pour des trucs simples
	private bool matchStartedDelay = false;
	public bool MatchStartedDelay
	{
		get
		{
			return matchStartedDelay;
		}
	}

	public static MatchManager instance = null;

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(this); //un seul match manager
		}
	}

	void Update()
	{
		if (!isServer)
			return;

		//faire bien attention a ce que startTime soit inférieur à 0 à l'instantiation
		if (startTime > 0.0F && startTime < Time.time && ! matchStarted)
		{
			if (hasTeams)
				makeTeams();
			
			Respawn.respawnEveryone();

			matchStarted = true;
		}
		else if (startTime > Time.time)
		{
			delayBeforeStart = startTime - Time.time;
		}
	}


	public bool isInTeamA(GameObject player)
	{
		int index = PlayerUtils.PlayerList.IndexOf(player);
		return teamA.Contains(index);
	}

	public bool isInTeamB(GameObject player)
	{
		int index = PlayerUtils.PlayerList.IndexOf(player);
		return teamB.Contains(index);
	}

	[Server]
	public void startMatch(float delay)
	{
		if (!isServer)
			return;
		startTime = Time.time + delay;
		matchStartedDelay = true;
	}

	[Server]
	private void makeTeams()
	{
		if (!isServer)
			return;

		int i = 0;
		bool teamSwitch = false;
		foreach (GameObject player in PlayerUtils.PlayerList)
		{
			if (teamSwitch)//bien sur cette répartition va changer
			{
				RpcAddToTeamB(i);
				teamSwitch = false;
			}
			else
			{
				RpcAddToTeamA(i);
				teamSwitch = true;
			}
			++i;
		}
	}

	//il faut que chaque client ajoute les joueurs dans la bonne equipe dans leur version du jeu
	[ClientRpc]
	private void RpcAddToTeamA(int index)
	{
		GameObject playerBody = PlayerUtils.getBodyMesh(index);
		if (playerBody)
		{
			Outline outline = playerBody.AddComponent<Outline>();
			if (outline)
				outline.color = 2; //2 = blue color
		}
		else
		{
			Debug.Log("Body mesh not found (teamA)");
		}
		teamA.Add(index);
	}

	[ClientRpc]
	private void RpcAddToTeamB(int index)
	{
		GameObject playerBody = PlayerUtils.getBodyMesh(index);
		if (playerBody)
		{
			Outline outline = playerBody.AddComponent<Outline>();
			if (outline)
				outline.color = 0; //0 = red color
		}
		else
		{
			Debug.Log("Body mesh not found (teamB)");
		}
		teamB.Add(index);
	}
}

