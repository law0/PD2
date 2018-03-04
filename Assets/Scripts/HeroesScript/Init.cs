using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/** 1) Doit etre présent parmi les components du prefab Player
 * Permet d'initialiser le player:
 * Rempli les Stats -> voir Stat.cs et StatSystem.cs
 * Rempli les Attaques -> voir Attack.cs et AttackSystem.cs
 * Enregistre le Player dans la liste des Player -> voir PlayerUtils.cs
 * */



public class Init : NetworkBehaviour
{
	//ne pas oublier de mettre les bullets dans le tableau Spawnable du NetworkManager!!! et dans le tableau bullets de AttackSystem
	//Rempli les stats et les attaques
	//Note: les stats et attaques peuvent aussi être directement renseignées dans le préfab
	public override void OnStartLocalPlayer()
	{
		tag = "LocalPlayer";

		StatSystem statSystem = GetComponent<StatSystem>();
		if (null != statSystem)
		{
			//vu que tous les player auront de la santé, on ajoute la stat health directement dans le prefab
			//statSystem.newStat("health", 0, 100, 100); //met la valeur a 100 au début
			statSystem.newStat("armor", 0, 50, 10);
		}

		AttackSystem attackSystem = GetComponent<AttackSystem>();

		if (null != attackSystem)
		{
			//Exemple
			var castData = new AttackData();
			castData.bulletIndex = 0; //cherchera dans attackSystem.bullets[0];

			var accelData = new AttackData();
			accelData.accelSpeed = 12.0F;

			var dashData = new AttackData();
			dashData.dashSpeed = 18.0F;
			dashData.dashDistance = 5.0F;
			dashData.dashDamageRadius = 0.5F;

			var meleeData = new AttackData();
			meleeData.meleeDamageRadius = 0.5F;

			var tpData = new AttackData();
			tpData.tpDistance = 5.0F;

			var cibleeData = new AttackData();
			cibleeData.cibleeRadius = 20.0F;
			cibleeData.bulletIndex = 1; //cherchera dans attackSystem.bullets[1];

			attackSystem.newAttack("Simple cast", AttackType.CAST, 0.5F, 10.0F, KeyCode.A, "cast_attack", castData);
			//attackSystem.newAttack("Simple Acceleration", AttackType.ACCEL, 1.0F, 0.0F, KeyCode.Z, "accel_attack", accelData);
			//attackSystem.newAttack("Simple Dash", AttackType.DASH, 1.0F, 5.0F, KeyCode.E, "dash_attack", dashData);
			//attackSystem.newAttack("Simple Cac", AttackType.MELEE, 1.0F, 5.0F, KeyCode.R, "melee_attack", meleeData);
			//attackSystem.newAttack("Simple Tp", AttackType.TP, 5.0F, 0.0F, KeyCode.T, "tp_attack", tpData);
			//tous les players auront aussi une attaque cible (auto attack) de base -> ajoute direct dans le prefab
			//attackSystem.newAttack("Simple cast ciblee", AttackType.CIBLEE, 1.0F, 5.0F, KeyCode.Mouse0, "cast_attack", cibleeData);
		}

		CmdReinit(); //to resync with Stats and Attacks of other players
	}

	// Use this for initialization
	void Awake()
	{
		PlayerUtils.PlayerList.Add(gameObject);
	}

	/** Cette fonction est temporaire
	 * Appelée à chaque new Player qui se connecte
	 * Elle appelle les fonctions resync de chaque StatSystem et AttackSystem de chaque player LOCALEMENT
	 * ces fonctions resync vont ensuite bah resync les Stat et Attack des player distant avec
	 * leur avatar local
	 * Dans le futur -> cette fonction ne devra être appelée qu'une fois au début du match
	 * */
	[Command]
	public void CmdReinit()
	{
		reinit();
		RpcCallbacksOnHealth(); //du coup vu qu'on a resync, chaque client doit remettre ses callback
	}

	[Server]
	public static void reinit()
	{
		if (! NetworkServer.active)
			return;
		
		if (MatchManager.instance && !MatchManager.instance.MatchStarted)
		{
			foreach (GameObject player in PlayerUtils.PlayerList)
			{
				StatSystem pStatSystem = player.GetComponent<StatSystem>();
				AttackSystem pAttackSystem = player.GetComponent<AttackSystem>();

				pStatSystem.resync();
				pAttackSystem.resync();

				pStatSystem.die();
			}
		}
	}

	/**
	 * Vu que les Stats sont resync au debut du match il faut aussi reconnecté les callbacks/watchers des stats
	 * Note: A faire: vérifier que si un watcher est déjà connecté, il ne faut pas le reconnecté
	 * */
	[ClientRpc]
	public void RpcCallbacksOnHealth()
	{
		StatSystem statSystem = GetComponent<StatSystem>();
		if (null != statSystem)
		{
			Respawn respawn_script = GetComponent<Respawn>();
			if (null != respawn_script)
			{
				statSystem.addCallback("health", respawn_script.healthWatcher); //sera appelé chaque fois que la valeur de health changera
			}
		}
	}
}
