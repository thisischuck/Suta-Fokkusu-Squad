using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* 
Script to place objects around the mesh 
     
16/10/2018 
    Marcio - normals from objects are still not affected from the vertice's normal. 
18/10/2018 
    Marcio - object's normal take the same value as the vertice's normal, also created an exception for stalactites tagged objects 
*/

public enum Location { CEILING, FLOOR, WALL, FLOOR_AND_CEILING, FLOOR_AND_WALL, CEILING_AND_WALL, ALL }

public class ObjectPlacer : MonoBehaviour
{
	public List<ObjectTobePlaced> objects;
	public float chunkSizeX = 60.0f, chunkSizeZ = 60.0f;
	public float chunkRenderDistance;
	private Dictionary<Vector3, GameObject> positionsUsed;
	private Dictionary<Vector2, Chunk> objectChunks;
	private CellularAutomata cellular;
	private Vector2 currentChunk;
	private Transform player;
	public GameObject obj;
	int height;
	int width;
	int length;

	public void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player").transform;
		cellular = GetComponent<CellularAutomata>();
		objectChunks = new Dictionary<Vector2, Chunk>();
		for (float z = 0; z < cellular.length * cellular.spacing; z += chunkSizeZ)
		{
			for (float x = 0; x < cellular.width * cellular.spacing; x += chunkSizeX)
			{
				GameObject chunkObject = new GameObject("Chunk: " + x + ", " + z);
				Vector3 center = new Vector3(x + chunkSizeX / 2.0f, cellular.height / 2.0f, z + chunkSizeX / 2.0f);
				chunkObject.transform.position = center;
				chunkObject.transform.parent = this.transform;
				Bounds bounds = new Bounds(center, new Vector3(chunkSizeX, cellular.height * 30.0f, chunkSizeZ));

				Chunk chunk = new Chunk();
				chunk.gameObject = chunkObject;
				chunk.bounds = bounds;
				objectChunks.Add(new Vector2(x, z), chunk);
			}
		}
	}

	public void Update()
	{
		foreach (Chunk chunk in objectChunks.Values)
		{
			/*float distanceFromChunk = Vector2.Distance(new Vector2(player.position.x, player.position.z), 
                new Vector2(chunk.transform.position.x, chunk.transform.position.z)); 
            bool inRange = distanceFromChunk <= chunkRenderDistance;*/

			//chunk.SetActive(inRange); 
			/*Vector3 screenPoint = Camera.main.WorldToViewportPoint(chunk.gameObject.transform.position); 
            bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1; 
            chunk.gameObject.SetActive(onScreen);*/
			float farPlane = Camera.main.farClipPlane;
			Camera.main.farClipPlane = chunkRenderDistance;
			Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
			chunk.gameObject.SetActive(GeometryUtility.TestPlanesAABB(planes, chunk.bounds));
			Camera.main.farClipPlane = farPlane;
		}
	}

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
						if (dungeon[y].Cells[x, z].isAlive && !positionsUsed.ContainsKey(new Vector3(x, y, z)))
						{
							currentChunk = new Vector2(Mathf.FloorToInt(x * cellular.spacing / chunkSizeX) * chunkSizeX,
								Mathf.FloorToInt(z * cellular.spacing / chunkSizeZ) * chunkSizeZ);
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

		QuadCenterVertex(dungeon, vertices, normals, obj);
	}

	public void Ceiling(CellularDungeonLayer[] dungeon, Vector3[] vertices, Vector3[] normals, int x, int y, int z, ObjectTobePlaced o)
	{
		float r = Random.Range(0.0f, 100.0f);
		if (y == dungeon.Length - 1 && r > 100 - o.SpawnRate)
		{
			GameObject newObj = Instantiate(o.GameObject, vertices[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length], Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f))); //buscar a normal do vertice para rotação 
			newObj.transform.parent = objectChunks[currentChunk].gameObject.transform;
			if (newObj.tag != "Stalactite")
				newObj.transform.rotation = Quaternion.FromToRotation(newObj.transform.up, normals[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length]) * newObj.transform.rotation;
			positionsUsed.Add(new Vector3(x, y, z), newObj);
		}
	}

	public void Floor(CellularDungeonLayer[] dungeon, Vector3[] vertices, Vector3[] normals, int x, int y, int z, ObjectTobePlaced o)
	{
		float r = Random.Range(0.0f, 100.0f);
		if (y == 0 && r > 100 - o.SpawnRate)
		{
			GameObject newObj = Instantiate(o.GameObject, vertices[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length], Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f)));
			newObj.transform.parent = objectChunks[currentChunk].gameObject.transform;
			newObj.transform.rotation = Quaternion.FromToRotation(newObj.transform.up, normals[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length]) * newObj.transform.rotation;
			positionsUsed.Add(new Vector3(x, y, z), newObj);
		}
	}

	public void Wall(CellularDungeonLayer[] dungeon, Vector3[] vertices, Vector3[] normals, int x, int y, int z, ObjectTobePlaced o)
	{
		float r = Random.Range(0.0f, 100.0f);
		if ((y != 0 && y != dungeon.Length - 1) && r > 100 - o.SpawnRate)
		{
			GameObject newObj = Instantiate(o.GameObject, vertices[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length], Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f)));
			newObj.transform.parent = objectChunks[currentChunk].gameObject.transform;
			newObj.transform.rotation = Quaternion.FromToRotation(newObj.transform.up, normals[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length]) * newObj.transform.rotation;
			positionsUsed.Add(new Vector3(x, y, z), newObj);
		}
	}

	public void QuadCenterVertex(CellularDungeonLayer[] dungeon, Vector3[] vertices, Vector3[] normals, GameObject o)
	{
		List<Vector3> QuadVertices = new List<Vector3>();
		List<Vector3> QuadNormals = new List<Vector3>();

		height = dungeon[0].height;
		width = dungeon[0].width;
		length = dungeon[0].length;

		//Calculate QuadCenterVertex 
		for (int y = 0; y < height; y++)
		{
			for (int z = 0; z < length; z++)
			{
				for (int x = 0; x < width; x++)
				{
					if (y == 0 && x < width - 1 && z < length - 1)
					{
						Vector3 BottomLeft = vertices[(x + 0) + (z + 0) * width + y * width * length];
						Vector3 BottomRight = vertices[(x + 1) + (z + 0) * width + y * width * length];
						Vector3 TopLeft = vertices[(x + 0) + (z + 1) * width + y * width * length];
						Vector3 TopRight = vertices[(x + 1) + (z + 1) * width + y * width * length];

						Vector3 randomPoint = new Vector3(Random.Range(BottomLeft.x, BottomRight.x), 0, Random.Range(BottomLeft.z, TopRight.z));

						Vector3 topVector = TopLeft - TopRight;
						Vector3 botVector = BottomLeft - BottomRight;



						randomPoint.y = Mathf.Lerp(Mathf.Lerp(BottomLeft.y, BottomRight.y, Vector2.Distance(new Vector2(BottomLeft.x, BottomLeft.z), new Vector2(randomPoint.x, randomPoint.z))),
							Mathf.Lerp(TopLeft.y, TopRight.y, Vector2.Distance(new Vector2(TopLeft.x, TopLeft.z), new Vector2(randomPoint.x, randomPoint.z))),
							Vector2.Distance(new Vector2(botVector.x, botVector.z), new Vector2(topVector.x, topVector.z)));

						QuadVertices.Add(randomPoint);

						//NORMALS 
						Vector3 nBottomLeft = normals[(x + 0) + (z + 0) * width + y * width * length];
						Vector3 nBottomRight = normals[(x + 1) + (z + 0) * width + y * width * length];
						Vector3 nTopLeft = normals[(x + 0) + (z + 1) * width + y * width * length];
						Vector3 nTopRight = normals[(x + 1) + (z + 1) * width + y * width * length];

						Vector3 ntopVector = nTopLeft - nTopRight;
						Vector3 nbotVector = nBottomLeft - nBottomRight;

						Vector3 nRandomPoint = Vector3.Lerp(Vector3.Lerp(nBottomLeft, nBottomRight, Vector2.Distance(new Vector2(nBottomLeft.x, nBottomLeft.z), new Vector2(randomPoint.x, randomPoint.z))),
							Vector3.Lerp(nTopLeft, nTopRight, Vector2.Distance(new Vector2(nTopLeft.x, nTopLeft.z), new Vector2(randomPoint.x, randomPoint.z))),
							Vector2.Distance(new Vector2(botVector.x, botVector.z), new Vector2(topVector.x, topVector.z)));

						QuadNormals.Add(nRandomPoint);
					}
				}
			}
		}

		int i = 0;
		foreach (Vector3 obg in QuadVertices)
		{
			GameObject newObj = Instantiate(o, obg, Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f)));
			//newObj.transform.localRotation = Quaternion.FromToRotation(-newObj.transform.up, QuadNormals[i]) /** newObj.transform.rotation*/; 
			i++;
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

public struct Chunk
{
	public GameObject gameObject;
	public Bounds bounds;
}
