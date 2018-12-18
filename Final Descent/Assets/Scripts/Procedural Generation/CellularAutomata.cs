#region Notes
/*
Bruno 27/09/18
--Cellular Automata in 2D - Works Perfectly

Bruno 30/09/18
--Projected dungeon to 3D
--Created mesh - Bugged
    -If width != length, error happens
    -If width and length > certain size, error happens

Bruno 01/10/18
--Fixed mesh bug where if width and length are different, error happened
--Created method for vertices random noise and gradual height noise

Bruno 04/10/18
--Divided mesh into submeshes because of the 16 bit index limit -- many errors

Bruno 06/10/18
--Turns out submeshes aren't the answer
    --Need gameObjects with their own meshes

Bruno 08/10/18
--Vertices need to be seperate for chunks too

Bruno 09/10/18
--Apparently unity meshes can be set to 32 bit.......
--Made method to calculate every triangle's normal (not sure if flat shading is going to be possible)

Bruno 10/10/18
--Improved randomness to mesh
--Mesh finally boxed in
--Implemented UVs
*/
#endregion

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellularAutomata : MonoBehaviour
{
    public int SeedInspector = 0;
    public static int Seed;

    public MeshFilter meshFilter1;
    public MeshFilter meshFilter2;

    private ObjectPlacer objectPlacer;
    private CellularDungeonLayer dungeonLayer;
    private CellularDungeonLayer[] dungeon;
    public int length, width, height; //z, x, y
    public float spacing;
    private float randomNoise;
    private int cycle;
    [Range(0, 100)]
    public float groundChance;
    private Vector3[] oldVertices;
    private Vector3[] vNormals;
    private Dictionary<int, int> newPositions;

    private Pathfinder pathfinder;

    void Start()
    {
        Seed = SeedInspector;
        if (Seed == 0)
            Seed = (int)System.DateTime.Now.Ticks;
        Random.InitState(Seed);

        pathfinder = new Pathfinder();
        randomNoise = 0.2f;
        cycle = 0;
        objectPlacer = GetComponent<ObjectPlacer>();

        Path();
    }

    void Path()
    {
        int i = 0;
        do
        {
            dungeonLayer = new CellularDungeonLayer(width, height, length, spacing, groundChance, this.transform);

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < length; z++)
                {
                    if (x == 0 || z == 0 || x == width - 1 || z == length - 1)
                    {
                        dungeonLayer.Cells[x, z].isAlive = true;
                        dungeonLayer.Cells[x, z].hasVisited = true;
                    }
                }
            }

            UpdateCycle();

            pathfinder.StartPathfinder(dungeonLayer, width, length, 10);

            i++;
        } while (i < 100 && !pathfinder.isfinished);

        pathfinder.ClearAndFill();

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        float x1 = pathfinder.SpawnPoint.x * spacing + this.transform.position.x;
        float y1 = pathfinder.SpawnPoint.y * spacing + this.transform.position.y;

        Vector3 a = new Vector3(x1, this.transform.position.y, y1);
        Instantiate(cube, a, this.transform.rotation);

        x1 = pathfinder.EndPoint.x * spacing + this.transform.position.x;
        y1 = pathfinder.EndPoint.y * spacing + this.transform.position.y;

        a = new Vector3(x1, this.transform.position.y, y1);
        Instantiate(cube, a, this.transform.rotation);

        //DrawVisitedCells();

        End();
    }

    void DrawVisitedCells()
    {
        for (int y = 1; y <= length - 1; y++)
        {
            for (int x = 1; x < width - 1; x++)
            {
                if (dungeonLayer.Cells[x, y].hasVisited && !dungeonLayer.Cells[x, y].isAlive)
                {
                    GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    obj.transform.position = new Vector3(x * 10.0f, 1.0f, y * 10.0f);
                    obj.transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
                    obj.transform.localScale *= 10.0f;
                    UnityEngine.MonoBehaviour.Destroy(obj.GetComponent<MeshCollider>());
                }
            }
        }
    }

    void UpdateCycle()
    {
        for (int cycle = 0; cycle < 20; cycle++)
        {
            dungeonLayer.CheckIfLives2D();
            dungeonLayer.UpdateLife2D();
        }
    }

    void End()
    {
        dungeon = new CellularDungeonLayer[height];
        ProjectTo3D();

        MeshData data = CreateMesh();
        meshFilter1.mesh = CreateTriangles(data, 0);
        meshFilter1.gameObject.GetComponent<MeshCollider>().sharedMesh = meshFilter1.mesh;
        meshFilter2.mesh = CreateTriangles(data, 1);
        meshFilter2.gameObject.GetComponent<MeshCollider>().sharedMesh = meshFilter2.mesh;

        vNormals = CreateTriangles(new MeshData(null, null, null), 2).normals;

        WaterGenerator waterGenerator = GetComponentInChildren<WaterGenerator>();
        if (waterGenerator != null)
            //waterGenerator.CreateMesh(0, 0, width * Mathf.CeilToInt(spacing), length * Mathf.CeilToInt(spacing), 1); //Needs optimization
            waterGenerator.CreateMesh(0, 0, width * 2, length * 2, 1);

        Vector3 spawn = new Vector3(
            pathfinder.SpawnPoint.x * spacing,
            oldVertices[Mathf.FloorToInt(pathfinder.SpawnPoint.x) + Mathf.FloorToInt(pathfinder.SpawnPoint.y) * width].y + (height * spacing / 2),
            pathfinder.SpawnPoint.y * spacing);
        GameObject.FindGameObjectWithTag("Player").transform.position = spawn;

        objectPlacer.Initialize();
        objectPlacer.Place(dungeon, oldVertices, vNormals);

        oldVertices = null;
    }

    private void ProjectTo3D()
    {
        for (int y = 0; y < height; y++)
        {
            if (y != 0 && y != height - 1)
                dungeon[y] = new CellularDungeonLayer(dungeonLayer, y);
            else
                dungeon[y] = new CellularDungeonLayer(dungeonLayer, y, true);
        }
    }

    private MeshData CreateMesh()
    {
        Vector3[] vertices = new Vector3[width * length * height];
        Vector3[] uvs = new Vector3[width * length * height];

        //Vertices
        for (int y = 0; y < height; y++)
        {
            for (int z = 0; z < length; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    vertices[x + z * width + y * width * length] = dungeon[y].Cells[x, z].position;
                }
            }
        }

        //UVs
        for (int y = 0; y < height; y++)
        {
            for (int z = 0; z < length; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector3 uv = new Vector3();

                    uv.x = x;
                    uv.y = y;
                    uv.z = z;

                    uvs[x + z * width + y * width * length] = uv;
                }
            }
        }

        RandomNoise(ref vertices);
        HeightNoise(ref vertices);

        /**
        * Create copy of these vertices individually so it's not a reference
        * These unoptimized vertices are needed for object placement
        */
        oldVertices = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            oldVertices[i] = new Vector3(vertices[i].x, vertices[i].y, vertices[i].z);
        }

        MeshData data = ClearExtraVerts(vertices, uvs);

        return data;
    }

    private Mesh CreateTriangles(MeshData data, int behaviour) //0 full mesh, 1 mirrored mesh, 2 dummy mesh
    {
        List<int> indices = new List<int>();

        for (int y = 0; y < height; y++)
        {
            for (int z = 0; z < length; z++)//z up --> down
            {
                for (int x = 0; x < width; x++)// x left --> right
                {
                    if (x < width - 1 && z < length - 1)
                    {
                        if (y < height - 1)
                        {
                            if (dungeon[y].Cells[x, z].isAlive && dungeon[y + 1].Cells[x, z].isAlive)
                            {
                                //Connect directly up
                                if (dungeon[y].Cells[x, z + 1].isAlive && dungeon[y + 1].Cells[x, z + 1].isAlive && z != 0)
                                {
                                    if (behaviour == 0 || behaviour == 2)
                                    {
                                        indices.Add(x + (z + 1) * width + y * width * length);
                                        indices.Add(x + z * width + y * width * length);
                                        indices.Add(x + z * width + (y + 1) * width * length);

                                        indices.Add(x + (z + 1) * width + (y + 1) * width * length);
                                        indices.Add(x + (z + 1) * width + y * width * length);
                                        indices.Add(x + z * width + (y + 1) * width * length);
                                    }
                                    else
                                    {
                                        indices.Add(x + z * width + y * width * length);
                                        indices.Add(x + (z + 1) * width + y * width * length);
                                        indices.Add(x + z * width + (y + 1) * width * length);

                                        indices.Add(x + (z + 1) * width + y * width * length);
                                        indices.Add(x + (z + 1) * width + (y + 1) * width * length);
                                        indices.Add(x + z * width + (y + 1) * width * length);
                                    }
                                }

                                //Connect directly right
                                if (dungeon[y].Cells[x + 1, z].isAlive && dungeon[y + 1].Cells[x + 1, z].isAlive && z != 0)
                                {
                                    if (behaviour == 0 || behaviour == 2)
                                    {
                                        indices.Add(x + z * width + (y + 1) * width * length);
                                        indices.Add(x + z * width + y * width * length);
                                        indices.Add(x + 1 + z * width + y * width * length);

                                        indices.Add(x + z * width + (y + 1) * width * length);
                                        indices.Add(x + 1 + z * width + y * width * length);
                                        indices.Add(x + 1 + z * width + (y + 1) * width * length);
                                    }
                                    else
                                    {
                                        indices.Add(x + z * width + y * width * length);
                                        indices.Add(x + z * width + (y + 1) * width * length);
                                        indices.Add(x + 1 + z * width + y * width * length);

                                        indices.Add(x + 1 + z * width + y * width * length);
                                        indices.Add(x + z * width + (y + 1) * width * length);
                                        indices.Add(x + 1 + z * width + (y + 1) * width * length);
                                    }

                                }
                                //Connect diagonally to right and down
                                if (dungeon[y].Cells[x + 1, z + 1].isAlive && dungeon[y + 1].Cells[x + 1, z + 1].isAlive)
                                {
                                    if (behaviour == 0 || behaviour == 2)
                                    {
                                        indices.Add(x + z * width + (y + 1) * width * length);
                                        indices.Add(x + z * width + y * width * length);
                                        indices.Add(x + 1 + (z + 1) * width + y * width * length);

                                        indices.Add(x + z * width + (y + 1) * width * length);
                                        indices.Add(x + 1 + (z + 1) * width + y * width * length);
                                        indices.Add(x + 1 + (z + 1) * width + (y + 1) * width * length);
                                    }
                                    else
                                    {
                                        indices.Add(x + z * width + y * width * length);
                                        indices.Add(x + z * width + (y + 1) * width * length);
                                        indices.Add(x + 1 + (z + 1) * width + y * width * length);

                                        indices.Add(x + 1 + (z + 1) * width + y * width * length);
                                        indices.Add(x + z * width + (y + 1) * width * length);
                                        indices.Add(x + 1 + (z + 1) * width + (y + 1) * width * length);
                                    }

                                }
                                //Connect diagonally to right and up
                                if (z != 0 && dungeon[y].Cells[x + 1, z - 1].isAlive && dungeon[y + 1].Cells[x + 1, z - 1].isAlive)
                                {
                                    if (behaviour == 0 || behaviour == 2)
                                    {
                                        indices.Add(x + z * width + (y + 1) * width * length);
                                        indices.Add(x + z * width + y * width * length);
                                        indices.Add(x + 1 + (z - 1) * width + y * width * length);

                                        indices.Add(x + z * width + (y + 1) * width * length);
                                        indices.Add(x + 1 + (z - 1) * width + y * width * length);
                                        indices.Add(x + 1 + (z - 1) * width + (y + 1) * width * length);
                                    }
                                    else
                                    {
                                        indices.Add(x + z * width + y * width * length);
                                        indices.Add(x + z * width + (y + 1) * width * length);
                                        indices.Add(x + 1 + (z - 1) * width + y * width * length);

                                        indices.Add(x + 1 + (z - 1) * width + y * width * length);
                                        indices.Add(x + z * width + (y + 1) * width * length);
                                        indices.Add(x + 1 + (z - 1) * width + (y + 1) * width * length);
                                    }

                                }
                            }
                            if (behaviour == 0 || behaviour == 2)
                            {
                                //Connect floor layer
                                if (y == 0)
                                {
                                    indices.Add(x + z * width);
                                    indices.Add(x + (z + 1) * width);
                                    indices.Add(x + 1 + z * width);

                                    indices.Add(x + 1 + (z + 1) * width);
                                    indices.Add(x + 1 + z * width);
                                    indices.Add(x + (z + 1) * width);
                                }
                                //End off left side wall
                                if (x == 0 && z != length - 1)
                                {
                                    indices.Add(x + z * width + y * width * length);
                                    indices.Add(x + z * width + (y + 1) * width * length);
                                    indices.Add(x + (z + 1) * width + y * width * length);

                                    indices.Add(x + (z + 1) * width + y * width * length);
                                    indices.Add(x + z * width + (y + 1) * width * length);
                                    indices.Add(x + (z + 1) * width + (y + 1) * width * length);
                                }
                                //End off front wall
                                if (z == 0 && x != width - 1)
                                {
                                    indices.Add(x + z * width + y * width * length);
                                    indices.Add(x + 1 + z * width + y * width * length);
                                    indices.Add(x + z * width + (y + 1) * width * length);

                                    indices.Add(x + 1 + z * width + y * width * length);
                                    indices.Add(x + 1 + z * width + (y + 1) * width * length);
                                    indices.Add(x + z * width + (y + 1) * width * length);
                                }
                            }
                        }
                        //Connect ceilling layer
                        else if (behaviour == 0 || behaviour == 2)
                        {
                            indices.Add(x + z * width + y * width * length);
                            indices.Add(x + 1 + z * width + y * width * length);
                            indices.Add(x + (z + 1) * width + y * width * length);

                            indices.Add(x + 1 + (z + 1) * width + y * width * length);
                            indices.Add(x + (z + 1) * width + y * width * length);
                            indices.Add(x + 1 + z * width + y * width * length);
                        }
                    }
                    else if (y < height - 1 && (behaviour == 0 || behaviour == 2))
                    {
                        //End off right side wall
                        if (x == width - 1 && z != length - 1)
                        {
                            indices.Add(x + z * width + y * width * length);
                            indices.Add(x + (z + 1) * width + y * width * length);
                            indices.Add(x + z * width + (y + 1) * width * length);

                            indices.Add(x + (z + 1) * width + y * width * length);
                            indices.Add(x + (z + 1) * width + (y + 1) * width * length);
                            indices.Add(x + z * width + (y + 1) * width * length);
                        }
                        //End off back wall
                        else if (z == length - 1 && x != width - 1)
                        {
                            indices.Add(x + z * width + y * width * length);
                            indices.Add(x + z * width + (y + 1) * width * length);
                            indices.Add(x + 1 + z * width + y * width * length);

                            indices.Add(x + 1 + z * width + y * width * length);
                            indices.Add(x + z * width + (y + 1) * width * length);
                            indices.Add(x + 1 + z * width + (y + 1) * width * length);
                        }
                    }
                }
            }
        }

        Mesh mesh = new Mesh();

        if (behaviour != 2)
        {
            OptimizeIndices(ref data, indices);

            mesh.name = "Mesh " + (behaviour == 0 ? 1 : 2);
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.vertices = data.vertices;
            mesh.SetUVs(0, new List<Vector3>(data.uvs));
            mesh.SetTriangles(data.indices, 0);
            mesh.RecalculateNormals();
        }
        else
        {
            mesh.vertices = oldVertices;
            mesh.SetTriangles(indices, 0);
            mesh.RecalculateNormals();
        }

        return mesh;
    }

    /** 
    * Applies random position shifts to every vertice to give a more natural cave look
    */
    private void RandomNoise(ref Vector3[] vertices)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] += new Vector3(
                Random.Range(-randomNoise, randomNoise) * spacing,
                Random.Range(-randomNoise, randomNoise) * spacing,
                Random.Range(-randomNoise, randomNoise) * spacing);
        }
    }

    /**
    * Applies a gradual shift of height to make the caves go up and down
    */
    private void HeightNoise(ref Vector3[] vertices)
    {
        float a = Random.Range(0.1f, 0.2f);
        float b = Random.Range(0.02f, 0.08f);
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].y += (0.5f - Mathf.PerlinNoise((vertices[i].x / spacing) * a, (vertices[i].z / spacing) * a)) * 5.0f * spacing;
            vertices[i].y += (0.5f - Mathf.PerlinNoise((vertices[i].x / spacing) * b, (vertices[i].z / spacing) * b)) * 15.0f * spacing;
        }
    }

    /**
    * Clears all the vertices of cells that aren't alive
    * Old vertices array is kept in memory for objects to be placed
    * but is then left null for the GC to take care of
    */
    private MeshData ClearExtraVerts(Vector3[] vertices, Vector3[] uvs)
    {
        /**
         * Count the number of vertices
         * that are actually needed
         */
        int vertCount = 0;
        for (int y = 0; y < height; y++)
        {
            for (int z = 0; z < length; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (dungeon[y].Cells[x, z].isAlive)
                        vertCount++;
                }
            }
        }

        /**
        * Trim all of the excess vertices
        * And save their new positions in a dictionary
        * Also takes care of the uvs
        */
        newPositions = new Dictionary<int, int>();
        Vector3[] optimizedVerts = new Vector3[vertCount];
        Vector3[] optimizedUVs = new Vector3[vertCount];

        int i = 0;

        for (int y = 0; y < height; y++)
        {
            for (int z = 0; z < length; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (dungeon[y].Cells[x, z].isAlive)
                    {
                        optimizedVerts[i] = vertices[x + z * width + y * width * length];
                        optimizedUVs[i] = uvs[x + z * width + y * width * length];
                        newPositions.Add(x + z * width + y * width * length, i);
                        i++;
                    }
                }
            }
        }
        return new MeshData(optimizedVerts, optimizedUVs, null);
    }

    /**
    * Create copy of new indices
    * Replace new copy with new positions
    */
    private void OptimizeIndices(ref MeshData data, List<int> indices)
    {
        int[] newIndices = indices.ToArray();
        for (int index = 0; index < indices.Count; index++)
        {
            newIndices[index] = newPositions[indices[index]];
        }

        data.indices = newIndices;
    }
}

public struct MeshData
{
    public Vector3[] vertices;
    public Vector3[] uvs;
    public int[] indices;
    public MeshData(Vector3[] vertices, Vector3[] uvs, int[] indices)
    {
        this.vertices = vertices;
        this.uvs = uvs;
        this.indices = indices;
    }
}