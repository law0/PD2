using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraObjectScript : MonoBehaviour {
	private Vector3 rot = Vector3.zero;
	public float rotSpeed = 10.0F;
	public GameObject target {get; set;}
	
	// Update is called once per frame
	void Update () 
	{
		if(target == null)
		{
			target = GameObject.FindWithTag("LocalPlayer");
		}
		else
		{
			transform.position = target.transform.position;
			if(Input.GetMouseButton(1))
			{
				rot.y = Input.GetAxis("Mouse X");
				rot *= rotSpeed * Time.deltaTime;
				transform.Rotate(rot);
			}
		}
	}
}

