using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum Fields
{
    NORMAL, FIRE, STONE, METAL
}

public class FieldFactory : MonoBehaviour 
{
    public static FieldFactory instance = null;
    public Field[] fieldsPrefabs;
    public Field invisibleWallPrefab;
    private Dictionary<Vector2, Field> positionsTaken = new Dictionary<Vector2, Field>();

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

	void Update() 
    {
	    
	}

    public void NewField(Vector2 positionXY, Fields field)
    {
        Field wall;
        if (positionsTaken.TryGetValue(positionXY, out wall))
        {
            Destroy(wall.gameObject);
            Field newField = (Field)Instantiate(fieldsPrefabs[(int)field], MapPositionToScene(positionXY), Quaternion.identity);
            positionsTaken[positionXY] = newField;
        }
        else
        {
            Field newField = (Field)Instantiate(fieldsPrefabs[(int)field], MapPositionToScene(positionXY), Quaternion.identity);
            positionsTaken.Add(positionXY, newField);
        }

        SurroundWithWalls(positionXY);
    }

    private void SurroundWithWalls(Vector2 positionXY)
    {
        Vector2[] positions =
        {
            positionXY + new Vector2(1, 0),
            positionXY + new Vector2(1, 1),
            positionXY + new Vector2(0, 1),
            positionXY + new Vector2(-1, 1),
            positionXY + new Vector2(-1, 0),
            positionXY + new Vector2(-1, -1),
            positionXY + new Vector2(0, -1),
            positionXY + new Vector2(1, -1)
        };

        foreach (Vector2 position in positions)
        {
            if (!positionsTaken.ContainsKey(position))
            {
                NewWall(position);
            }
        }
    }

    private void NewWall(Vector2 positionXY)
    {
        Field newWall = (Field)Instantiate(invisibleWallPrefab, MapPositionToScene(positionXY), Quaternion.identity);
        positionsTaken.Add(positionXY, newWall);
    }

    private Vector3 MapPositionToScene(Vector2 position)
    {
        return new Vector3(3 * position.x, 0, 3 * position.y);
    }

    public void DeleteAllFields()
    {
        foreach (var field in positionsTaken.Values)
        {
            if (field.tag.Equals("Field"))
            {
                Destroy(field.gameObject);
            }
        }
        positionsTaken.Clear();
    }

    public bool IsPositionFree(Vector2 positionXY)
    {
        Field value;
        if (positionsTaken.TryGetValue(positionXY, out value))
        {
            if (value.tag.Equals("Field"))
            {
                return false;
            }
            return true;
        }
        return true;
    }

    public bool FieldOrWall(Vector2 positionXY)
    {
        return positionsTaken[positionXY];
    }

    public List<Vector2> GetPositions()
    {
        var positions = new List<Vector2>();

        foreach (Vector2 position in positionsTaken.Keys)
        {
            if (positionsTaken[position].tag.Equals("Field"))
            {
                positions.Add(MapPositionToScene(position));
            }
        }

        return positions;
    }

    public List<Field> GetFields()
    {
        var fields = positionsTaken.Values.Where(field => field.tag.Equals("Field")).ToList();
        return fields;
    }
}
