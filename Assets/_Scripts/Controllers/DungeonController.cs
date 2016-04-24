using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class DungeonController : MonoBehaviour
{
    public static DungeonController instance = null;
    private DungeonBuilder dungeonBuilder;
    private PlayerController playerController;
    private DungeonFiller dungeonFiller;

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
	    dungeonBuilder = GetComponent<DungeonBuilder>();
	    playerController = FindObjectOfType<PlayerController>();
	    dungeonFiller = GetComponent<DungeonFiller>();
        GenerateDungeon();
    }
	
	void Update() 
    {
	}

    private void GenerateDungeon()
    {
        dungeonBuilder.BuildDungeon((Fields)Random.Range(0, Enum.GetNames(typeof(Fields)).Length));
        while (!InspectDungeon())
        {
            dungeonBuilder.DeleteDungeon();
            dungeonBuilder.BuildDungeon((Fields)Random.Range(0, Enum.GetNames(typeof(Fields)).Length));
        } 
        dungeonBuilder.CombineDungeon();

        playerController.SpawnPlayer(Vector3.zero);

        dungeonFiller.FillDungeon();

        GetComponent<FieldFactory>().DeleteAllFields();
    }

    private bool InspectDungeon()
    {
        GameObject[] fields = GameObject.FindGameObjectsWithTag("Field");
        return fields.Length > 40;
    }


}
