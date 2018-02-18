using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUtils : MonoBehaviour {

	public static List<Init> PlayerList = new List<Init>();

	public static Plane groundPlane = new Plane(new Vector3(0.0F, 1.0F, 0.0F), new Vector3(0.0F, 0.0F, 0.0F));

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
		foreach (Init i in PlayerList)
		{
			if (Vector3.Distance(i.gameObject.transform.position, pos) < radius)
				returnList.Add(i.gameObject);
		}
		return returnList;
	}

	public static GameObject selectedPlayer()
	{
		return null;
	}
}
