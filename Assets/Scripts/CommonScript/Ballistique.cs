using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ballistique : MonoBehaviour {
	public static void Launch(Rigidbody rb, GameObject target, float timeInAir, bool setZeroDrag = false)
	{

		if(setZeroDrag)
			rb.drag = 0.0F;

		Vector3 direction = target.transform.position - rb.gameObject.transform.position;
		direction.y /= timeInAir;
		direction.y += timeInAir * 0.5F * -Physics.gravity.y;
		direction.x /= timeInAir;
		direction.z /= timeInAir;
		rb.AddForce(direction, ForceMode.VelocityChange);
	}
}
