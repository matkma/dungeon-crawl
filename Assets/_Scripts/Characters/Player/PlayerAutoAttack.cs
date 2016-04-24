using UnityEngine;
using System.Collections;
using System.Collections.Generic;

struct Weapon
{
    public float fireRate;
    public int minDamage;
    public int maxDamage;
};

public class PlayerAutoAttack : MonoBehaviour 
{
    List<GameObject> targets = new List<GameObject>();
    private PlayerMovement movement;
    private float fireTimer = 0.0f;
    private float effectsTimer = 0.1f;
    private float range = 300f;
    private int enemyMask;
    private GameObject firePoint;
    private LineRenderer gunLine;
    private GameObject gun;
    private Light gunLight;
    private Weapon weapon = new Weapon();

	void Awake()
	{
	    movement = FindObjectOfType<PlayerMovement>();
	    firePoint = GameObject.FindWithTag("FirePoint");
	    enemyMask = LayerMask.GetMask("Enemies");
	    gunLine = FindObjectOfType<LineRenderer>();
	    gun = gameObject;
	    gunLight = GetComponentInChildren<Light>();
	    weapon.fireRate = 0.2f;
	    weapon.minDamage = 2;
	    weapon.maxDamage = 3;
	}

	void Update() 
    {
	    if (targets.Count > 0 && !movement.isMoving)
	    {
	        Enemy target = GetClosest();
	        if (target != null)
	        {
                movement.RotateToEnemy(target.transform.position);

                if (fireTimer >= weapon.fireRate && CheckShootingAngle(target.transform.position))
                {
                    Shoot(target);
                }
	        }    
	    }
	    else
	    {
	        
	    }

	    fireTimer += Time.deltaTime;
        DisableVisualEffects();
    }

    private Enemy GetClosest()
    {
        float closestDistance = 300.0f;
        GameObject closestEnemy = null;

        foreach (var target in targets)
        {
            if (target != null)
            {
                Vector3 vector = target.transform.position - transform.position;
                float distance = vector.sqrMagnitude;
                if (distance < closestDistance && target.GetComponent<Enemy>().health > 0)
                {
                    closestDistance = distance;
                    closestEnemy = target;
                }
            }
        }

        if (closestEnemy != null) return closestEnemy.GetComponent<Enemy>();
        return null;
    }

    private bool CheckShootingAngle(Vector3 target)
    {
        var reference = (transform.right).normalized;
        target = (target - transform.position).normalized;

        var angle = Vector3.Angle(reference, target);

        return angle <= 50.0f;
    }

    private void Shoot(Enemy target)
    {
        fireTimer = 0f;
        gunLine.enabled = true;
        gunLine.SetPosition(0, firePoint.transform.position);
        gunLight.enabled = true;

        Ray ray = new Ray();
        ray.origin = firePoint.transform.position;
        Vector3 targetPos = target.transform.position + new Vector3(Random.Range(0f, 0.5f),
                                                                Random.Range(0f, 0.5f),
                                                                Random.Range(0f, 0.5f));
        ray.direction = targetPos - firePoint.transform.position;
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, range, enemyMask))
        {
            Enemy hitted = hit.collider.GetComponent<Enemy>();
            if (hitted != null && hitted.health > 0)
            {
                hitted.TakeDamage(Random.Range(weapon.minDamage, weapon.maxDamage + 1));
            }
            gunLine.SetPosition(1, hit.point);
        }
        else
        {
            gunLine.SetPosition(1, ray.origin + ray.direction * range);
        }
    }

    private void DisableVisualEffects()
    {
        if (fireTimer >= effectsTimer && gunLine != null)
        {
            gunLine.enabled = false;
            gunLight.enabled = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Enemy"))
        {
            targets.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("Enemy"))
        {
            if (targets.Contains(other.gameObject))
            {
                targets.Remove(other.gameObject);
            }
        }
    }
}
