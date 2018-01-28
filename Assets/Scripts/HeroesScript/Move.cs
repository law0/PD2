using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Move : NetworkBehaviour 
{
    private Animator anim;
	public float speed = 6.0F;
	public float jumpSpeed = 0.5F;
	private Vector3 targetPos = Vector3.zero;
	private Vector3 moveDirection = Vector3.zero;
	private bool moveAsked = false;

	public override void OnStartLocalPlayer()
	{
		tag = "LocalPlayer";
	}

	void Start()
	{
        anim = transform.GetComponent<Animator>();
	}

	void Update() 
	{
        
		if(!isLocalPlayer)
			return;

		if(Input.GetMouseButton(0))
		{
			//targetPos is used to move the player
			PlayerUtils.GetMouse3DPosition(ref targetPos);
			PlayerUtils.LookAtY(gameObject, ref targetPos);
			moveAsked = true;
		}

		if (moveAsked) //condition sinon le personnage voudra rejoindre targetPos tout le temps
		{
			//actual movement is done below
			Vector3 xyz = Vector3.Normalize(targetPos - transform.position);
			moveDirection.x = xyz.x * speed;
			moveDirection.z = xyz.z * speed;

			float movement = Mathf.Sqrt(xyz.x * xyz.x + xyz.z * xyz.z);

			anim.SetFloat("moving", movement);

			Rigidbody rb = GetComponent<Rigidbody>();
			rb.MovePosition(transform.position + moveDirection * Time.deltaTime);
			if ((targetPos - transform.position).magnitude < 0.1F)
			{
				stopMove();
			}
		}

	}

    public void stopMove()
    {
        targetPos = transform.position;
		anim.SetFloat("moving", 0.0F);
		moveAsked = false;
    }

	public void jump()
	{
		Rigidbody rb = GetComponent<Rigidbody>();
		rb.velocity = Vector3.up * jumpSpeed;
	}

}
