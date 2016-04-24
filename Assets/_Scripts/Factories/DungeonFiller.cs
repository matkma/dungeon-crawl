using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class DungeonFiller : MonoBehaviour
{
    private List<Vector3> positions;
    public Enemy[] enemiesPrefabs;

    void Awake()
    {
        
    }

    void Update()
    {

    }

    private void LoadDungeon(out int limit)
    {
        positions = new List<Vector3>();
        Field[] fields = FindObjectsOfType<Field>();
        limit = (int)Mathf.Floor(fields.Length * 0.05f);
        foreach (Field field in fields)
        {
            if (field != null && field.tag.Equals("Field"))
            {
                if (field.transform.position.sqrMagnitude >= 225.0f)
                {
                    positions.Add(field.transform.position);
                }
            }
        }
    }

    public void FillDungeon()
    {
        int maxEnemies;
        LoadDungeon(out maxEnemies);

        for (int i = 0; i < maxEnemies; i++)
        {
            SpawnEnemy(GeneratePosition());
        }
    }

    private void SpawnEnemy(Vector3 position)
    {
        position.y = 1.0f;
        Instantiate(enemiesPrefabs[0], position, Quaternion.EulerAngles(new Vector3(0, Random.Range(0f, 360f), 0)));
    }

    private Vector3 GeneratePosition()
    {
        Vector3 position = positions[Random.Range(0, positions.Count)];
        position += new Vector3(Random.Range(-1.2f, 1.2f), 0f, Random.Range(-1.2f, 1.2f));
        return position;
    }
}
