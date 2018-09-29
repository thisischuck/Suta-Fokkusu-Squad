using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellularAutomata : MonoBehaviour
{
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private CellularDungeonLayer dungeonLayer;
    private CellularDungeonLayer[] dungeon;
    private int length, width, height; //z, x, y
    private float spacing;
    private int cycle;
    [Range(0, 100)]
    public float groundChance = 70.0f;

    private Vector3[] vertices;
    private int[] indices;
    private Vector2[] uvs;

    void Start()
    {
        width = 60;
        height = 15;
        length = 60;
        spacing = 5.0f;
        cycle = 0;

        dungeonLayer = new CellularDungeonLayer(width, height, length, spacing, groundChance, this.transform);
        dungeon = new CellularDungeonLayer[height];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                /*if (!dungeonLayer.Cells[x, z].isAlive)
                {
                    dungeonLayer.Cells[x, z].cube.SetActive(false);
                }*/
                if (x == 0 || z == 0 || x == width - 1 || z == length - 1)
                {
                    //dungeonLayer.Cells[x, z].cube.SetActive(true);
                    dungeonLayer.Cells[x, z].isAlive = true;
                }
            }
        }

        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            CheckIfLives2D();
            UpdateLife2D();
        }
        if (cycle < 15)
        {
            CheckIfLives2D();
            UpdateLife2D();
            cycle++;
            if (cycle == 15)
            {
                CreateDungeon3D();
                meshFilter.mesh = CreateMesh();
            }
        }
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
                    //dungeonLayer.Cells[x, z].cube.SetActive(dungeonLayer.Cells[x, z].isAlive);
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
                if (dungeonLayer.Cells[x, z].isAlive && x != xCurrent && z != zCurrent) // out of range
                {
                    count++;
                }
            }
        }

        return count;
    }

    private void CreateDungeon3D()
    {
        for (int y = 0; y < height; y++)
        {
            dungeon[y] = new CellularDungeonLayer(dungeonLayer, y);
        }
    }

    private Mesh CreateMesh()
    {
        List<int> tempIndices;

        vertices = new Vector3[width * length * height];
        tempIndices = new List<int>();

        for (int y = 0; y < height - 1; y++)
        {
            for (int z = 0; z < length - 1; z++)
            {
                for (int x = 0; x < width - 1; x++)
                {
                    vertices[x + z * width + y * width * length] = dungeon[y].Cells[x, z].position;
                }
            }
        }

        for (int y = 0; y < height - 1; y++)
        {
            for (int z = 0; z < length - 1; z++)
            {
                for (int x = 0; x < width - 1; x++)
                {
                    if (dungeon[y].Cells[x, z].isAlive)
                    {
                        if (dungeon[y].Cells[x + 1, z].isAlive)
                        {
                            tempIndices.Add(x + z * width + y * width * length);
                            tempIndices.Add(x + 1 + z * width + y * width * length);
                            tempIndices.Add(x + z * width + (y + 1) * width * length);
                        }

                        else if (dungeon[y].Cells[x + 1, z + 1].isAlive)
                        {
                            tempIndices.Add(x + z * width + y * width * length);
                            tempIndices.Add(x + 1 + (z + 1) * width + y * width * length);
                            tempIndices.Add(x + z * width + (y + 1) * width * length);
                        }

                        else if (z != 0 && dungeon[y].Cells[x + 1, z - 1].isAlive)
                        {
                            tempIndices.Add(x + z * width + y * width * length);
                            tempIndices.Add(x + 1 + (z - 1) * width + y * width * length);
                            tempIndices.Add(x + z * width + (y + 1) * width * length);
                        }
                    }
                }
            }
        }

        indices = tempIndices.ToArray();

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = indices;
        mesh.RecalculateNormals();
        return mesh;
    }

    /*
    //Not needed anymore
    bool CheckInArray(int x, int y, int z)
    {
        // Posiçoes em x=0 0,0,0 / 0,h,l / 0,0,l / 0,h,0 / 0,0,l
        // em y=0 w,0,l / w,0,0
        // em z=0 w,h,0  
        if (x < 0 || x >= width || y < 0 || y >= height || z < 0 || z >= length)
            return false;
        return true;
    }*/
}

public class CellularDungeonLayer
{
    Transform parent;
    public Cell[,] Cells;
    private float spacing, groundChance;
    private int width, height, length;

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

    public CellularDungeonLayer(CellularDungeonLayer baseLayer, int y)
    {
        this.width = baseLayer.width;
        this.height = baseLayer.height;
        this.length = baseLayer.length;
        this.spacing = baseLayer.spacing;
        this.parent = baseLayer.parent;

        CreateFromLayer(baseLayer, y);
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
                /*Cells[x, z].cube = GameObject.CreatePrimitive(PrimitiveType.Quad);
                Object.Destroy(Cells[x, z].cube.GetComponent<MeshCollider>());
                Cells[x, z].cube.transform.position = Cells[x, z].position;
                Cells[x, z].cube.transform.parent = parent;
                Cells[x, z].cube.transform.localScale *= spacing;*/
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
                    /*Cells[x, z].cube = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    Object.Destroy(Cells[x, z].cube.GetComponent<MeshCollider>());
                    Cells[x, z].cube.transform.position = Cells[x, z].position;
                    Cells[x, z].cube.transform.parent = parent;
                    Cells[x, z].cube.transform.localScale *= spacing;*/
                }
            }
        }
    }
}

public struct Cell
{
    public GameObject cube;
    public Vector3 position;
    public bool isAlive;
    public bool isGoingToLive;
    public Cell(Vector3 position, bool isAlive)
    {
        this.position = position;
        this.isAlive = isAlive;
        this.isGoingToLive = isAlive;
        this.cube = null;
    }
}