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
    private Material material;
    private MeshFilter meshFilter;
    public MeshFilter collisionMeshFilter;
    private ObjectPlacer objectPlacer;
    private CellularDungeonLayer dungeonLayer;
    private CellularDungeonLayer[] dungeon;
    public int length, width, height; //z, x, y
    public float spacing;
    private float randomNoise;
    private int cycle;
    [Range(0, 100)]
    public float groundChance;
    private MeshCollider meshCollider;
    private Vector3[] vertices;
    private Vector3[] vNormals;

    void Start()
    {
        randomNoise = 0.2f;
        cycle = 0;
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        objectPlacer = GetComponent<ObjectPlacer>();

        dungeonLayer = new CellularDungeonLayer(width, height, length, spacing, groundChance, this.transform);
        dungeon = new CellularDungeonLayer[height];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                if (x == 0 || z == 0 || x == width - 1 || z == length - 1)
                {
                    dungeonLayer.Cells[x, z].isAlive = true;
                }
            }
        }
    }

    void Update()
    {
        if (cycle < 20)
        {
            CheckIfLives2D();
            UpdateLife2D();
            cycle++;
            if (cycle == 20)
                End();
        }
    }

    void End()
    {
        ProjectTo3D();
        meshFilter.mesh = CreateMesh();
        meshCollider.sharedMesh = meshFilter.mesh;
        objectPlacer.Place(dungeon, vertices, vNormals);
    }

    void CheckIfLives2D()
    {
        int surrounding = 0;

        for (int x = 1; x < width - 1; x++)
        {
            for (int z = 1; z < length - 1; z++)
            {
                surrounding = CountSurroundings(x, z);
                if (dungeonLayer.Cells[x, z].isAlive)
                {
                    if (surrounding < 2)
                    {
                        dungeonLayer.Cells[x, z].isGoingToLive = false;
                    }
                    else if (surrounding >= 2 && surrounding <= 3)
                    {
                        dungeonLayer.Cells[x, z].isGoingToLive = true;
                    }
                    else if (surrounding > 3)
                    {
                        dungeonLayer.Cells[x, z].isGoingToLive = false;
                    }
                }
                else if (surrounding == 3)
                {
                    dungeonLayer.Cells[x, z].isGoingToLive = true;
                }
            }
        }
    }

    void UpdateLife2D()
    {
        for (int x = 1; x < width - 1; x++)
        {
            for (int z = 1; z < length - 1; z++)
            {
                if (dungeonLayer.Cells[x, z].isAlive != dungeonLayer.Cells[x, z].isGoingToLive)
                {
                    dungeonLayer.Cells[x, z].isAlive = dungeonLayer.Cells[x, z].isGoingToLive;
                }
            }
        }
    }

    int CountSurroundings(int xCurrent, int zCurrent)
    {
        int count = 0;

        for (int x = xCurrent - 1; x <= xCurrent + 1; x++)
        {
            for (int z = zCurrent - 1; z <= zCurrent + 1; z++)
            {
                if (dungeonLayer.Cells[x, z].isAlive && x != xCurrent && z != zCurrent)
                {
                    count++;
                }
            }
        }

        return count;
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

    private Mesh CreateMesh()
    {
        List<int> indices;
        Vector2[] uvs;
        List<Vector3> normals;
        vertices = new Vector3[width * length * height];
        uvs = new Vector2[width * length * height];
        indices = new List<int>();
        normals = new List<Vector3>();

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

                    /*
                       x = 0 -> uvx = z, uvy = y;
                       y = 0 -> uvx = x, uvy = z;
                       z = 0 -> uvx = x, uvy = y;
                     */
                    Vector2 uv = new Vector2();
                    #region OLD 
                    if (!IfinMiddle(x, y, z))
                    {
                        uv.x = x + z;
                        uv.y = y + z;
                    }
                    else
                    {
                        uv.x = x;
                        uv.y = y + z;
                    }

                    #endregion

                    uvs[x + z * width + y * width * length] = uv;
                }
            }
        }

        //Indices
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

                                //Connect directly right
                                if (dungeon[y].Cells[x + 1, z].isAlive && dungeon[y + 1].Cells[x + 1, z].isAlive && z != 0)
                                {
                                    indices.Add(x + z * width + y * width * length);
                                    indices.Add(x + z * width + (y + 1) * width * length);
                                    indices.Add(x + 1 + z * width + y * width * length);

                                    indices.Add(x + 1 + z * width + y * width * length);
                                    indices.Add(x + z * width + (y + 1) * width * length);
                                    indices.Add(x + 1 + z * width + (y + 1) * width * length);
                                }
                                //Connect diagonally to right and down
                                if (dungeon[y].Cells[x + 1, z + 1].isAlive && dungeon[y + 1].Cells[x + 1, z + 1].isAlive)
                                {
                                    indices.Add(x + z * width + y * width * length);
                                    indices.Add(x + z * width + (y + 1) * width * length);
                                    indices.Add(x + 1 + (z + 1) * width + y * width * length);

                                    indices.Add(x + 1 + (z + 1) * width + y * width * length);
                                    indices.Add(x + z * width + (y + 1) * width * length);
                                    indices.Add(x + 1 + (z + 1) * width + (y + 1) * width * length);
                                }
                                //Connect diagonally to right and up
                                if (z != 0 && dungeon[y].Cells[x + 1, z - 1].isAlive && dungeon[y + 1].Cells[x + 1, z - 1].isAlive)
                                {
                                    indices.Add(x + z * width + y * width * length);
                                    indices.Add(x + z * width + (y + 1) * width * length);
                                    indices.Add(x + 1 + (z - 1) * width + y * width * length);

                                    indices.Add(x + 1 + (z - 1) * width + y * width * length);
                                    indices.Add(x + z * width + (y + 1) * width * length);
                                    indices.Add(x + 1 + (z - 1) * width + (y + 1) * width * length);
                                }


                            }
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
                        //Connect ceilling layer
                        else
                        {
                            indices.Add(x + z * width + y * width * length);
                            indices.Add(x + 1 + z * width + y * width * length);
                            indices.Add(x + (z + 1) * width + y * width * length);

                            indices.Add(x + 1 + (z + 1) * width + y * width * length);
                            indices.Add(x + (z + 1) * width + y * width * length);
                            indices.Add(x + 1 + z * width + y * width * length);
                        }

                    }
                    else if (y < height - 1)
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
        /*bool hasDoneLast = false;
        for (int y = 0; y < height - 1; y++)
        {
            for (int x = 1; x < width; x += 3)// x left --> right
            {
                if (x >= width - 1 && !hasDoneLast)
                {
                    x = width - 1;
                    hasDoneLast = true;
                }
                for (int z = 0; z < length - 1; z++)//z down --> up
                {
                    //check left
                    if (dungeon[y].Cells[x - 1, z].isAlive &&
                        dungeon[y + 1].Cells[x, z + 1].isAlive &&
                        dungeon[y + 1].Cells[x - 1, z].isAlive &&
                        dungeon[y].Cells[x, z + 1].isAlive)
                    {
                        indices.Add(x - 1 + z * width + y * width * length);
                        indices.Add(x + (z + 1) * width + y * width * length);
                        indices.Add(x - 1 + z * width + (y + 1) * width * length);

                        indices.Add(x + (z + 1) * width + y * width * length);
                        indices.Add(x + (z + 1) * width + (y + 1) * width * length);
                        indices.Add(x - 1 + z * width + (y + 1) * width * length);
                    }

                    //check current
                    //connect to left
                    if (dungeon[y].Cells[x, z].isAlive &&
                        dungeon[y].Cells[x - 1, z + 1].isAlive &&
                        dungeon[y + 1].Cells[x - 1, z + 1].isAlive &&
                        dungeon[y + 1].Cells[x, z].isAlive)
                    {
                        indices.Add(x + z * width + y * width * length);
                        indices.Add(x - 1 + (z + 1) * width + y * width * length);
                        indices.Add(x + z * width + (y + 1) * width * length);

                        indices.Add(x - 1 + (z + 1) * width + y * width * length);
                        indices.Add(x - 1 + (z + 1) * width + (y + 1) * width * length);
                        indices.Add(x + z * width + (y + 1) * width * length);
                    }

                    //connect to right
                    if (!hasDoneLast && dungeon[y].Cells[x, z].isAlive &&
                        dungeon[y].Cells[x + 1, z + 1].isAlive &&
                        dungeon[y + 1].Cells[x + 1, z + 1].isAlive &&
                        dungeon[y + 1].Cells[x, z].isAlive)
                    {
                        indices.Add(x + z * width + y * width * length);
                        indices.Add(x + z * width + (y + 1) * width * length);
                        indices.Add(x + 1 + (z + 1) * width + y * width * length);

                        indices.Add(x + 1 + (z + 1) * width + y * width * length);
                        indices.Add(x + z * width + (y + 1) * width * length);
                        indices.Add(x + 1 + (z + 1) * width + (y + 1) * width * length);
                    }

                    //check right
                    if (dungeon[y].Cells[x + 1, z].isAlive &&
                        dungeon[y + 1].Cells[x, z + 1].isAlive &&
                        dungeon[y + 1].Cells[x + 1, z].isAlive &&
                        dungeon[y].Cells[x, z + 1].isAlive)
                    {
                        indices.Add(x + 1 + z * width + y * width * length);
                        indices.Add(x + 1 + z * width + (y + 1) * width * length);
                        indices.Add(x + (z + 1) * width + y * width * length);

                        indices.Add(x + (z + 1) * width + y * width * length);
                        indices.Add(x + 1 + z * width + (y + 1) * width * length);
                        indices.Add(x + (z + 1) * width + (y + 1) * width * length);
                    }
                }
            }
        }*/
        RandomNoise();
        HeightNoise();

        //CalculateTriFlatNormal(indices, normals);
        CreateCollisionMesh();
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = vertices;
        mesh.uv = uvs;
        //mesh.SetNormals(normals);
        //mesh.SetIndices(indices.ToArray(), MeshTopology.Points, 0);
        mesh.SetTriangles(indices, 0);
        mesh.RecalculateNormals();
        vNormals = mesh.normals;

        WaterGenerator waterGenerator = GetComponentInChildren<WaterGenerator>();
        if (waterGenerator != null)
            //waterGenerator.CreateMesh(width * Mathf.CeilToInt(spacing), length * Mathf.CeilToInt(spacing), 1); //Needs optimization
            waterGenerator.CreateMesh(width * 2, length * 2, 1);

        return mesh;
    }


    private bool IfinMiddle(int x, int y, int z)
    {
        // Se estiver no meio de um mas nao dos outros dois e true;
        //Verifica se esta no meio de todo
        // Verifica se esta no meio de um
        if (x == 0 || x == width - 1)
        {
            /* if (z != 0 || z != length - 1)
                if (y != 0 || y != height - 1)
                    return true;
            */
            return false;
        }
        if (z == 0 || z == length - 1)
        {
            /*  if (x != 0 || x != width - 1)
                if (y != 0 || y != height - 1)
                    return true;
            */
            return false;
        }
        //if (y == 0 || y == height - 1)
        //{
        /* if (z != 0 || z != length - 1)
            if (x != 0 || x != width - 1)
                return true;
                */
        //return false;
        //}

        return true;
    }

    //Normal calculation for flat shading
    private void CalculateTriFlatNormal(List<int> indices, List<Vector3> normals)
    {
        /*for (int i = 0; i < indices.Count; i += 3)
        {
            Vector3 a = vertices[indices[i + 1]] - vertices[indices[i]];
            Vector3 b = vertices[indices[i + 2]] - vertices[indices[i]];
            Vector3 normal = Vector3.Cross(a, b);

            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
        }*/
    }

    //Applies random position shifts to every vertice to give a more natural cave look
    private void RandomNoise()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] += new Vector3(
                Random.Range(-randomNoise, randomNoise) * spacing,
                Random.Range(-randomNoise, randomNoise) * spacing,
                Random.Range(-randomNoise, randomNoise) * spacing);
        }
    }

    //Applies a gradual shift of height to make the caves go up and down
    private void HeightNoise()
    {
        float a = Random.Range(0.1f, 0.2f);
        float b = Random.Range(0.02f, 0.08f);
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].y += (0.5f - Mathf.PerlinNoise((vertices[i].x / spacing) * a, (vertices[i].z / spacing) * a)) * 5.0f * spacing;
            vertices[i].y += (0.5f - Mathf.PerlinNoise((vertices[i].x / spacing) * b, (vertices[i].z / spacing) * b)) * 7.5f * spacing;
        }
    }

    private void CreateCollisionMesh()
    {
        if (collisionMeshFilter == null)
            return;
        List<int> indices = new List<int>();
        for (int y = 0; y < height - 1; y++)
        {
            for (int z = 0; z < length - 1; z++)//z up --> down
            {
                for (int x = 0; x < width - 1; x++)// x left --> right
                {
                    if (dungeon[y].Cells[x, z].isAlive && dungeon[y + 1].Cells[x, z].isAlive)
                    {
                        //Connect directly right
                        if (dungeon[y].Cells[x + 1, z].isAlive && dungeon[y + 1].Cells[x + 1, z].isAlive && z != 0)
                        {
                            //first face
                            indices.Add(x + z * width + y * width * length);
                            indices.Add(x + z * width + (y + 1) * width * length);
                            indices.Add(x + 1 + z * width + y * width * length);

                            indices.Add(x + 1 + z * width + y * width * length);
                            indices.Add(x + z * width + (y + 1) * width * length);
                            indices.Add(x + 1 + z * width + (y + 1) * width * length);

                            //second face
                            indices.Add(x + z * width + y * width * length);
                            indices.Add(x + 1 + z * width + y * width * length);
                            indices.Add(x + z * width + (y + 1) * width * length);

                            indices.Add(x + 1 + z * width + y * width * length);
                            indices.Add(x + 1 + z * width + (y + 1) * width * length);
                            indices.Add(x + z * width + (y + 1) * width * length);
                        }
                        //Connect diagonally to right and down
                        if (dungeon[y].Cells[x + 1, z + 1].isAlive && dungeon[y + 1].Cells[x + 1, z + 1].isAlive)
                        {
                            indices.Add(x + z * width + y * width * length);
                            indices.Add(x + z * width + (y + 1) * width * length);
                            indices.Add(x + 1 + (z + 1) * width + y * width * length);

                            indices.Add(x + 1 + (z + 1) * width + y * width * length);
                            indices.Add(x + z * width + (y + 1) * width * length);
                            indices.Add(x + 1 + (z + 1) * width + (y + 1) * width * length);

                            indices.Add(x + z * width + y * width * length);
                            indices.Add(x + 1 + (z + 1) * width + y * width * length);
                            indices.Add(x + z * width + (y + 1) * width * length);

                            indices.Add(x + 1 + (z + 1) * width + y * width * length);
                            indices.Add(x + 1 + (z + 1) * width + (y + 1) * width * length);
                            indices.Add(x + z * width + (y + 1) * width * length);
                        }
                        //Connect diagonally to right and up
                        if (z != 0 && dungeon[y].Cells[x + 1, z - 1].isAlive && dungeon[y + 1].Cells[x + 1, z - 1].isAlive)
                        {
                            indices.Add(x + z * width + y * width * length);
                            indices.Add(x + z * width + (y + 1) * width * length);
                            indices.Add(x + 1 + (z - 1) * width + y * width * length);

                            indices.Add(x + 1 + (z - 1) * width + y * width * length);
                            indices.Add(x + z * width + (y + 1) * width * length);
                            indices.Add(x + 1 + (z - 1) * width + (y + 1) * width * length);

                            indices.Add(x + z * width + y * width * length);
                            indices.Add(x + 1 + (z - 1) * width + y * width * length);
                            indices.Add(x + z * width + (y + 1) * width * length);

                            indices.Add(x + 1 + (z - 1) * width + y * width * length);
                            indices.Add(x + 1 + (z - 1) * width + (y + 1) * width * length);
                            indices.Add(x + z * width + (y + 1) * width * length);
                        }
                    }
                }
            }
        }
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = vertices;
        mesh.SetTriangles(indices, 0);
        mesh.RecalculateNormals();
        collisionMeshFilter.mesh = mesh;
        collisionMeshFilter.gameObject.GetComponent<MeshCollider>().sharedMesh = collisionMeshFilter.mesh;
    }
}

public class CellularDungeonLayer
{
    Transform parent;
    public Cell[,] Cells;
    private float spacing, groundChance;
    public int width, height, length;

    //To create the first layer through Cellular Automata
    public CellularDungeonLayer(int width, int height, int length, float spacing, float groundChance, Transform parent)
    {
        this.width = width;
        this.height = height;
        this.length = length;
        this.spacing = spacing;
        this.groundChance = groundChance;
        this.parent = parent;

        Create(0.0f);
    }

    //Create Layers equal to an already existing layer
    public CellularDungeonLayer(CellularDungeonLayer baseLayer, int y)
    {
        this.width = baseLayer.width;
        this.height = baseLayer.height;
        this.length = baseLayer.length;
        this.spacing = baseLayer.spacing;
        this.parent = baseLayer.parent;

        CreateFromLayer(baseLayer, y);
    }

    //Create Layer full of vertices
    public CellularDungeonLayer(CellularDungeonLayer baseLayer, int y, bool full)
    {
        this.width = baseLayer.width;
        this.height = baseLayer.height;
        this.length = baseLayer.length;
        this.spacing = baseLayer.spacing;
        this.parent = baseLayer.parent;

        CreateFullLayer(baseLayer, y);
    }

    private void Create(float yCurrent)
    {
        Cells = new Cell[width, length];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                bool isAlive = false;

                int r = Random.Range(0, 101);
                if (r < groundChance) isAlive = true;

                Cells[x, z] = new Cell(new Vector3(x * spacing, yCurrent * spacing, z * spacing), isAlive);
            }
        }
    }

    private void CreateFromLayer(CellularDungeonLayer baseLayer, int y)
    {
        Cells = new Cell[width, length];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                if (baseLayer.Cells[x, z].isAlive)
                {
                    Cells[x, z] = new Cell(new Vector3(x * spacing, y * spacing, z * spacing), true);
                }
            }
        }
    }

    private void CreateFullLayer(CellularDungeonLayer baseLayer, int y)
    {
        Cells = new Cell[width, length];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                Cells[x, z] = new Cell(new Vector3(x * spacing, y * spacing, z * spacing), true);
            }
        }
    }
}

public struct Cell
{
    public Vector3 position;
    public bool isAlive;
    public bool isGoingToLive;
    public Cell(Vector3 position, bool isAlive)
    {
        this.position = position;
        this.isAlive = isAlive;
        this.isGoingToLive = isAlive;
    }
}