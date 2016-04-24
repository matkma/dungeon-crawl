using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    public float speed;
    public float aggro;
    private float sqrAggro;
    private Vector3 destination;
    private Vector3 movement;
    private Animator animator;
    private Rigidbody rigidbody;
    private GameObject target;
    private Player player;
    private Enemy self;
    private EnemyAttack attack;
    [HideInInspector] public bool isMoving = false;


	void Awake()
	{
	    sqrAggro = aggro*aggro;
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
	    player = FindObjectOfType<Player>();
        destination = transform.position;
        movement = transform.position;
	    self = GetComponent<Enemy>();
	    attack = GetComponent<EnemyAttack>();
    }
	
	void FixedUpdate()
	{
	    if (self.health > 0)
	    {
            DetectPlayer();
            Move();
	    }
	}

    private void Move()
    {
        if (target != null)
        {
            destination = target.transform.position;
            if (isMoving)
            {
                movement = destination - transform.position;
                movement = movement.normalized * speed * Time.deltaTime;
                rigidbody.MovePosition(transform.position + movement);
                Rotate();
            }

            animator.SetBool("Move", isMoving);
        }
    }

    private void Rotate()
    {
        Quaternion rotation = Quaternion.LookRotation(movement);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2.0f);
    }

    private void DetectPlayer()
    {
        var result = player.transform.position - transform.position;
        var sqrMag = result.sqrMagnitude;

        if (sqrMag <= sqrAggro)
        {
            target = player.gameObject;
            isMoving = true;
            
            attack.CheckPlayerDistance(sqrMag);
        }
        else if (sqrMag >= 2*sqrAggro)
        {
            target = null;
            isMoving = false;
        }
    }
}
