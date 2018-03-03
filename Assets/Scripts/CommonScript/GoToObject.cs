using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void CallbackOnReached();

public class GoToObject : MonoBehaviour {
	public float radius = 0.5f;
	public float speed = 6.0f;
	private GameObject _target = null;
	public CallbackOnReached callbackOnReached = null;
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (null != _target)
		{
			Vector3 dir_normalized = Vector3.Normalize(_target.transform.position - transform.position);
			dir_normalized *= speed;
			Rigidbody rb = GetComponent<Rigidbody>();
			if (null != rb)
			{
				rb.MovePosition(transform.position + dir_normalized * Time.deltaTime);
				Debug.Log("move");
			}

			if (null != callbackOnReached && Vector3.Distance(transform.position, _target.transform.position) < radius)
			{
				callbackOnReached();
			}
		}
	}

	public void goToTarget(GameObject target)
	{
		_target = target;
	}

	public void setCallbackOnReached(CallbackOnReached c)
	{
		callbackOnReached = c;
	}
}
