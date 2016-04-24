using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    public float range;
    public float attackRate;
    public int minDamage;
    public int maxDamage;
    private float sqrRange;
    private float attackTimer;
    private bool attackAllowed = false;
    private Enemy self;
    private Animator animator;
    private Player player;

	void Awake()
	{
	    sqrRange = range*range;
	    attackTimer = 0;
        self = GetComponent<Enemy>();
	    player = FindObjectOfType<Player>();
	    animator = GetComponent<Animator>();
	}
	
	void FixedUpdate()
	{
	    if (self.health > 0)
	    {
            attackTimer += Time.deltaTime;

            if (attackAllowed)
            {
                if (attackTimer >= attackRate)
                {
                    Attack();
                }
            }
	    }   
	}

    public void CheckPlayerDistance(float distance)
    {
        attackAllowed = distance <= sqrRange;
    }

    private void Attack()
    {
        animator.SetTrigger("Attack");
        player.TakeDamage(Random.Range(minDamage, maxDamage + 1));
        attackTimer = 0;
    }
}
