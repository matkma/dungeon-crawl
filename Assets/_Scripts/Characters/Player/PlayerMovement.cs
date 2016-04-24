using System;
using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour 
{
    private float speed = 4.0f;
    private Vector3 destination;
    private Vector3 movement;
    private Animator animator;
    private Rigidbody rigidbody;
    private float cameraRayLength = 100.0f;
    private int floorMask;
    [HideInInspector] public bool isMoving = false;

	void Awake()
	{
	    animator = GetComponent<Animator>();
	    rigidbody = GetComponent<Rigidbody>();
	    floorMask = LayerMask.GetMask("Floor");
        destination = transform.position;
	    movement = transform.position;
	}

	void FixedUpdate() 
    {
	    Go();
	}

    public void Move(Vector3 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit floorHit;

        if (Physics.Raycast(ray, out floorHit, cameraRayLength, floorMask))
        {
            destination = floorHit.point;
            destination.y = transform.position.y;
            isMoving = true;
        }
        AnimateMove();
    }

    public void RotateToEnemy(Vector3 position)
    {
        Quaternion rotation = Quaternion.LookRotation(position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2.0f);
    }

    private void Go()
    {
        if (isMoving)
        {
            movement = destination - transform.position;
            movement = movement.normalized * speed * Time.deltaTime;
            rigidbody.MovePosition(transform.position + movement);
            Rotate();
            isMoving = !(Mathf.Abs(transform.position.x - destination.x) < 0.5f) ||
                       !(Mathf.Abs(transform.position.z - destination.z) < 0.5f);
        }
        else
        {
            animator.SetBool("Moving", isMoving);
        }
    }

    private void Rotate()
    {
        Quaternion rotation = Quaternion.LookRotation(movement);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2.0f);
    }

    private void AnimateMove()
    {
        animator.SetBool("Moving", isMoving);
    }
}
