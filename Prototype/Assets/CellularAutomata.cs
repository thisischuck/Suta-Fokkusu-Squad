﻿#region Notes
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
--Divided mesh into submeshes because of the index limit -- many errors

Bruno 06/10/18
--Turns out submeshes aren't the answer
    --Need gameObjects with their own meshes

Bruno 08/10/18
--Vertices need to be seperate for chunks too

Bruno 09/10/18
--Apparently unity meshes can be set to 32 bit...
...
...

*/
#endregion

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellularAutomata : MonoBehaviour
{
    public Material material;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private CellularDungeonLayer dungeonLayer;
    private CellularDungeonLayer[] dungeon;
    private int length, width, height; //z, x, y
    private float spacing;
    private int cycle;
    private float noiseWaveSmoothness;
    [Range(0, 100)]
    public float groundChance = 70.0f;
    public static int maxIndices = 65533;

    private Vector3[] vertices;

    void Start()
    {
        width = 200;
        height = 20;
        length = 80;
        spacing = 5.0f;
        cycle = 0;
        noiseWaveSmoothness = 20.0f;
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

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
        List<int> indices;
        List<Vector3> normals;

        vertices = new Vector3[width * length * height];
        indices = new List<int>();
        normals = new List<Vector3>();

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

        for (int y = 0; y < height - 1; y++)
        {
            for (int z = 0; z < length; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (x < width - 1 && z < length - 1)
                    {
                        if (dungeon[y].Cells[x, z].isAlive)
                        {
                            if (dungeon[y].Cells[x + 1, z].isAlive)
                            {
                                indices.Add(x + z * width + y * width * length);
                                indices.Add(x + 1 + z * width + y * width * length);
                                indices.Add(x + z * width + (y + 1) * width * length);

                                indices.Add(x + 1 + z * width + y * width * length);
                                indices.Add(x + 1 + z * width + (y + 1) * width * length);
                                indices.Add(x + z * width + (y + 1) * width * length);

                                /*Vector3 normal = Vector3.Cross(
                                    vertices[x + 1 + z * width + y * width * length] - vertices[x + z * width + y * width * length],
                                    vertices[x + z * width + (y + 1) * width * length] - vertices[x + z * width + y * width * length]);

                                normals.Add(normal);
                                normals.Add(normal);
                                normals.Add(normal);

                                Vector3 normal2 = Vector3.Cross(
                                    vertices[x + 1 + z * width + (y + 1) * width * length] - vertices[x + 1 + z * width + y * width * length],
                                    vertices[x + z * width + (y + 1) * width * length] - vertices[x + 1 + z * width + y * width * length]);

                                normals.Add(normal2);
                                normals.Add(normal2);
                                normals.Add(normal2);*/
                            }

                            if (dungeon[y].Cells[x + 1, z + 1].isAlive)
                            {
                                indices.Add(x + z * width + y * width * length);
                                indices.Add(x + 1 + (z + 1) * width + y * width * length);
                                indices.Add(x + z * width + (y + 1) * width * length);

                                indices.Add(x + 1 + (z + 1) * width + y * width * length);
                                indices.Add(x + 1 + (z + 1) * width + (y + 1) * width * length);
                                indices.Add(x + z * width + (y + 1) * width * length);
                            }

                            if (z != 0 && dungeon[y].Cells[x + 1, z - 1].isAlive)
                            {
                                indices.Add(x + z * width + y * width * length);
                                indices.Add(x + 1 + (z - 1) * width + y * width * length);
                                indices.Add(x + z * width + (y + 1) * width * length);

                                indices.Add(x + 1 + (z - 1) * width + y * width * length);
                                indices.Add(x + 1 + (z - 1) * width + (y + 1) * width * length);
                                indices.Add(x + z * width + (y + 1) * width * length);
                            }

                            if (x == 0 && dungeon[y].Cells[x, z + 1].isAlive)
                            {
                                indices.Add(x + z * width + y * width * length);
                                indices.Add(x + (z + 1) * width + y * width * length);
                                indices.Add(x + z * width + (y + 1) * width * length);

                                indices.Add(x + (z + 1) * width + y * width * length);
                                indices.Add(x + (z + 1) * width + (y + 1) * width * length);
                                indices.Add(x + z * width + (y + 1) * width * length);
                            }
                        }
                    }
                    else
                    {
                        if (x == width - 1 && z != length - 1)
                        {
                            indices.Add(x + z * width + y * width * length);
                            indices.Add(x + (z + 1) * width + y * width * length);
                            indices.Add(x + z * width + (y + 1) * width * length);

                            indices.Add(x + (z + 1) * width + y * width * length);
                            indices.Add(x + (z + 1) * width + (y + 1) * width * length);
                            indices.Add(x + z * width + (y + 1) * width * length);
                        }
                        else if (z == length - 1 && x != width - 1)
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
            }
        }

        RandomNoise();
        HeightNoise();

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = vertices;
        //mesh.SetNormals();
        mesh.SetTriangles(indices, 0);
        mesh.RecalculateNormals();
        return mesh;


        /*int i = 0;
        int iteration = 0;
        List<int> newIndices = new List<int>();
        List<Vector3> newVertices = new List<Vector3>();

        foreach (int index in indices)
        {
            if (i == maxIndices - 1)
            {
                i = 0;
                iteration++;
                CreateChunkMesh(newIndices, newVertices.ToArray());
                newIndices.Clear();
                newVertices.Clear();
            }
            newIndices.Add(index - iteration * maxIndices);

            //if (!newVertices.Contains(vertices[index]))
            newVertices.Add(vertices[index]);
            i++;
        }*/
    }

    //Applies random position shifts to every vertice to give a more natural cave look
    private void RandomNoise()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] += new Vector3(Random.Range(-0.4f, 0.4f) * spacing, Random.Range(-0.4f, 0.4f) * spacing, Random.Range(-0.4f, 0.4f) * spacing);
        }
    }

    //Applies a gradual shift of height to make the caves go up and down
    private void HeightNoise()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].y += (Mathf.Sin(vertices[i].x / noiseWaveSmoothness) + Mathf.Cos(vertices[i].z / noiseWaveSmoothness)) * 10.0f;
        }
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