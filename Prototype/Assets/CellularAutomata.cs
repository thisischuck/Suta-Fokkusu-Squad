using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellularAutomata : MonoBehaviour
{
    private CellularDungeon dungeon;
    private int length, width, height; //z, x, y
    private float spacing;
    private int cycle;
    [Range(0, 100)]
    public float groundChance = 40.0f;

    public int surroundingMin, surroundingMax;

    void Start()
    {
        width = 25;
        height = 25;
        length = 25;
        spacing = 3.0f;
        cycle = 0;

        dungeon = new CellularDungeon(width, height, length, spacing, groundChance);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < length; z++)
                {
                    dungeon.Cells[x, y, z].cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Destroy(dungeon.Cells[x, y, z].cube.GetComponent<BoxCollider>());
                    dungeon.Cells[x, y, z].cube.transform.position = dungeon.Cells[x, y, z].position;
                    dungeon.Cells[x, y, z].cube.transform.parent = this.transform;
                    dungeon.Cells[x, y, z].cube.transform.localScale *= spacing;

                    if (!dungeon.Cells[x, y, z].isAlive)
                    {
                        dungeon.Cells[x, y, z].cube.SetActive(false);
                    }
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            CheckIfLives();
            UpdateLife();
        }
        //cycle++;

        /* 
        if (Input.GetKeyDown(KeyCode.L))
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < length; z++)
                    {
                        if (x == 0 || y == 0 || z == 0 || x == width - 1 || y == height - 1 || z == length - 1)
                        {
                            if (dungeon.Cells[x, y, z].cube.activeSelf)
                                dungeon.Cells[x, y, z].cube.SetActive(false);
                            else
                                dungeon.Cells[x, y, z].cube.SetActive(true);
                        }
                    }
                }
            }
        }
		*/
    }

    void CheckIfLives()
    {
        //min = 0, max = 26
        int surrounding = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < length; z++)
                {
                    surrounding = CountSurroundings(x, y, z);
                    if (dungeon.Cells[x, y, z].isAlive)
                    {
                        if (surrounding < surroundingMin)
                        {
                            dungeon.Cells[x, y, z].isGoingToLive = false;
                        }
                        else if (surrounding >= surroundingMin && surrounding <= surroundingMax)
                        {
                            dungeon.Cells[x, y, z].isGoingToLive = true;
                        }
                        else if (surrounding > surroundingMax)
                        {
                            dungeon.Cells[x, y, z].isGoingToLive = false;
                        }
                    }
                    else if (surrounding == surroundingMin)
                    {
                        dungeon.Cells[x, y, z].isGoingToLive = true;
                    }
                }
            }
        }
    }

    void UpdateLife()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < length; z++)
                {
                    if (dungeon.Cells[x, y, z].isAlive != dungeon.Cells[x, y, z].isGoingToLive)
                    {
                        dungeon.Cells[x, y, z].isAlive = dungeon.Cells[x, y, z].isGoingToLive;
                        dungeon.Cells[x, y, z].cube.SetActive(dungeon.Cells[x, y, z].isAlive);
                    }
                }
            }
        }
    }

    int CountSurroundings(int xCurrent, int yCurrent, int zCurrent)
    {

        //Count Surroundings in corners
        int count = 0;


        for (int x = xCurrent - 1; x <= xCurrent + 1; x++)
        {
            for (int y = yCurrent - 1; y <= yCurrent + 1; y++)
            {
                for (int z = zCurrent - 1; z < zCurrent + 1; z++)
                {
                    if (!CheckInArray(x, y, z))
                    {
                        continue;
                    }
                    else if (dungeon.Cells[x, y, z].isAlive) // out of range
                    {
                        count++;
                    }
                }
            }
        }

        return count;
    }

    bool CheckInArray(int x, int y, int z)
    {

        // Posiçoes em x=0 0,0,0 / 0,h,l / 0,0,l / 0,h,0 / 0,0,l
        // em y=0 w,0,l / w,0,0
        // em z=0 w,h,0  
        if (x < 0 || x >= width)
        {
            return false;
        }
        if (y < 0 || y >= height)
        {
            return false;
        }
        if (z < 0 || z >= length)
        {
            return false;
        }
        return true;
    }
}

public class CellularDungeon
{
    public Cell[,,] Cells;
    private float spacing, groundChance;
    private int width, height, length; //x, y, z
    public CellularDungeon(int width, int height, int length, float spacing, float groundChance)
    {
        this.width = width;
        this.height = height;
        this.length = length;
        this.spacing = spacing;
        this.groundChance = groundChance;

        Create();
    }

    private void Create()
    {
        Cells = new Cell[width, height, length];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < length; z++)
                {
                    bool isAlive = false;
                    /*if (x == 0 || y == 0 || z == 0 || x == width - 1 || y == height - 1 || z == length - 1)
                    {
                        isAlive = true;
                    }
                    else
                    {
                    }*/
                    int r = Random.Range(0, 101);
                    if (r < groundChance) isAlive = true;
                    Cells[x, y, z] = new Cell(new Vector3(x * spacing, y * spacing, z * spacing), isAlive);
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