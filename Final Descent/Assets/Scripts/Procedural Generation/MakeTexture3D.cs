using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using UnityEngine.Rendering;

public class MakeTexture3D
{
    Texture3D texture3D;
    public MakeTexture3D(Texture2D texture2D)
    {
        int size = texture2D.width;
        texture3D = new Texture3D(size, size, size, TextureFormat.RGBA32, true);

        Color32[] colors2D = texture2D.GetPixels32();

        Color32[] color3D = new Color32[size * size * size];

        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
                for (int z = 0; z < size; z++)
                {
                    color3D[x + (z * size) + (y * size * size)] = colors2D[x + (z * size)];
                }

        //float r = 1.0f / (size - 1.0f);
        /* for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    Color c = new Color(x * r, y * r, z * r, 1.0f);
                    color3D[x + (y * size) + (z * size * size)] = c;
                }
            }
        }*/
        texture3D.SetPixels32(color3D);
        texture3D.wrapMode = TextureWrapMode.Repeat;
        texture3D.Apply();
        //AssetDatabase.CreateAsset(texture3D, "Assets/texture3d.tif");
    }

    public Texture3D GetTexture
    {
        get { return texture3D; }
    }
}