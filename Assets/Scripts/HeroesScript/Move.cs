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

	public override void OnStartLocalPlayer()
	{
		cam = Camera.main;
		tag = "LocalPlayer";
        anim = transform.GetChild(0).GetComponent<Animator>();
	}

	void Update() 
	{
        
		if(!isLocalPlayer)
			return;

        Vector3 origPos = transform.position;
        Vector3 origRot = body.transform.eulerAngles;

		CharacterController controller = GetComponent<CharacterController>();

		if(Input.GetMouseButton(0))
		{
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			float rayDistance;
			if(groundPlane.Raycast(ray, out rayDistance))
			{
				targetPos = ray.GetPoint(rayDistance);
				body.transform.LookAt(targetPos);
				Vector3 lea = body.transform.eulerAngles;
				lea.x *= 0.0F;
				lea.z *= 0.0F;
				body.transform.eulerAngles = lea;

			}
		}

		if(Input.GetButton("Jump"))
			moveDirection.y = jumpSpeed;

        Vector3 xyz = Vector3.Normalize(targetPos - transform.position);
        moveDirection.x = xyz.x * speed;
        moveDirection.z = xyz.z * speed;

        float movement = Mathf.Sqrt(xyz.x * xyz.x + xyz.z * xyz.z);

        print(movement);
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

}
