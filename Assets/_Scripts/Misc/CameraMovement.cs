using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour 
{
    private Transform target = null;
    private float smoothing = 5.0f;
    private Vector3 offset = Vector3.zero;

    void Awake()
    {
        
    }
	
	void Update () 
    {
	    if (target != null)
	    {
	        Vector3 targetCamPos = target.position + offset;
	        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
	    }
	}

    public void SetTarget(Player player)
    {
        target = player.gameObject.transform;
        offset = transform.position - target.position;
    }

    public void RemoveTarget()
    {
        target = null;
        offset = Vector3.zero;
    }
}
