using UnityEngine;
using System.Collections;
using System.Net.Configuration;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int health;
    private bool damaged = false;
    [HideInInspector]public PlayerMovement playerMovement;
    private CameraMovement camera;
    private PlayerController controller;

	void Awake()
	{
	    controller = FindObjectOfType<PlayerController>();
	    playerMovement = GetComponent<PlayerMovement>();
        camera = FindObjectOfType<CameraMovement>();
        camera.SetTarget(this);
	}
	
	void FixedUpdate() 
    {
	    controller.Damaged(damaged);
        damaged = false;
	}

    public void RemoveCamera()
    {
        camera.RemoveTarget();
    }

    public void TakeDamage(int damage)
    {
        if (health > 0)
        {
            health -= damage;
            damaged = true;
        }
    }
}
