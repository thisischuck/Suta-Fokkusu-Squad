using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterGenerator : MonoBehaviour
{
    private MeshFilter meshFilter;
    private int length, width, height; //z, x, y
    private float spacing;
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        length = 0;
        width = 0;
        height = 0;
    }

    void Update()
    {

    }

    public void CreateMesh(int width, int length, float spacing)
    {
        this.width = width;
        this.length = length;
        this.spacing = spacing;

        Vector3[] vertices = new Vector3[width * length];
        Vector2[] uvs = new Vector2[width * length];
        List<int> indexes = new List<int>();

        for (int z = 0; z < length; z++)
        {
            for (int x = 0; x < width; x++)
            {
                vertices[x + z * width] = new Vector3(x * spacing, height * spacing, z * spacing);

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
}
