using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_ground : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision col)
	{
		Move moveScript = col.gameObject.GetComponent<Move>();
		if (moveScript != null)
		{
			Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();
			if (rb != null)
			{
				rb.drag = 10.0F;
			}
		}
	}

	void OnCollisionExit(Collision col)
	{
		Move moveScript = col.gameObject.GetComponent<Move>();
		if (moveScript != null)
		{
			Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();
			if (rb != null)
			{
				rb.drag = 0.0F;
			}
		}
	}
}
