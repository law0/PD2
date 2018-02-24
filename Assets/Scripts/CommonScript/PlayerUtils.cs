using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerUtils : NetworkBehaviour {

	public static List<GameObject> PlayerList = new List<GameObject>();

	public static Plane groundPlane = new Plane(new Vector3(0.0F, 1.0F, 0.0F), new Vector3(0.0F, 0.0F, 0.0F));

	public static GameObject lastSelectedPlayer = null;

		//get where mouse point
	public static void GetMouse3DPosition(ref Vector3 target)
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		float rayDistance;
		if (groundPlane.Raycast(ray, out rayDistance))
		{
			target = ray.GetPoint(rayDistance);
		}
	}

		//rotate transform along Y axis only such as gameObject is looking at target
	public static void LookAtY(GameObject obj, ref Vector3 target)
	{
		obj.transform.LookAt(target);
		Vector3 lea = obj.transform.eulerAngles;
		lea.x *= 0.0F;
		lea.z *= 0.0F;
		obj.transform.eulerAngles = lea;
	}

	public static List<GameObject> getPlayerIn3DRadius(Vector3 pos, float radius)
	{
		var returnList = new List<GameObject>();
		foreach (GameObject player in PlayerList)
		{
			if (Vector3.Distance(player.transform.position, pos) < radius)
				returnList.Add(player);
		}
		return returnList;
	}

	public static GameObject clickedOnPlayer(KeyCode code)
	{
		if (Input.GetKey(code))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			foreach (GameObject player in PlayerList)
			{
				if (Physics.Raycast(ray, out hit) && hit.transform.gameObject == player)
				{
					lastSelectedPlayer = player;
					return player;
				}
			}
		}
		return null;
	}

	public static void spawn(GameObject obj)
	{
		NetworkServer.Spawn(obj);
	}
}
