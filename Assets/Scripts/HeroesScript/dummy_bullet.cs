using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dummy_bullet : MonoBehaviour {
	public float speed;
	public float distanceToLive = 2.0F;
	private Vector3 startingPoint;

	void Start()
	{
		startingPoint = transform.position;
		gameObject.GetComponent<Rigidbody>().velocity = transform.forward * speed;
	}

	// Update is called once per frame
	void Update () 
	{
		if(Vector3.Distance(startingPoint, transform.position) > distanceToLive)
			Destroy(gameObject);
	}
}
