using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class dummy_bullet : MonoBehaviour {
	public float speed;
	public float distanceToLive = 2.0F;
	private Vector3 startingPoint;
	private GameObject originGameObject;

	public void setOriginGameObject(GameObject obj)
	{
		originGameObject = obj;
	}

	public GameObject getOriginGameObject()
	{
		return originGameObject;
	}

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

	void OnTriggerEnter(Collider other)
	{
		var statSystem = other.GetComponent<StatSystem>();
		if (statSystem != null && other.gameObject != originGameObject)
		{
			statSystem.stats["health"].substract(10);
			Destroy(gameObject);
		}
	}
}
