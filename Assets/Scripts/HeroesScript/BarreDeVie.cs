using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarreDeVie : MonoBehaviour {

	public RectTransform vie;

	public float vieRealWidth;

	// Update is called once per frame
	void Update () 
	{
		var camPos = Camera.main.transform.position;
		transform.LookAt(camPos);
		var eulerAngles = transform.eulerAngles;
		eulerAngles.x += 90.0F;
		//eulerAngles.z = 0.0F;
		eulerAngles.y += 180.0F;
		transform.eulerAngles = eulerAngles;
	}

	public void healthWatcher(float v)
	{
		vie.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, v / 100.0f * vieRealWidth);
	}
}
