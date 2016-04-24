using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Camera = UnityEngine.Camera;

public class PlayerController : MonoBehaviour 
{
    public static PlayerController instance = null;
    public Image damageImage;
    private Color enabledColor = new Color(1f, 0f, 0f, 0.2f);
    private Color disabledColor = new Color(1f, 0f, 0f, 0f);
    public Player playerPrefab;
    private Player player = null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
	
	void Update () 
    {
	    if (player != null)
	    {
            if (Input.touchCount > 0 || Input.GetButtonDown("Fire1"))
	        {
                player.playerMovement.Move(Input.mousePosition);
                /*
	            Touch touch = Input.GetTouch(0);
	            switch (touch.phase)
	            {
	                case TouchPhase.Began:
	                {
                        player.playerMovement.Move(touch.position);
	                } break;
	            }
                 * */
	        }
	    }
    }

    public void SpawnPlayer(Vector3 position)
    {
        position.y = 1f;
        player = (Player)Instantiate(playerPrefab, position, Quaternion.identity);
    }

    public void Damaged(bool damaged)
    {
        if (damaged)
        {
            damageImage.color = enabledColor;
        }
        else
        {
            damageImage.color = Color.Lerp(damageImage.color, disabledColor, 5 * Time.deltaTime);
        }
    }

    public void RemovePlayer()
    {
        player.RemoveCamera();
        Destroy(player);
        player = null;
    }
}
