using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Move : NetworkBehaviour 
{
    private Animator anim;
	public float speed = 6.0F;
	public float jumpSpeed = 8.0F;
	public float gravity = 20.0F;
	private Vector3 targetPos = Vector3.zero;
	private Vector3 moveDirection = Vector3.zero;

    private Plane groundPlane = new Plane(new Vector3(0.0F, 1.0F, 0.0F), new Vector3(0.0F, 0.0F, 0.0F));
	public Camera cam;
	public GameObject body;

	//called on start if this object is local player
	public override void OnStartLocalPlayer()
	{
		cam = Camera.main;
		tag = "LocalPlayer";
        anim = transform.GetComponent<Animator>();
	}

	void Update() 
	{
        
		if(!isLocalPlayer)
			return;

		CharacterController controller = GetComponent<CharacterController>();

		if(Input.GetMouseButton(0))
		{
			//targetPos is used to move the player
			getMouse3DPosition(ref targetPos);
			lookAtY(ref targetPos);
		}

		if (Input.GetButton("Jump"))
		{
			jump();
		}

		if (Input.GetKeyDown(KeyCode.A))
		{
			//if A (attack) we want to look at the mouse position but not update targetPos (not to move)
			Vector3 attackDir = targetPos;
			getMouse3DPosition(ref attackDir); //getMouse3DPosition may or may not update the vector passed to it (if mouse is out of screen)
			//if attackDir is not updated, it will not rotate the Player because attackDir is a copy of targetPos
			lookAtY(ref attackDir);
		}

		//actual movement is done below
        Vector3 xyz = Vector3.Normalize(targetPos - transform.position);
        moveDirection.x = xyz.x * speed;
        moveDirection.z = xyz.z * speed;

        float movement = Mathf.Sqrt(xyz.x * xyz.x + xyz.z * xyz.z);

        anim.SetFloat("moving", movement);

        if (controller.isGrounded)
            moveDirection.y = 0.0F;
        else
		    moveDirection.y -= gravity * Time.deltaTime;
		controller.Move(moveDirection * Time.deltaTime);

	}

    public void stopMove()
    {
        targetPos = transform.position;
    }

	//get where mouse point
	public void getMouse3DPosition(ref Vector3 target)
	{
		Ray ray = cam.ScreenPointToRay(Input.mousePosition);
		float rayDistance;
		if (groundPlane.Raycast(ray, out rayDistance))
		{
			target = ray.GetPoint(rayDistance);
		}
	}

	//rotate transform along Y axis only such as gameObject is looking at target
	public void lookAtY(ref Vector3 target)
	{
		body.transform.LookAt(target);
		Vector3 lea = body.transform.eulerAngles;
		lea.x *= 0.0F;
		lea.z *= 0.0F;
		body.transform.eulerAngles = lea;
	}

	public void jump()
	{
		moveDirection.y = jumpSpeed;
	}

}
