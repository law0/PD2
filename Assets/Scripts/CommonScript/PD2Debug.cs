using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PD2Debug
{
	public static bool active = false;

	public static void Log(object str)
	{
		if (active)
			Debug.Log(str);
	}
}
