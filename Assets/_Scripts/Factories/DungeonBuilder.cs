using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class DungeonBuilder : MonoBehaviour
{
    private FieldFactory fieldFactory;
    private List<Node> nodes = new List<Node>();
    private List<Node> nodesBuffer = new List<Node>();
    public MeshFilter dungeonMesh;
    private struct Node
    {
        public Vector2 position;
        public Vector2 direction;
        public NodeType type;
    }

    private enum NodeType
    {
        PASSAGE, JUNCTION, ROOM
    }

    public void BuildDungeon(Fields type)
    {
        PlaceRoot(type);
        for (int i = 0; i < 15; i++)
        {
            foreach (Node node in nodes)
            {
                Decide(node, type);
            }
            nodes.Clear();
            nodes = nodesBuffer.ToList();
            nodesBuffer.Clear();
        }
        foreach (Node node in nodes)
        {
            BuildRoom(node, type);
        }
        nodes.Clear();
        nodesBuffer.Clear();
    }

    public void DeleteDungeon()
    {
        nodes.Clear();
        nodesBuffer.Clear();
        fieldFactory.DeleteAllFields();
    }

    private void Decide(Node node, Fields type)
    {
        switch (node.type)
        {
            case NodeType.JUNCTION:
            {
                BuildPassage(node, type);
            } break;
            case NodeType.PASSAGE:
            {
                if (RollWithChanceOneTo(3))
                {
                    BuildJunction(node, type);
                }
                else
                {
                    BuildRoom(node, type);
                }
                 
            } break;
            case NodeType.ROOM:
            {
                BuildPassage(node, type);
            } break;
        }
    }

    private void PlaceRoot(Fields type)
    {
        fieldFactory = GetComponent<FieldFactory>();
        Vector2 position = new Vector2(0, 0);
        fieldFactory.NewField(position, type);
        Node up, down;
        up.position = position;
        down.position = position;
        up.direction = new Vector2(-1, 0);
        down.direction = new Vector2(1, 0);
        up.type = NodeType.JUNCTION;
        down.type = NodeType.JUNCTION;
        nodes.Add(up);
        nodes.Add(down);
    }

    private void BuildPassage(Node node, Fields type)
    {
        int length = Random.Range(5, 12);
        Vector2 position = node.position;
        Vector2 direction = node.direction;

        int i = 0;
        int blockCount = 0;
        bool turned = false;
        while(i < length)
        {
            if (fieldFactory.IsPositionFree(position + direction))
            {
                fieldFactory.NewField(position + direction, type);
                position = position + direction;
                i++;
                blockCount = 0;
                if (!turned && RollWithChanceOneTo(10))
                {
                    if (RollWithChanceOneTo(2))
                    {
                        direction = TurnRight(direction);
                    }
                    else
                    {
                        direction = TurnLeft(direction);
                    }
                    turned = true;
                }
            }
            else
            {
                if (blockCount == 3)
                {
                    break;
                }
                direction = TurnLeft(direction);
                blockCount++;
            }
        }
        if (blockCount == 0)
        {
            Node newNode = new Node();
            newNode.direction = direction;
            newNode.position = position;
            newNode.type = NodeType.PASSAGE;
            nodesBuffer.Add(newNode);
        }
    }

    private void BuildJunction(Node node, Fields fieldType)
    {
        int type = Random.Range(1, 4);
        switch (type)
        {
            case 1:
            {
                CreateJunction(node.position, fieldType, node.direction, TurnLeft(node.direction));
            } break;
            case 2:
            {
                CreateJunction(node.position, fieldType, node.direction, TurnRight(node.direction));
            } break;
            case 3:
            {
                CreateJunction(node.position, fieldType, TurnRight(node.direction), TurnLeft(node.direction));
            } break;
            case 4:
            {
                CreateJunction(node.position, fieldType, node.direction, TurnRight(node.direction), TurnLeft(node.direction));
            } break;
        }
    }

    private void CreateJunction(Vector2 position, Fields type, Vector2 direction1, Vector2 direction2, Vector2 direction3 = default(Vector2))
    {
        if (fieldFactory.IsPositionFree(position + direction1))
        {
            fieldFactory.NewField(position + direction1, type);
            Node node1 = new Node();
            node1.position = position + direction1;
            node1.direction = direction1;
            node1.type = NodeType.JUNCTION;
            nodesBuffer.Add(node1);
        }
        if (fieldFactory.IsPositionFree(position + direction2))
        {
            fieldFactory.NewField(position + direction2, type);
            Node node2 = new Node();
            node2.position = position + direction2;
            node2.direction = direction2;
            node2.type = NodeType.JUNCTION;
            nodesBuffer.Add(node2);
        }
        if (!direction3.Equals(default(Vector2)))
        {
            if (fieldFactory.IsPositionFree(position + direction3))
            {
                fieldFactory.NewField(position + direction3, type);
                Node node3 = new Node();
                node3.position = position + direction3;
                node3.direction = direction3;
                node3.type = NodeType.JUNCTION;
                nodesBuffer.Add(node3);
            }
        }
    }

    private void BuildRoom(Node node, Fields type)
    {
        List<Node> possibleNodes = new List<Node>();
        int width, height;
        Vector2[,] room = InitRoomSpace(node, out width, out height);

        int iterator = 0;
        foreach (Vector2 position in room)
        {
            if (fieldFactory.IsPositionFree(position))
            {
                fieldFactory.NewField(position, type);
                Node newNode = new Node();
                newNode.type = NodeType.ROOM;
                newNode.position = position;
                if (iterator % width == 0)
                {
                    newNode.direction = TurnRight(node.direction);
                    possibleNodes.Add(newNode);
                }
                else if (iterator%width == height)
                {
                    newNode.direction = TurnLeft(node.direction);
                    possibleNodes.Add(newNode);
                }
                else if (iterator >= (width*height) - width)
                {
                    newNode.direction = node.direction;
                    possibleNodes.Add(newNode);
                }
            }
            iterator++;
        }

        int nodesCount = Random.Range(1, 4);
        for (int i = 0; i < nodesCount; i++)
        {
            if (possibleNodes.Count > 0)
            {
                int index = Random.Range(0, possibleNodes.Count - 1);
                Node newNode = possibleNodes[index];
                possibleNodes.RemoveAt(index);
                nodesBuffer.Add(newNode);
            }     
        }
    }

    private Vector2[,] InitRoomSpace(Node node, out int width, out int height)
    {
        width = Random.Range(3, 8);
        height = Random.Range(3, 8);
        Vector2[,] room = new Vector2[width, height];
        int startIndex = Random.Range(0, width - 1);

        Vector2 startPosition = node.position;
        Vector2 left = TurnLeft(node.direction);
        Vector2 right = TurnRight(node.direction);

        for (int i = 0; i < width; i++)
        {
            if (startIndex + i < width)
            {
                room[startIndex + i, 0] = startPosition + i * right;
            }
            if (startIndex - i >= 0)
            {
                room[startIndex - i, 0] = startPosition + i * left;
            }
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 1; j < height; j++)
            {
                room[i, j] = room[i, j - 1] + node.direction;
            }
        }

        return room;
    }

    private Vector2 TurnLeft(Vector2 direction)
    {
        if (direction.x.Equals(0))
        {
            if (direction.y.Equals(1))
            {
                return new Vector2(1, 0);
            }
            else
            {
                return new Vector2(-1, 0);
            }
        }
        else
        {
            if (direction.x.Equals(1))
            {
                return new Vector2(0, -1);
            }
            else
            {
                return new Vector2(0, 1);
            }
        }
    }

    private Vector2 TurnRight(Vector2 direction)
    {
        if (direction.x.Equals(0))
        {
            if (direction.y.Equals(1))
            {
                return new Vector2(-1, 0);
            }
            else
            {
                return new Vector2(1, 0);
            }
        }
        else
        {
            if (direction.x.Equals(1))
            {
                return new Vector2(0, 1);
            }
            else
            {
                return new Vector2(0, -1);
            }
        }
    }

    private bool RollWithChanceOneTo(int chance)
    {
        return chance <= 1 || Random.Range(1, chance) == 1;
    }

    public void CombineDungeon()
    {
        var fields = fieldFactory.GetFields();
        List<MeshFilter> meshes = fields.Select(field => field.GetComponent<MeshFilter>()).ToList();
        CombineInstance[] combine = new CombineInstance[meshes.Count];
        for (int i = 0; i < meshes.Count; i++)
        {
            combine[i].mesh = meshes[i].sharedMesh;
            combine[i].transform = meshes[i].transform.localToWorldMatrix;
        }
        dungeonMesh.mesh = new Mesh();
        dungeonMesh.mesh.CombineMeshes(combine);
        dungeonMesh.gameObject.SetActive(true);

        dungeonMesh.GetComponent<MeshCollider>().sharedMesh = dungeonMesh.mesh;
        dungeonMesh.GetComponent<MeshRenderer>().material = fields[0].GetComponent<MeshRenderer>().material;
        
    }
}
