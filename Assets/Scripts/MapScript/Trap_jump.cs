using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_jump : MonoBehaviour {
	public float timeToDeactivation = 1.0F;
	private float nextDeactivationTime = 0.0F;
	public GameObject target;
	public float timeInAir = 1.0F;

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (nextDeactivationTime < Time.time)
			transform.GetComponent<Animator>().SetBool("activated", false);
	}

	void OnTriggerEnter(Collider other)
	{
		Move moveScript = other.gameObject.GetComponent<Move>();
		if (moveScript != null) //if it has the Move script then it's a player
		{
			transform.GetComponent<Animator>().SetBool("activated", true);
			nextDeactivationTime = Time.time + timeToDeactivation;
			moveScript.stopMove();
			Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
			Ballistique.Launch(rb, target, timeInAir, true);
		}
	}
}
