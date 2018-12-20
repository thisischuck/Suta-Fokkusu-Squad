using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ObjectPlacer_Online : NetworkBehaviour {
    public bool IsOnline = false;

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
        //player = GameObject.FindGameObjectWithTag("Player").transform;
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
        /**
        * Active and deactivate chunks based on the camera's frustum
        * Far plane is reduced for calculations of a low view distance
        * and then it's resetted back to it's original distance
        */
        foreach (Chunk chunk in objectChunks.Values)
        {
            float farPlane = Camera.main.farClipPlane;
            Camera.main.farClipPlane = chunkRenderDistance;
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
            chunk.gameObject.SetActive(GeometryUtility.TestPlanesAABB(planes, chunk.bounds));
            Camera.main.farClipPlane = farPlane;
        }
    }

    private void RegisterPrefabs()
    {
        foreach (ObjectTobePlaced o in objects)
        {
            GameObject networkObject = o.GameObject;
            networkObject.AddComponent<NetworkIdentity>();
            ClientScene.RegisterPrefab(networkObject);
        }
    }

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
    }

    public void Ceiling(CellularDungeonLayer[] dungeon, Vector3[] vertices, Vector3[] normals, int x, int y, int z, ObjectTobePlaced o)
    {
        //if (!dungeon[y].Cells[x, z].hasVisited) return;

        float r = Random.Range(0.0f, 100.0f);
        if (y == dungeon.Length - 1 && r > 100 - o.SpawnRate)
        {
            GameObject newObj = Instantiate(o.GameObject, vertices[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length], Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f))); //buscar a normal do vertice para rotação 
            NetworkServer.Spawn(newObj);

            newObj.transform.parent = objectChunks[currentChunk].gameObject.transform;
            newObj.transform.localScale = Vector3.one * Random.Range(0, maxSize);
            if (newObj.tag != "Stalactite")
                newObj.transform.rotation = Quaternion.FromToRotation(newObj.transform.up, normals[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length]) * newObj.transform.rotation;
            newObj.transform.rotation = Quaternion.AngleAxis(Random.Range(-180, 180), Vector3.up);
            positionsUsed.Add(new Vector3(x, y, z), newObj);
        }
    }

    public void Floor(CellularDungeonLayer[] dungeon, Vector3[] vertices, Vector3[] normals, int x, int y, int z, ObjectTobePlaced o)
    {
        //if (!dungeon[y].Cells[x, z].hasVisited) return;

        if (o.GameObject.layer == LayerMask.NameToLayer("AboveWater") && vertices[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length].y <= water.position.y ||
            o.GameObject.layer == LayerMask.NameToLayer("BelowWater") && vertices[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length].y > water.position.y) return;

        float r = Random.Range(0.0f, 100.0f);
        if (y == 0 && r > 100 - o.SpawnRate)
        {
            GameObject newObj = Instantiate(o.GameObject, vertices[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length], Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f)));
            NetworkServer.Spawn(newObj);

            newObj.transform.parent = objectChunks[currentChunk].gameObject.transform;
            if (newObj.tag != "Up")
                newObj.transform.rotation = Quaternion.FromToRotation(newObj.transform.up, normals[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length]) * newObj.transform.rotation;
            positionsUsed.Add(new Vector3(x, y, z), newObj);
        }
    }

    public void Wall(CellularDungeonLayer[] dungeon, Vector3[] vertices, Vector3[] normals, int x, int y, int z, ObjectTobePlaced o)
    {
        if (o.GameObject.layer == LayerMask.NameToLayer("AboveWater") && vertices[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length].y <= water.position.y) return;

        float r = Random.Range(0.0f, 100.0f);
        if ((y != 0 && y != dungeon.Length - 1) && r > 100 - o.SpawnRate)
        {
            GameObject newObj = Instantiate(o.GameObject, vertices[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length], Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f)));
            NetworkServer.Spawn(newObj);

            newObj.transform.parent = objectChunks[currentChunk].gameObject.transform;
            if (newObj.tag != "Up")
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

                        Vector3 randomPoint = new Vector3(Random.Range(BottomLeft.x, BottomRight.x), 0, Random.Range(BottomLeft.z, TopLeft.z));
                        while (!IsInsidePolygon(randomPoint, BottomLeft, BottomRight, TopRight, TopLeft))
                        {
                            randomPoint = new Vector3(Random.Range(BottomLeft.x, BottomRight.x), 0, Random.Range(BottomLeft.z, TopLeft.z));
                        }

                        Vector3 topPoint = randomPoint;
                        Vector3 botPoint = randomPoint;

                        topPoint.y = HighestPoint(new Vector3[] { BottomLeft, BottomRight, TopRight, TopLeft });
                        botPoint.y = LowestPoint(new Vector3[] { BottomLeft, BottomRight, TopRight, TopLeft });

                        Vector3 intersectionTop = Vector3.zero;
                        Vector3 intersectionBot = Vector3.zero;
                        Vector3 nRandomPoint = Vector3.zero;

                        IntersectWithMesh(topPoint, botPoint, out randomPoint.y, out nRandomPoint);

                        //Vector3 topVector = new Vector3(randomPoint.x, Mathf.Lerp(TopLeft.y, TopRight.y, CalculateWeight(TopLeft, TopRight, randomPoint, out intersectionTop, true)), randomPoint.z);
                        //Vector3 botVector = new Vector3(randomPoint.x, Mathf.Lerp(BottomLeft.y, BottomRight.y, CalculateWeight(BottomLeft, BottomRight, randomPoint, out intersectionBot, false)), randomPoint.z);

                        //Vector3 aux = Vector3.zero;
                        //if (IsThePointTotheRight(intersectionTop, intersectionBot, randomPoint))
                        //    randomPoint.y = Mathf.Lerp(topVector.y, botVector.y, CalculateWeight(intersectionTop, intersectionBot, randomPoint, out aux, true));
                        //else
                        //    randomPoint.y = Mathf.Lerp(topVector.y, botVector.y, CalculateWeight(intersectionTop, intersectionBot, randomPoint, out aux, false));

                        QuadVertices.Add(randomPoint);

                        ////NORMALS 
                        //Vector3 nBottomLeft = normals[(x + 0) + (z + 0) * width + y * width * length];
                        //Vector3 nBottomRight = normals[(x + 1) + (z + 0) * width + y * width * length];
                        //Vector3 nTopLeft = normals[(x + 0) + (z + 1) * width + y * width * length];
                        //Vector3 nTopRight = normals[(x + 1) + (z + 1) * width + y * width * length];

                        //Vector3 ntopVector = nTopLeft - nTopRight;
                        //Vector3 nbotVector = nBottomLeft - nBottomRight;

                        //Vector3 nRandomPoint = Vector3.Lerp(Vector3.Lerp(nBottomLeft, nBottomRight, Vector2.Distance(new Vector2(nBottomLeft.x, nBottomLeft.z), new Vector2(randomPoint.x, randomPoint.z))),
                        //    Vector3.Lerp(nTopLeft, nTopRight, Vector2.Distance(new Vector2(nTopLeft.x, nTopLeft.z), new Vector2(randomPoint.x, randomPoint.z))),
                        //    Vector2.Distance(new Vector2(botVector.x, botVector.z), new Vector2(topVector.x, topVector.z)));
                        ////Vector3 nRandomPoint = Vector3.up;
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

    private bool IntersectWithMesh(Vector3 top, Vector3 bot, out float targetPointY, out Vector3 normal)
    {
        targetPointY = 0;
        normal = Vector3.zero;
        int layerMask = 1 << 8;

        RaycastHit hit;
        if (Physics.Linecast(top, bot, out hit))
        {
            if (hit.collider.tag == "Dungeon")
            {
                targetPointY = hit.point.y;
                normal = hit.normal;
                return true;
            }
        }
        return false;
    }

    private float LowestPoint(Vector3[] polygon)
    {
        float lowestPoint = polygon[0].y;
        for (int i = 1; i < polygon.Length; i++)
        {
            if (lowestPoint > polygon[i].y)
                lowestPoint = polygon[i].y;
        }

        return lowestPoint;
    }

    private float HighestPoint(Vector3[] polygon)
    {
        float highestPoint = polygon[0].y;
        for (int i = 1; i < polygon.Length; i++)
        {
            if (highestPoint < polygon[i].y)
                highestPoint = polygon[i].y;
        }

        return highestPoint;
    }

    private float CalculateWeight(Vector3 point1, Vector3 point2, Vector3 targetPoint, out Vector3 intersectionPoint, bool Point2toPoint1)
    {
        intersectionPoint = Vector3.zero;

        Vector3 dir_targetToPoint1 = point1 - targetPoint;
        Vector3 dir_targetToPoint2 = point2 - targetPoint;
        Vector3 dir_point1ToPoint2 = point2 - point1;
        Vector3 dir_point1ToTarget = targetPoint - point1;
        Vector3 dir_point2ToPoint1 = point1 - point2;
        Vector3 dir_point2ToTarget = targetPoint - point2;
        Vector3 normal_targetPoint = Vector3.zero;

        //dir_targetToPoint1.y = 0;
        //dir_point1ToPoint2.y = 0;
        //dir_point1ToTarget.y = 0;
        //dir_point2ToPoint1.y = 0;
        //dir_point2ToTarget.y = 0; 
        if (Point2toPoint1)
            normal_targetPoint = Vector3.Cross(dir_targetToPoint2, dir_targetToPoint1);
        else
            normal_targetPoint = Vector3.Cross(dir_targetToPoint1, dir_targetToPoint2);

        normal_targetPoint = normal_targetPoint.normalized;

        float angle_Alpha = AngleSigned(dir_point1ToPoint2, dir_point1ToTarget, normal_targetPoint);
        float angle_Alpha2 = AngleSigned(dir_point2ToPoint1, dir_point2ToTarget, normal_targetPoint);

        if (angle_Alpha <= -90 || angle_Alpha >= 90)
            return 0.01f;
        if (angle_Alpha2 <= -90 || angle_Alpha2 >= 90)
            return 0.99f;

        float angle_newVector = 180 - (Mathf.Abs(angle_Alpha) + 90);

        if (Mathf.Sign(angle_Alpha) < 0)
            angle_newVector *= -1;

        Vector3 dir_newVector = Quaternion.AngleAxis(angle_newVector, normal_targetPoint) * dir_targetToPoint1.normalized;
        dir_newVector = dir_newVector.normalized;
        Vector3 point_auxPoint = targetPoint + (dir_newVector * 20f);
        //point1.y = 0;
        //point2.y = 0;
        //targetPoint.y = 0;

        if (!LineSegmentsIntersection(point1, point2, targetPoint, point_auxPoint, out intersectionPoint))
        {
            return 0;
        }

        float distancePoint1Point2 = Vector3.Distance(point1, point2);
        float distacePoint1IntersectionPoint = Vector3.Distance(intersectionPoint, point1);
        float returnValue = distacePoint1IntersectionPoint / distancePoint1Point2;

        return returnValue;
    }

    private bool IsThePointTotheRight(Vector3 topPoint, Vector3 botPoint, Vector3 targetPoint)
    {
        topPoint.y = 0;
        botPoint.y = 0;
        targetPoint.y = 0;
        Vector3 auxTargetPoint = targetPoint + (Vector3.left * 20);
        Vector3 empty = Vector3.zero;

        if (LineSegmentsIntersection(topPoint, botPoint, targetPoint, auxTargetPoint, out empty))
        {
            return true;
        }

        return false;
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
                    numberIntersections++;
            }
            if (i == 3)
            {
                if (LineSegmentsIntersection(point, auxPoint, polygon[i], polygon[0], out pointInterstion))
                    numberIntersections++;
            }
        }

        if (numberIntersections == 1)
            return true;

        return false;
    }

    private float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
    {
        return Mathf.Atan2(
            Vector3.Dot(n, Vector3.Cross(v1, v2)),
            Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
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

