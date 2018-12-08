using System.Collections.Generic;
using UnityEngine;

public class WaterGenerator : MonoBehaviour
{
    //Mesh stuff
    private BoxCollider boxCollider;
    private MeshFilter meshFilter;
    private int length, width, height; //z, x, y
    private int offsetX, offsetY;
    private float spacing;
    Vector3[] vertices;

    //Shader Stuff
    public Material mat;
    [Tooltip("Wave dampening speed")]
    public float dampingSpeed = 0.02f;
    [Tooltip("Wave propagation distance")]
    public float streachDistance = 2;
    float streach;


    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        meshFilter = GetComponent<MeshFilter>();
        length = 0;
        width = 0;
        height = 0;

    }

    // Update is called once per frame
    void Update()
    {
        streach = Mathf.Clamp(streach - dampingSpeed, 0, 20);
        mat.SetFloat("_streach", streach);

    }

    public void CreateMesh(int offsetX, int offsetY, int width, int length, float spacing)
    {
        this.width = width;
        this.length = length;
        this.spacing = spacing;
        this.offsetX = offsetX;
        this.offsetY = offsetY;

        boxCollider.size = new Vector3(width, height, length);
        boxCollider.center = new Vector3(width / 2.0f, height, length / 2.0f);

        vertices = new Vector3[width * length];
        Vector2[] uvs = new Vector2[width * length];
        List<int> indexes = new List<int>();

        for (int z = 0; z < length; z++)
        {
            for (int x = 0; x < width; x++)
            {
                vertices[x + z * width] = new Vector3(offsetX + x * spacing, height, offsetY + z * spacing);

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
        meshFilter.mesh.MarkDynamic();

    }

    public void Wave(Vector3 point)
    {
        mat.SetVector("_point_of_bend", point);
        streach = streachDistance;
    }
}
