using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
Script to place objects around the mesh
*/

public enum Location { CEILING, FLOOR, WALL, FLOOR_AND_CEILING, FLOOR_AND_WALL, CEILING_AND_WALL, ALL }

public class ObjectPlacer : MonoBehaviour
{
    public List<ObjectTobePlaced> objects;
    private Dictionary<Vector3, GameObject> positionsUsed;

    public void Place(CellularDungeonLayer[] dungeon, Vector3[] vertices, Vector3[] normals)
    {
        positionsUsed = new Dictionary<Vector3, GameObject>();
        int height = dungeon[0].height;
        int width = dungeon[0].width;
        int length = dungeon[0].length;

        foreach (ObjectTobePlaced o in objects)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < length; z++)
                    {
                        if (!positionsUsed.ContainsKey(new Vector3(x, y, z)))
                        {
                            switch (o.Place)
                            {
                                case Location.CEILING:
                                    Ceiling(dungeon, vertices, normals, x, y, z, o);
                                    break;
                                case Location.FLOOR:
                                    Floor(dungeon, vertices, normals, x, y, z, o);
                                    break;
                                case Location.WALL:
                                    Wall(dungeon, vertices, normals, x, y, z, o);
                                    break;
                                case Location.CEILING_AND_WALL:
                                    Ceiling(dungeon, vertices, normals, x, y, z, o);
                                    Wall(dungeon, vertices, normals, x, y, z, o);
                                    break;
                                case Location.FLOOR_AND_WALL:
                                    Floor(dungeon, vertices, normals, x, y, z, o);
                                    Wall(dungeon, vertices, normals, x, y, z, o);
                                    break;
                                case Location.FLOOR_AND_CEILING:
                                    Ceiling(dungeon, vertices, normals, x, y, z, o);
                                    Floor(dungeon, vertices, normals, x, y, z, o);
                                    break;
                                case Location.ALL:
                                    Ceiling(dungeon, vertices, normals, x, y, z, o);
                                    Floor(dungeon, vertices, normals, x, y, z, o);
                                    Wall(dungeon, vertices, normals, x, y, z, o);
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }

    public void Ceiling(CellularDungeonLayer[] dungeon, Vector3[] vertices, Vector3[] normals, int x, int y, int z, ObjectTobePlaced o)
    {
        float r = Random.Range(0.0f, 100.0f);
        if (dungeon[y].Cells[x, z].isAlive && y == dungeon.Length - 1 && r > 100 - o.SpawnRate)
        {
            GameObject newObj = Instantiate(o.GameObject, vertices[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length], Quaternion.Euler(normals[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length])); //buscar a normal do vertice para rotação
            newObj.transform.parent = this.transform;
            positionsUsed.Add(new Vector3(x, y, z), newObj);
        }
    }

    public void Floor(CellularDungeonLayer[] dungeon, Vector3[] vertices, Vector3[] normals, int x, int y, int z, ObjectTobePlaced o)
    {
        float r = Random.Range(0.0f, 100.0f);
        if (dungeon[y].Cells[x, z].isAlive && y == 0 && r > 100 - o.SpawnRate)
        {
            GameObject newObj = Instantiate(o.GameObject, vertices[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length], Quaternion.Euler(normals[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length]));
            newObj.transform.parent = this.transform;
            positionsUsed.Add(new Vector3(x, y, z), newObj);
        }
    }

    public void Wall(CellularDungeonLayer[] dungeon, Vector3[] vertices, Vector3[] normals, int x, int y, int z, ObjectTobePlaced o)
    {
        float r = Random.Range(0.0f, 100.0f);
        if (dungeon[y].Cells[x, z].isAlive && (y != 0 && y != dungeon.Length - 1) && r > 100 - o.SpawnRate)
        {
            GameObject newObj = Instantiate(o.GameObject, vertices[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length], Quaternion.Euler(normals[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length]));
            newObj.transform.parent = this.transform;
            positionsUsed.Add(new Vector3(x, y, z), newObj);
        }
    }
}

[System.Serializable]
public struct ObjectTobePlaced
{
    public GameObject GameObject;
    public Location Place;
    public int SpawnRate;
}
