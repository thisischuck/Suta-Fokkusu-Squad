using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pathfinder
{
    private Vector2 spawnPoint, endPoint;
    private int width, height;
    private int range;
    private CellularDungeonLayer dungeonLayer, dungeonCopy;
    public bool isfinished;
    public float maxY;
    //PathFinding to see if you can finish the game. Returns a spawnPoint and a endPoint

    public Vector2 SpawnPoint
    {
        get { return spawnPoint; }
    }

    public Vector2 EndPoint
    {
        get { return endPoint; }
    }


    public void StartPathfinder(CellularDungeonLayer layer, int width, int height, int range)
    {
        dungeonLayer = layer;
        dungeonCopy = layer;
        isfinished = false;
        this.width = width;
        this.height = height;
        this.range = range;
        Start();
    }

    private void Start()
    {
        Vector2 spawnPointTmp;
        Vector2 endPointTmp;

        for (int y = 1; y <= range; y++)
        {
            dungeonLayer = dungeonCopy;
            for (int x = 1; x < width - 1; x++)
            {
                spawnPointTmp = new Vector2(x, y);
                if (RecursiveFindEnd(spawnPointTmp, out endPointTmp))
                {
                    spawnPoint = spawnPointTmp;
                    endPoint = endPointTmp;
                    isfinished = true;
                    break;
                }
            }
        }
    }

    public enum Move
    {
        Right, Left, Up, Down
    }

    public bool FindAllVisited()
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < width; y++)
            {
                if (!dungeonLayer.Cells[x, y].hasVisited)
                {
                    return false;
                }
            }
        return true;
    }


    List<Move> NextPossibleMoves(int xPos, int yPos)
    {
        List<Move> moveList = new List<Move>();

        for (int x = -1; x <= 1; x++)
            for (int y = -1; y <= 1; y++)
            {
                if (x == y || x == -y)
                    continue;
                if (xPos + x >= 0 && xPos + x < width - 1)
                    if (yPos + y >= 0 && yPos + y < height - 1)
                        if (!dungeonLayer.Cells[xPos + x, yPos + y].hasVisited)
                        {
                            if (x == 0)
                            {
                                if (y > 0)
                                    moveList.Add(Move.Up);
                                else if (y < 0) moveList.Add(Move.Down);
                            }

                            if (y == 0)
                            {
                                if (x > 0)
                                    moveList.Add(Move.Right);
                                else if (x < 0) moveList.Add(Move.Left);
                            }
                        }
            }
        return moveList;
    }

    Vector2 ApplyMovement(Vector2 currentPoint, Move move)
    {
        Vector2 tmp = currentPoint;
        switch (move)
        {
            case Move.Right:
                tmp.x += 1;
                break;
            case Move.Left:
                tmp.x -= 1;
                break;
            case Move.Up:
                tmp.y += 1;
                break;
            case Move.Down:
                tmp.y -= 1;
                break;
        }
        return tmp;
    }

    bool RecursiveFindEnd(Vector2 spawnPoint, out Vector2 endPoint)
    {
        Vector2 current = spawnPoint;
        if (spawnPoint.y > maxY)
            maxY = spawnPoint.y;

        if (current.y >= height - range)
        {
            endPoint = current;
            return true;
        }

        int x = (int)current.x;
        int y = (int)current.y;

        endPoint = new Vector2();

        if (dungeonLayer.Cells[x, y].hasVisited)
            return false;

        dungeonLayer.Cells[x, y].hasVisited = true;

        List<Move> moves = NextPossibleMoves(x, y);

        if (moves.Count == 0)
        {
            return false;
        }

        foreach (Move tmp in moves)
        {
            Vector2 tmpVector;
            tmpVector = ApplyMovement(current, tmp);
            if (RecursiveFindEnd(tmpVector, out endPoint))
                return true;
        }

        moves.Clear();
        return false;
    }

    // Use this for initialization
    bool findEnd(Vector2 spawnPoint, out Vector2 endPoint)
    {
        endPoint = new Vector2();
        Vector2 currentPoint = spawnPoint;

        while (currentPoint.y < height)
        {
            int x = (int)currentPoint.x;
            int y = (int)currentPoint.y;

            if (dungeonLayer.Cells[x, y].hasVisited)
                return false;

            List<Move> moves = NextPossibleMoves(x, y);

            if (moves.Count == 0)
                return false;

            int rnd = UnityEngine.Random.Range(0, moves.Count);

            currentPoint = ApplyMovement(
                currentPoint,
                moves[rnd]
                );


            dungeonLayer.Cells[x, y].hasVisited = true;
        }

        endPoint = currentPoint;
        return true;
    }
}
