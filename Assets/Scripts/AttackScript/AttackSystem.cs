using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class AttackSystem : NetworkBehaviour {

	//animator
	private Animator anim;
	public GameObject[] bullets;

	//code en dur a remplacer
	//les attaques sont ici, a remplacer pour chargement "dynamique"
	private Dictionary<string, Attack> attacks = new Dictionary<string, Attack>();

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
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!isLocalPlayer)
			return;

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

	//[Command] functions are Asked by client but Called on the Server only
	[Command]
	public void CmdAttack(string attackName)
	{
		if(Time.time > attacks[attackName].NextFireTime) //empeche les clients de hacker les cooldown et spammer les attack
			RpcActualAttack(attackName);
	}

	[ClientRpc]
	public void RpcActualAttack(string attackName)
	{
		StartCoroutine(attacks[attackName].fire(gameObject));
	}

	[Server]
	public void resync()
	{
		foreach (KeyValuePair<string, Attack> entry in attacks)
		{
			Attack attack = entry.Value;
			MemoryStream stream = new MemoryStream();
			BinaryFormatter formatter = new BinaryFormatter();
			try
			{
				formatter.Serialize(stream, attack.attackData);
			}
			catch (SerializationException e)
			{
				Debug.Log("Serialization Failed : " + e.Message);
			}
			byte[] objectAsBytes = stream.ToArray();
			stream.Close();
			RpcNewAttack(attack.attackName, attack.type, attack.cooldown, attack.damage, attack.key, attack.animFloatName, objectAsBytes, attack.chargeCooldown);
		}
	}

	public void newAttack(string attackName, AttackType type, float cooldown, float damage, KeyCode key, string animFloat, AttackData attackData, float chargeCooldown = 0.0F)
	{
		MemoryStream stream = new MemoryStream();
		BinaryFormatter formatter = new BinaryFormatter();
		try
		{
			formatter.Serialize(stream, attackData);
		}
		catch (SerializationException e)
		{
			Debug.Log("Serialization Failed : " + e.Message);
		}
		byte[] objectAsBytes = stream.ToArray();
		stream.Close();
		CmdNewAttack(attackName, type, cooldown, damage, key, animFloat, objectAsBytes, chargeCooldown);
	}

	[Command]
	public void CmdNewAttack(string attackName, AttackType type, float cooldown, float damage, KeyCode key, string animFloat, byte[] attackData, float chargeCooldown)
	{
		RpcNewAttack(attackName, type, cooldown, damage, key, animFloat, attackData, chargeCooldown);
	}

	[ClientRpc]
	public void RpcNewAttack(string attackName, AttackType type, float cooldown, float damage, KeyCode key, string animFloat, byte[] attackData, float chargeCooldown)
	{
		if (!attacks.ContainsKey(attackName))
		{
			AttackData attackDataDeserialized = new AttackData();
			MemoryStream stream = new MemoryStream();
			stream.Write(attackData, 0, attackData.Length);
			stream.Seek(0, SeekOrigin.Begin);
			BinaryFormatter formatter = new BinaryFormatter();
			try
			{
				attackDataDeserialized = (AttackData)formatter.Deserialize(stream);
			}
			catch (SerializationException e)
			{
				Debug.Log("Deserialization Failed : " + e.Message);
			}
			stream.Close();

			Attack new_attack = gameObject.AddComponent<Attack>();
			new_attack.attackName = attackName;
			new_attack.type = type;
			new_attack.cooldown = cooldown;
			new_attack.key = key;
			new_attack.animFloatName = animFloat;
			new_attack.attackData = attackDataDeserialized;
			new_attack.chargeCooldown = chargeCooldown;
			new_attack.damage = damage;
			attacks.Add(attackName, new_attack);
		}
	}
}
