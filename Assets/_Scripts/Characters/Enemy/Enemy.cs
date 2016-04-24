using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public int health;
    private bool disappearing;
    private Animator animator;

	void Awake()
	{
	    animator = GetComponent<Animator>();
	    disappearing = false;
	}
	
	void Update() 
    {
	    if (disappearing)
	    {
	        transform.Translate(Vector3.down * Time.deltaTime);
	    }
	}

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            animator.SetTrigger("Death");
            Invoke("Die", 2.0f);
        }
    }

    private void Die()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        disappearing = true;
        Destroy(gameObject, 2.0f);
    }
}
