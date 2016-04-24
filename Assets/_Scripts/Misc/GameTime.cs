using UnityEngine;
using System.Collections;

public class GameTime : MonoBehaviour
{
    public static float worldDeltaTime = 0;
    public static float levelDeltaTime = 0;
    private static bool gamePaused = false;
    private static float lastFrameTime;

	void Start ()
	{
	    lastFrameTime = Time.realtimeSinceStartup;
	}
	
	void Update ()
	{
	    worldDeltaTime = Time.realtimeSinceStartup - lastFrameTime;
	    levelDeltaTime = Time.deltaTime;
	    lastFrameTime = Time.realtimeSinceStartup;
	}

    public static void Pause(bool pause)
    {
        gamePaused = pause;
        Time.timeScale = gamePaused ? 0f : 1f;
    }

    public static bool IsPaused()
    {
        return gamePaused;
    }
}
