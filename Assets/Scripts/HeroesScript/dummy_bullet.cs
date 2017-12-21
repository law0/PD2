using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dummy_bullet : MonoBehaviour {
	public float speed;
	public float timeToLive = 1.0F;

	void Start()
	{
		//destroy the object (this bullet) after timeToLive seconds
		Destroy(gameObject, timeToLive);
	}

	// Update is called once per frame
	void Update () 
	{
		transform.position += transform.forward * speed;
	}
}
