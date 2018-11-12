using System.Collections.Generic;
using UnityEngine;

public class Wave
{
    float initialAmplitude;
    float thisPosition;
    float lastPosition;
    bool isWaving;
    float lifeSpan;
    float lifeCounter;

    public bool IsWaving
    {
        get
        {
            return isWaving;
        }
    }

    public Wave()
    {
        initialAmplitude = 0;
        thisPosition = 0;
        lastPosition = 0;
        lifeSpan = 0;
        lifeCounter = 0;
        isWaving = false;
    }

    public void Waving(float amp, float life)
    {
        //if (!isWaving)
        //{
        initialAmplitude = amp;
        isWaving = true;
        lifeSpan += life;
        thisPosition = 0;
        lastPosition = 0;
        lifeCounter = 0;
        //}
        //else
        //{
        //    initialAmplitude += amp/50;
        //    lifeSpan += life/2;
        //    lifeCounter = 0;
        //}
    }

    public void HasItStopped()
    {
        if (isWaving == true && lifeSpan > lifeCounter)
        {
            lifeCounter += 1 * Time.deltaTime;
        }

        if (lifeSpan <= lifeCounter)
        {
            isWaving = false;
        }

    }

    public float CalculateWavePosition(float time)
    {
        if (isWaving)
        {
            lastPosition = thisPosition;
            thisPosition = initialAmplitude * Mathf.Exp(-0.5f * lifeCounter) * Mathf.Cos(2 * Mathf.PI * lifeCounter);

            return thisPosition;
        }
        return 0;
    }
}

public class WaterGenerator : MonoBehaviour
{
    private BoxCollider boxCollider;
    private MeshFilter meshFilter;
    private int length, width, height; //z, x, y
    private float spacing;
    Vector3[] vertices;
    public Wave[] verticesWave;
    List<Vector3> movingVertices;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        meshFilter = GetComponent<MeshFilter>();
        movingVertices = new List<Vector3>();
        length = 0;
        width = 0;
        height = 0;

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            CreateWave(6, 21, -5);
        }

        UpdateMesh();
    }


    public void CreateMesh(int width, int length, float spacing)
    {
        this.width = width;
        this.length = length;
        this.spacing = spacing;
        boxCollider.size = new Vector3(width, height, length);
        boxCollider.center = new Vector3(width / 2.0f, height, length / 2.0f);

        vertices = new Vector3[width * length];
        verticesWave = new Wave[width * length];
        Vector2[] uvs = new Vector2[width * length];
        List<int> indexes = new List<int>();

        for (int z = 0; z < length; z++)
        {
            for (int x = 0; x < width; x++)
            {
                vertices[x + z * width] = new Vector3(x * spacing, height, z * spacing);
                verticesWave[x + z * width] = new Wave();

                if (x < width - 1 && z < length - 1)
                {
                    //primeiro triangulo
                    indexes.Add(x + z * width);
                    indexes.Add(x + (z + 1) * width);
                    indexes.Add(x + 1 + z * width);

                    //segundo triangulo
                    indexes.Add(x + 1 + (z + 1) * width);
                    indexes.Add(x + 1 + z * width);
                    indexes.Add(x + (z + 1) * width);
                }

                uvs[x + z * width] = new Vector2(x / (float)width, z / (float)length);
            }
        }

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.SetTriangles(indexes, 0);
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

    }

    public void FindPoint(Vector3 cord, float power)
    {
        if (power < 0.08)
        {
            return;
        }
        int y = 0, x = 0;
        y = Mathf.RoundToInt(cord.z);
        x = Mathf.RoundToInt(cord.x);
        CreateWave(x, y, -power);

        if (x - 1 >= 0 && y - 1 >= 0 && !verticesWave[(x - 1) + (y - 1) * width].IsWaving) { FindPoint(vertices[(x - 1) + (y - 1) * width], (power / 2f)); } //Canto superior esquerdo;
        if (y - 1 >= 0 && !verticesWave[x + (y - 1) * width].IsWaving) { FindPoint(vertices[x + (y - 1) * width], (power / 2f)); }       //Em cima;
        if (x + 1 < width && y - 1 >= 0 && !verticesWave[(x + 1) + (y - 1) * width].IsWaving) { FindPoint(vertices[(x + 1) + (y - 1) * width], (power / 2f)); } //Canto superior direito;
        if (x - 1 >= 0 && !verticesWave[(x - 1) + y * width].IsWaving) { FindPoint(vertices[(x - 1) + y * width], (power / 2f)); }        //Esquerda;
        if (x + 1 < width && !verticesWave[(x + 1) + y * width].IsWaving) { FindPoint(vertices[(x + 1) + y * width], (power / 2f)); }       //Direita;
        if (x - 1 >= 0 && y + 1 < height && !verticesWave[(x - 1) + (y + 1) * width].IsWaving) { FindPoint(vertices[(x - 1) + (y + 1) * width], (power / 2f)); } //Canto inferior esquerdo;
        if (y + 1 < height && !verticesWave[x + (y + 1) * width].IsWaving) { FindPoint(vertices[x + (y + 1) * width], (power / 2f)); }       //Em baixo;
        if (x + 1 < width && y + 1 < height && !verticesWave[(x + 1) + (y + 1) * width].IsWaving) { FindPoint(vertices[(x + 1) + (y + 1) * width], (power / 2f)); } //Canto inferior direito;



    }


    private void CreateWave(int x, int y, float power)
    {
        verticesWave[x + y * width].Waving(power, 5.25f);
        movingVertices.Add(vertices[x + y * width]);
    }

    private void UpdateWaves()
    {
        for (int i = 0; i < movingVertices.Count; i++)
        {
            Vector3 v = movingVertices[i];
            if (verticesWave[(int)v.x + (int)v.z * width].IsWaving)
            {
                vertices[(int)v.x + (int)v.z * width].y = verticesWave[(int)v.x + (int)v.z * width].CalculateWavePosition(Time.fixedTime);
            }
            verticesWave[(int)v.x + (int)v.z * width].HasItStopped();
        }
    }

    private void RemoveOldWaves()
    {
        for (int i = movingVertices.Count; i < 0; i--)
        {
            Vector3 v = movingVertices[i];
            if (!verticesWave[(int)v.x + (int)v.z * width].IsWaving)
            {
                movingVertices.Remove(v);
            }
        }
    }

    public void UpdateMesh()
    {
        UpdateWaves();
        RemoveOldWaves();

        meshFilter.mesh.vertices = vertices;

        //meshFilter.mesh.RecalculateNormals();
    }
}
