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
    public bool IsOnline = false;
    public bool IsServer = false;
    int counter = 0;

    public List<ObjectTobePlaced> objects;
    public float chunkSizeX = 60.0f, chunkSizeZ = 60.0f;
    public float chunkRenderDistance;
    private Dictionary<Vector3, GameObject> positionsUsed;
    private Dictionary<Vector2, Chunk> objectChunks;
    private CellularAutomata cellular;
    private Vector2 currentChunk;
    private Transform player;
    public GameObject obj;
    public Transform water;
    public float maxSize;
    int height;
    int width;
    int length;
    float waterHeight;

    public void Initialize()
    {
        //if (IsOnline)
        //{
        //    RegisterPrefabs();
        //}
        //else
        //{
        //    player = GameObject.FindGameObjectWithTag("Player").transform;
        //}
        if (!IsOnline)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        cellular = GetComponent<CellularAutomata>();
        //waterHeight = GameObject.Find("Water").transform.position.y;
        objectChunks = new Dictionary<Vector2, Chunk>();
        for (float z = 0; z < cellular.length * cellular.spacing; z += chunkSizeZ)
        {
            for (float x = 0; x < cellular.width * cellular.spacing; x += chunkSizeX)
            {
                GameObject chunkObject = new GameObject("Chunk: " + x + ", " + z);
                Vector3 center = new Vector3(x + chunkSizeX / 2.0f, cellular.height / 2.0f, z + chunkSizeX / 2.0f);
                chunkObject.transform.position = center;
                chunkObject.transform.parent = transform;
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
        /**
        * Active and deactivate chunks based on the camera's frustum
        * Far plane is reduced for calculations of a low view distance
        * and then it's resetted back to it's original distance
        */
        if (IsOnline && player == null)
        {
            try
            {
                player = GameObject.Find("localPlayer").transform;
            }
            finally
            { }

        }
        foreach (Chunk chunk in objectChunks.Values)
        {
            float farPlane = Camera.main.farClipPlane;
            Camera.main.farClipPlane = chunkRenderDistance;
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
            chunk.gameObject.SetActive(GeometryUtility.TestPlanesAABB(planes, chunk.bounds));
            Camera.main.farClipPlane = farPlane;
        }
    }

    //private void RegisterPrefabs()
    //{
    //    foreach (ObjectTobePlaced o in objects)
    //    {
    //        GameObject networkObject = o.GameObject;
    //        networkObject.AddComponent<UnityEngine.Networking.NetworkIdentity>();
    //        UnityEngine.Networking.ClientScene.RegisterPrefab(networkObject);
    //    }
    //}

    public void Place(CellularDungeonLayer[] dungeon, Vector3[] vertices, Vector3[] normals)
    {
        positionsUsed = new Dictionary<Vector3, GameObject>();
        height = dungeon[0].height;
        width = dungeon[0].width;
        length = dungeon[0].length;

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
        //QuadCenterVertex(dungeon, vertices, normals, obj);
        //cellular.GetComponent<CellularAutomata>().manager.GetComponent<DungeonController>().AllDone();
    }

    public void Ceiling(CellularDungeonLayer[] dungeon, Vector3[] vertices, Vector3[] normals, int x, int y, int z, ObjectTobePlaced o)
    {
        if (!dungeon[y].Cells[x, z].hasVisited)
        {
            return;
        }

        float r = Random.Range(0.0f, 100.0f);
        if (y == dungeon.Length - 1 && r > 100 - o.SpawnRate)
        {
            GameObject newObj = null;
            if (IsOnline && o.GameObject.name == "Spawner_Online" && IsServer)
            {
                newObj = Instantiate(o.GameObject, vertices[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length], Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f)));
                newObj.transform.parent = objectChunks[currentChunk].gameObject.transform;
                newObj.transform.localScale = Vector3.one * Random.Range(0, maxSize);
                if (newObj.tag != "Stalactite")
                {
                    newObj.transform.rotation = Quaternion.FromToRotation(newObj.transform.up, normals[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length]) * newObj.transform.rotation;
                }

                newObj.transform.rotation = Quaternion.AngleAxis(Random.Range(-180, 180), Vector3.up);
                positionsUsed.Add(new Vector3(x, y, z), newObj);
                cellular.GetComponent<CellularAutomata>().manager.GetComponent<DungeonController>().SpawnEnemy(newObj.transform.position, newObj.transform.rotation);
            }
            else if (!IsOnline || o.GameObject.name != "Spawner_Online")
            {
                newObj = Instantiate(o.GameObject, vertices[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length], Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f))); //buscar a normal do vertice para rotação 

                newObj.transform.parent = objectChunks[currentChunk].gameObject.transform;
                newObj.transform.localScale = Vector3.one * Random.Range(0, maxSize);
                if (newObj.tag != "Stalactite")
                {
                    newObj.transform.rotation = Quaternion.FromToRotation(newObj.transform.up, normals[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length]) * newObj.transform.rotation;
                }

                newObj.transform.rotation = Quaternion.AngleAxis(Random.Range(-180, 180), Vector3.up);
                positionsUsed.Add(new Vector3(x, y, z), newObj);
            }
        }
    }

    public void Floor(CellularDungeonLayer[] dungeon, Vector3[] vertices, Vector3[] normals, int x, int y, int z, ObjectTobePlaced o)
    {
        if (!dungeon[y].Cells[x, z].hasVisited)
        {
            return;
        }

        if (o.GameObject.layer == LayerMask.NameToLayer("AboveWater") && vertices[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length].y <= water.position.y ||
            o.GameObject.layer == LayerMask.NameToLayer("BelowWater") && vertices[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length].y > water.position.y)
        {
            return;
        }

        float r = Random.Range(0.0f, 100.0f);
        if (y == 0 && r > 100 - o.SpawnRate)
        {
            GameObject newObj = null;
            if (IsOnline && o.GameObject.name == "Spawner_Online" && IsServer)
            {
                newObj = Instantiate(o.GameObject, vertices[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length], Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f)));
                newObj.transform.parent = objectChunks[currentChunk].gameObject.transform;
                newObj.transform.localScale = Vector3.one * Random.Range(0, maxSize);
                if (newObj.tag != "Stalactite")
                {
                    newObj.transform.rotation = Quaternion.FromToRotation(newObj.transform.up, normals[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length]) * newObj.transform.rotation;
                }

                newObj.transform.rotation = Quaternion.AngleAxis(Random.Range(-180, 180), Vector3.up);
                positionsUsed.Add(new Vector3(x, y, z), newObj);
                cellular.GetComponent<CellularAutomata>().manager.GetComponent<DungeonController>().SpawnEnemy(newObj.transform.position, newObj.transform.rotation);
            }
            else if (!IsOnline || o.GameObject.name != "Spawner_Online")
            {
                newObj = Instantiate(o.GameObject, vertices[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length], Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f))); //buscar a normal do vertice para rotação 

                newObj.transform.parent = objectChunks[currentChunk].gameObject.transform;
                if (newObj.tag != "Up")
                {
                    newObj.transform.rotation = Quaternion.FromToRotation(newObj.transform.up, normals[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length]) * newObj.transform.rotation;
                }

                positionsUsed.Add(new Vector3(x, y, z), newObj);
            }
        }
    }

    public void Wall(CellularDungeonLayer[] dungeon, Vector3[] vertices, Vector3[] normals, int x, int y, int z, ObjectTobePlaced o)
    {
        if (o.GameObject.layer == LayerMask.NameToLayer("AboveWater") && vertices[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length].y <= water.position.y)
        {
            return;
        }

        float r = Random.Range(0.0f, 100.0f);
        if ((y != 0 && y != dungeon.Length - 1) && r > 100 - o.SpawnRate)
        {
            GameObject newObj = null;
            if (IsOnline && o.GameObject.name == "Spawner_Online" && IsServer)
            {
                newObj = Instantiate(o.GameObject, vertices[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length], Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f)));
                newObj.transform.parent = objectChunks[currentChunk].gameObject.transform;
                newObj.transform.localScale = Vector3.one * Random.Range(0, maxSize);
                if (newObj.tag != "Stalactite")
                {
                    newObj.transform.rotation = Quaternion.FromToRotation(newObj.transform.up, normals[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length]) * newObj.transform.rotation;
                }

                newObj.transform.rotation = Quaternion.AngleAxis(Random.Range(-180, 180), Vector3.up);
                positionsUsed.Add(new Vector3(x, y, z), newObj);
                cellular.GetComponent<CellularAutomata>().manager.GetComponent<DungeonController>().SpawnEnemy(newObj.transform.position, newObj.transform.rotation);
            }
            else if (!IsOnline || o.GameObject.name != "Spawner_Online")
            {
                newObj = Instantiate(o.GameObject, vertices[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length], Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f))); //buscar a normal do vertice para rotação 

                newObj.transform.parent = objectChunks[currentChunk].gameObject.transform;
                if (newObj.tag != "Up")
                {
                    newObj.transform.rotation = Quaternion.FromToRotation(newObj.transform.up, normals[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length]) * newObj.transform.rotation;
                }

                positionsUsed.Add(new Vector3(x, y, z), newObj);
            }
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
                    if (x == 0 || x == width - 1) //PAREDE
                    {
                        char axis = 'x';
                        Vector3 BottomLeft = vertices[(x + 0) + (z + 0) * width + y * width * length];
                        Vector3 BottomRight = vertices[(x + 1) + (z + 0) * width + y * width * length];
                        Vector3 TopLeft = vertices[(x + 0) + (z + 1) * width + y * width * length];
                        Vector3 TopRight = vertices[(x + 1) + (z + 1) * width + y * width * length];

                        Vector3 randomPoint = new Vector3(0, Random.Range(BottomLeft.y, BottomRight.y), Random.Range(BottomLeft.z, TopLeft.z));
                        while (!IsInsidePolygon(randomPoint, BottomLeft, BottomRight, TopRight, TopLeft))
                        {
                            randomPoint = new Vector3(0, Random.Range(BottomLeft.y, BottomRight.y), Random.Range(BottomLeft.z, TopLeft.z));
                        }

                        Vector3 topPoint = randomPoint;
                        Vector3 botPoint = randomPoint;
                        Vector3 nRandomPoint = Vector3.zero;
                        CalculateRandomNumberPosition(BottomLeft, BottomRight, TopLeft, TopRight, out randomPoint, out nRandomPoint, topPoint, botPoint, axis);

                        QuadVertices.Add(randomPoint);

                        QuadNormals.Add(nRandomPoint);
                    }
                    else
                    if (z == 0 || z == length - 1) //PAREDE
                    {
                        char axis = 'z';
                        Vector3 BottomLeft = vertices[(x + 0) + (z + 0) * width + y * width * length];
                        Vector3 BottomRight = vertices[(x + 1) + (z + 0) * width + y * width * length];
                        Vector3 TopLeft = vertices[(x + 0) + (z + 1) * width + y * width * length];
                        Vector3 TopRight = vertices[(x + 1) + (z + 1) * width + y * width * length];

                        Vector3 randomPoint = new Vector3(Random.Range(BottomLeft.x, BottomRight.x), Random.Range(BottomLeft.y, TopLeft.y), 0);
                        while (!IsInsidePolygon(randomPoint, BottomLeft, BottomRight, TopRight, TopLeft))
                        {
                            randomPoint = new Vector3(Random.Range(BottomLeft.x, BottomRight.x), Random.Range(BottomLeft.y, TopLeft.y), 0);
                        }

                        Vector3 topPoint = randomPoint;
                        Vector3 botPoint = randomPoint;
                        Vector3 nRandomPoint = Vector3.zero;
                        CalculateRandomNumberPosition(BottomLeft, BottomRight, TopLeft, TopRight, out randomPoint, out nRandomPoint, topPoint, botPoint, axis);

                        QuadVertices.Add(randomPoint);

                        QuadNormals.Add(nRandomPoint);
                    }
                    else
                    if (y == height - 1 && x < width - 1 && z < length - 1) //TECTO
                    {
                        char axis = 'y';
                        Vector3 BottomLeft = vertices[(x + 0) + (z + 0) * width + y * width * length];
                        Vector3 BottomRight = vertices[(x + 1) + (z + 0) * width + y * width * length];
                        Vector3 TopLeft = vertices[(x + 0) + (z + 1) * width + y * width * length];
                        Vector3 TopRight = vertices[(x + 1) + (z + 1) * width + y * width * length];

                        Vector3 randomPoint = new Vector3(Random.Range(BottomLeft.x, BottomRight.x), 0, Random.Range(BottomLeft.z, TopLeft.z));
                        while (!IsInsidePolygon(randomPoint, BottomLeft, BottomRight, TopRight, TopLeft))
                        {
                            randomPoint = new Vector3(Random.Range(BottomLeft.x, BottomRight.x), 0, Random.Range(BottomLeft.z, TopLeft.z));
                        }

                        Vector3 topPoint = randomPoint;
                        Vector3 botPoint = randomPoint;
                        Vector3 nRandomPoint = Vector3.zero;
                        CalculateRandomNumberPosition(BottomLeft, BottomRight, TopLeft, TopRight, out randomPoint, out nRandomPoint, topPoint, botPoint, axis);

                        QuadVertices.Add(randomPoint);

                        QuadNormals.Add(nRandomPoint);
                    }
                    else
                    if (y == 0 && x < width - 1 && z < length - 1) //CHAO
                    {
                        char axis = 'y';
                        Vector3 BottomLeft = vertices[(x + 0) + (z + 0) * width + y * width * length];
                        Vector3 BottomRight = vertices[(x + 1) + (z + 0) * width + y * width * length];
                        Vector3 TopLeft = vertices[(x + 0) + (z + 1) * width + y * width * length];
                        Vector3 TopRight = vertices[(x + 1) + (z + 1) * width + y * width * length];

                        Vector3 randomPoint = new Vector3(Random.Range(BottomLeft.x, BottomRight.x), 0, Random.Range(BottomLeft.z, TopLeft.z));
                        while (!IsInsidePolygon(randomPoint, BottomLeft, BottomRight, TopRight, TopLeft))
                        {
                            randomPoint = new Vector3(Random.Range(BottomLeft.x, BottomRight.x), 0, Random.Range(BottomLeft.z, TopLeft.z));
                        }

                        Vector3 topPoint = randomPoint;
                        Vector3 botPoint = randomPoint;
                        Vector3 nRandomPoint = Vector3.zero;
                        CalculateRandomNumberPosition(BottomLeft, BottomRight, TopLeft, TopRight, out randomPoint, out nRandomPoint, topPoint, botPoint, axis);

                        QuadVertices.Add(randomPoint);

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

    private void CalculateRandomNumberPosition(Vector3 BottomLeft, Vector3 BottomRight, Vector3 TopLeft, Vector3 TopRight, out Vector3 randomPoint, out Vector3 nRandomPoint, Vector3 topPoint, Vector3 botPoint, char axis)
    {
        Vector3 intersectionTop = Vector3.zero;
        Vector3 intersectionBot = Vector3.zero;

        switch (axis)
        {
            case 'x':
                topPoint.x = HighestPoint(new Vector3[] { BottomLeft, BottomRight, TopRight, TopLeft }, axis);
                botPoint.x = LowestPoint(new Vector3[] { BottomLeft, BottomRight, TopRight, TopLeft }, axis);

                intersectionTop = Vector3.zero;
                intersectionBot = Vector3.zero;
                nRandomPoint = Vector3.zero;

                IntersectWithMesh(topPoint, botPoint, out randomPoint.x, out nRandomPoint, axis);
                break;
            case 'y':
                topPoint.y = HighestPoint(new Vector3[] { BottomLeft, BottomRight, TopRight, TopLeft }, axis);
                botPoint.y = LowestPoint(new Vector3[] { BottomLeft, BottomRight, TopRight, TopLeft }, axis);

                intersectionTop = Vector3.zero;
                intersectionBot = Vector3.zero;
                nRandomPoint = Vector3.zero;

                IntersectWithMesh(topPoint, botPoint, out randomPoint.y, out nRandomPoint, axis);
                break;
            case 'z':
                topPoint.z = HighestPoint(new Vector3[] { BottomLeft, BottomRight, TopRight, TopLeft }, axis);
                botPoint.z = LowestPoint(new Vector3[] { BottomLeft, BottomRight, TopRight, TopLeft }, axis);

                intersectionTop = Vector3.zero;
                intersectionBot = Vector3.zero;
                nRandomPoint = Vector3.zero;

                IntersectWithMesh(topPoint, botPoint, out randomPoint.z, out nRandomPoint, axis);
                break;
        }

        //SE NENHUM FUNCIONAR
        randomPoint = Vector3.zero;
        nRandomPoint = Vector3.zero;
    }

    private bool IntersectWithMesh(Vector3 top, Vector3 bot, out float targetPoint, out Vector3 normal, char axis)
    {
        targetPoint = 0;
        normal = Vector3.zero;

        RaycastHit hit;

        if (Physics.Linecast(top, bot, out hit))
        {
            if (hit.collider.tag == "Dungeon")
            {
                switch (axis)
                {
                    case 'x':
                        targetPoint = hit.point.x;
                        normal = hit.normal;
                        return true;
                    case 'y':
                        targetPoint = hit.point.y;
                        normal = hit.normal;
                        return true;
                    case 'z':
                        targetPoint = hit.point.z;
                        normal = hit.normal;
                        return true;

                }
            }
        }
        return false;
    }

    private float LowestPoint(Vector3[] polygon, char axis)
    {
        float lowestPoint = 0;
        switch (axis)
        {
            case 'x':
                lowestPoint = polygon[0].x;
                for (int i = 1; i < polygon.Length; i++)
                {
                    if (lowestPoint > polygon[i].x)
                    {
                        lowestPoint = polygon[i].x;
                    }
                }
                break;
            case 'y':
                lowestPoint = polygon[0].y;
                for (int i = 1; i < polygon.Length; i++)
                {
                    if (lowestPoint > polygon[i].y)
                    {
                        lowestPoint = polygon[i].y;
                    }
                }
                break;
            case 'z':
                lowestPoint = polygon[0].z;
                for (int i = 1; i < polygon.Length; i++)
                {
                    if (lowestPoint > polygon[i].z)
                    {
                        lowestPoint = polygon[i].z;
                    }
                }
                break;
        }

        return lowestPoint;
    }

    private float HighestPoint(Vector3[] polygon, char axis)
    {
        float highestPoint = 0;
        switch (axis)
        {
            case 'x':
                highestPoint = polygon[0].x;
                for (int i = 1; i < polygon.Length; i++)
                {
                    if (highestPoint < polygon[i].x)
                    {
                        highestPoint = polygon[i].x;
                    }
                }
                break;
            case 'y':
                highestPoint = polygon[0].y;
                for (int i = 1; i < polygon.Length; i++)
                {
                    if (highestPoint < polygon[i].y)
                    {
                        highestPoint = polygon[i].y;
                    }
                }
                break;
            case 'z':
                highestPoint = polygon[0].z;
                for (int i = 1; i < polygon.Length; i++)
                {
                    if (highestPoint < polygon[i].z)
                    {
                        highestPoint = polygon[i].z;
                    }
                }
                break;
        }

        return highestPoint;
    }

    private bool IsInsidePolygon(Vector3 point, Vector3 polygonVert1, Vector3 polygonVert2, Vector3 polygonVert3, Vector3 polygonVert4)
    {
        point.y = 0;
        polygonVert1.y = 0;
        polygonVert2.y = 0;
        polygonVert3.y = 0;
        polygonVert4.y = 0;

        Vector3[] polygon = new Vector3[] { polygonVert1, polygonVert2, polygonVert3, polygonVert4 };
        int numberIntersections = 0;

        Vector3 auxPoint = point + (Vector3.right * 20);
        Vector3 pointInterstion = Vector3.zero;

        for (int i = 0; i < 4; i++)
        {
            if (i < 3)
            {
                if (LineSegmentsIntersection(point, auxPoint, polygon[i], polygon[i + 1], out pointInterstion))
                {
                    numberIntersections++;
                }
            }
            if (i == 3)
            {
                if (LineSegmentsIntersection(point, auxPoint, polygon[i], polygon[0], out pointInterstion))
                {
                    numberIntersections++;
                }
            }
        }

        if (numberIntersections == 1)
        {
            return true;
        }

        return false;
    }

    private bool LineSegmentsIntersection(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, out Vector3 intersection)
    {
        intersection = Vector2.zero;

        var d = (p2.x - p1.x) * (p4.z - p3.z) - (p2.z - p1.z) * (p4.x - p3.x);

        if (d == 0.0f)
        {
            return false;
        }

        var u = ((p3.x - p1.x) * (p4.z - p3.z) - (p3.z - p1.z) * (p4.x - p3.x)) / d;
        var v = ((p3.x - p1.x) * (p2.z - p1.z) - (p3.z - p1.z) * (p2.x - p1.x)) / d;

        if (u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f)
        {
            return false;
        }

        intersection.x = p1.x + u * (p2.x - p1.x);
        intersection.y = p1.y + u * (p2.y - p1.y);
        intersection.z = p1.z + u * (p2.z - p1.z);

        return true;
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

