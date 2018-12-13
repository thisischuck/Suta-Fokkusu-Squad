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
    public MeshFilter walls;
    private ObjectPlacer objectPlacer;
    private CellularDungeonLayer dungeonLayer;
    private CellularDungeonLayer[] dungeon;
    public int length, width, height; //z, x, y
    public float spacing;
    private float randomNoise;
    private int cycle;
    [Range(0, 100)]
    public float groundChance;
    private Vector3[] vertices;
    private Vector3[] vNormals;

    private Pathfinder pathfinder;

    void Start()
    {
        pathfinder = new Pathfinder();
        randomNoise = 0.2f;
        cycle = 0;
        meshFilter = GetComponent<MeshFilter>();
        objectPlacer = GetComponent<ObjectPlacer>();

        Path();
    }

    void Path()
    {
        int i = 0;
        do
        {
            Debug.Log("NewDungeon");
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

        DrawVisitedCells();

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
            CheckIfLives2D();
            UpdateLife2D();
        }
    }

    void End()
    {
        dungeon = new CellularDungeonLayer[height];
        ProjectTo3D();
        meshFilter.mesh = CreateMesh();
        objectPlacer.Initialize();
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
                    dungeonLayer.Cells[x, z].hasVisited = dungeonLayer.Cells[x, z].isGoingToLive;
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
        Vector3[] uvs = new Vector3[width * length * height];
        vertices = new Vector3[width * length * height];
        indices = new List<int>();

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

        //Indices
        #region Indices
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
                                    indices.Add(x + z * width + y * width * length);
                                    indices.Add(x + (z + 1) * width + y * width * length);
                                    indices.Add(x + z * width + (y + 1) * width * length);

                                    indices.Add(x + (z + 1) * width + y * width * length);
                                    indices.Add(x + (z + 1) * width + (y + 1) * width * length);
                                    indices.Add(x + z * width + (y + 1) * width * length);
                                }

                                //Connect directly right
                                if (dungeon[y].Cells[x + 1, z].isAlive && dungeon[y + 1].Cells[x + 1, z].isAlive && z != 0)
                                {
                                    //if (dungeon[y].Cells[x + 1, z + 1].hasVisited)
                                    //{
                                    indices.Add(x + z * width + y * width * length);
                                    indices.Add(x + z * width + (y + 1) * width * length);
                                    indices.Add(x + 1 + z * width + y * width * length);

                                    indices.Add(x + 1 + z * width + y * width * length);
                                    indices.Add(x + z * width + (y + 1) * width * length);
                                    indices.Add(x + 1 + z * width + (y + 1) * width * length);
                                    //}
                                    /*else
                                    {
                                        indices.Add(x + z * width + (y + 1) * width * length);
                                        indices.Add(x + z * width + y * width * length);
                                        indices.Add(x + 1 + z * width + y * width * length);

                                        indices.Add(x + z * width + (y + 1) * width * length);
                                        indices.Add(x + 1 + z * width + y * width * length);
                                        indices.Add(x + 1 + z * width + (y + 1) * width * length);
                                    }*/
                                }
                                //Connect diagonally to right and down
                                if (dungeon[y].Cells[x + 1, z + 1].isAlive && dungeon[y + 1].Cells[x + 1, z + 1].isAlive)
                                {
                                    //if (dungeon[y].Cells[x, z + 1].hasVisited)
                                    //{
                                    indices.Add(x + z * width + y * width * length);
                                    indices.Add(x + z * width + (y + 1) * width * length);
                                    indices.Add(x + 1 + (z + 1) * width + y * width * length);

                                    indices.Add(x + 1 + (z + 1) * width + y * width * length);
                                    indices.Add(x + z * width + (y + 1) * width * length);
                                    indices.Add(x + 1 + (z + 1) * width + (y + 1) * width * length);
                                    //}
                                    /*else
                                    {
                                        indices.Add(x + z * width + (y + 1) * width * length);
                                        indices.Add(x + z * width + y * width * length);
                                        indices.Add(x + 1 + (z + 1) * width + y * width * length);

                                        indices.Add(x + z * width + (y + 1) * width * length);
                                        indices.Add(x + 1 + (z + 1) * width + y * width * length);
                                        indices.Add(x + 1 + (z + 1) * width + (y + 1) * width * length);
                                    }*/
                                }
                                //Connect diagonally to right and up
                                if (z != 0 && dungeon[y].Cells[x + 1, z - 1].isAlive && dungeon[y + 1].Cells[x + 1, z - 1].isAlive)
                                {
                                    //if (dungeon[y].Cells[x + 1, z].hasVisited)
                                    //{
                                    indices.Add(x + z * width + y * width * length);
                                    indices.Add(x + z * width + (y + 1) * width * length);
                                    indices.Add(x + 1 + (z - 1) * width + y * width * length);

                                    indices.Add(x + 1 + (z - 1) * width + y * width * length);
                                    indices.Add(x + z * width + (y + 1) * width * length);
                                    indices.Add(x + 1 + (z - 1) * width + (y + 1) * width * length);
                                    //}
                                    /*else
                                    {
                                        indices.Add(x + z * width + (y + 1) * width * length);
                                        indices.Add(x + z * width + y * width * length);
                                        indices.Add(x + 1 + (z - 1) * width + y * width * length);

                                        indices.Add(x + z * width + (y + 1) * width * length);
                                        indices.Add(x + 1 + (z - 1) * width + y * width * length);
                                        indices.Add(x + 1 + (z - 1) * width + (y + 1) * width * length);
                                    }*/
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
        #endregion

        RandomNoise();
        HeightNoise();

        CreateCollisionMesh();
        //CreateSideWallsMesh();
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = vertices;
        mesh.SetUVs(0, new List<Vector3>(uvs));
        mesh.SetTriangles(indices, 0);
        mesh.RecalculateNormals();
        vNormals = mesh.normals;

        WaterGenerator waterGenerator = GetComponentInChildren<WaterGenerator>();
        if (waterGenerator != null)
            waterGenerator.CreateMesh(0, 0, width * Mathf.CeilToInt(spacing), length * Mathf.CeilToInt(spacing), 1); //Needs optimization
                                                                                                                     //waterGenerator.CreateMesh(width * 2, length * 2, 1);

        return mesh;
    }

    /** 
    * Applies random position shifts to every vertice to give a more natural cave look
    */
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

    /**
    * Applies a gradual shift of height to make the caves go up and down
    */
    private void HeightNoise()
    {
        float a = Random.Range(0.1f, 0.2f);
        float b = Random.Range(0.02f, 0.08f);
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].y += (0.5f - Mathf.PerlinNoise((vertices[i].x / spacing) * a, (vertices[i].z / spacing) * a)) * 5.0f * spacing;
            vertices[i].y += (0.5f - Mathf.PerlinNoise((vertices[i].x / spacing) * b, (vertices[i].z / spacing) * b)) * 20.0f * spacing;
        }
    }

    /**
    * Different mesh to test collisions only
    * Needs to be done since normals aren't right
    * so this might be just a temporary fix until normals are right
    * This fixes it because in this mesh, triangles are created two sided
     */
    private void CreateCollisionMesh()
    {
        List<int> indices = new List<int>();

        //Indices
        #region CollisionIndexes
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
                                    indices.Add(x + z * width + y * width * length);
                                    indices.Add(x + (z + 1) * width + y * width * length);
                                    indices.Add(x + z * width + (y + 1) * width * length);

                                    indices.Add(x + (z + 1) * width + y * width * length);
                                    indices.Add(x + (z + 1) * width + (y + 1) * width * length);
                                    indices.Add(x + z * width + (y + 1) * width * length);

                                    indices.Add(x + (z + 1) * width + y * width * length);
                                    indices.Add(x + z * width + y * width * length);
                                    indices.Add(x + z * width + (y + 1) * width * length);

                                    indices.Add(x + (z + 1) * width + (y + 1) * width * length);
                                    indices.Add(x + (z + 1) * width + y * width * length);
                                    indices.Add(x + z * width + (y + 1) * width * length);
                                }

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
        #endregion

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = vertices;
        mesh.SetTriangles(indices, 0);
        mesh.RecalculateNormals();
        collisionMeshFilter.mesh = mesh;
        collisionMeshFilter.gameObject.GetComponent<MeshCollider>().sharedMesh = collisionMeshFilter.mesh;
    }

    /**
    * This creates two walls on each side of the cave
    * that are seperate from the cave's mesh because
    * the uvs' vertices need to be seperate
    */
    private void CreateSideWallsMesh()
    {
        List<int> indices = new List<int>();

        for (int y = 0; y < height - 1; y++)
        {
            for (int z = 0; z < length - 1; z++)//z up --> down
            {
                //Left wall
                indices.Add(0 + z * width + y * width * length);
                indices.Add(0 + z * width + (y + 1) * width * length);
                indices.Add(0 + (z + 1) * width + y * width * length);

                indices.Add(0 + (z + 1) * width + y * width * length);
                indices.Add(0 + z * width + (y + 1) * width * length);
                indices.Add(0 + (z + 1) * width + (y + 1) * width * length);

                //Right wall
                indices.Add(width - 1 + z * width + y * width * length);
                indices.Add(width - 1 + (z + 1) * width + y * width * length);
                indices.Add(width - 1 + z * width + (y + 1) * width * length);

                indices.Add(width - 1 + (z + 1) * width + y * width * length);
                indices.Add(width - 1 + (z + 1) * width + (y + 1) * width * length);
                indices.Add(width - 1 + z * width + (y + 1) * width * length);
            }
        }

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = vertices;
        mesh.SetTriangles(indices, 0);
        mesh.RecalculateNormals();
        walls.mesh = mesh;
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
    public bool hasVisited;
    public Cell(Vector3 position, bool isAlive)
    {
        this.position = position;
        this.isAlive = isAlive;
        this.isGoingToLive = isAlive;
        this.hasVisited = isAlive;
    }
}