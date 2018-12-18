using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void CheckIfLives2D()
    {
        int surrounding = 0;

        for (int x = 1; x < width - 1; x++)
        {
            for (int z = 1; z < length - 1; z++)
            {
                surrounding = CountSurroundings(x, z);
                if (Cells[x, z].isAlive)
                {
                    if (surrounding < 2)
                    {
                        Cells[x, z].isGoingToLive = false;
                    }
                    else if (surrounding >= 2 && surrounding <= 3)
                    {
                        Cells[x, z].isGoingToLive = true;
                    }
                    else if (surrounding > 3)
                    {
                        Cells[x, z].isGoingToLive = false;
                    }
                }
                else if (surrounding == 3)
                {
                    Cells[x, z].isGoingToLive = true;
                }
            }
        }
    }

    public void UpdateLife2D()
    {
        for (int x = 1; x < width - 1; x++)
        {
            for (int z = 1; z < length - 1; z++)
            {
                if (Cells[x, z].isAlive != Cells[x, z].isGoingToLive)
                {
                    Cells[x, z].isAlive = Cells[x, z].isGoingToLive;
                    Cells[x, z].hasVisited = Cells[x, z].isGoingToLive;
                }
            }
        }
    }

    public int CountSurroundings(int xCurrent, int zCurrent)
    {
        int count = 0;

        for (int x = xCurrent - 1; x <= xCurrent + 1; x++)
        {
            for (int z = zCurrent - 1; z <= zCurrent + 1; z++)
            {
                if (Cells[x, z].isAlive && x != xCurrent && z != zCurrent)
                {
                    count++;
                }
            }
        }
        return count;
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
