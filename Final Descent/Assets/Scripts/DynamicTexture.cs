using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicTexture : MonoBehaviour
{
    public Color ColorShip1 = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    public Color ColorShip2 = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    public Color ColorShip3 = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    Texture2D texture;
    void Start()
    {

        texture = new Texture2D(2, 2, TextureFormat.RGBA32, false, false);
        texture.SetPixel(0, 1, ColorShip1);
        texture.SetPixel(1, 1, ColorShip2);
        texture.SetPixel(0, 0, ColorShip3);
        texture.SetPixel(1, 0, ColorShip3);
        texture.filterMode = FilterMode.Point;
        texture.Apply();
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
            r.material.SetTexture("_Texture2D_BaseColors", texture);
    }

    void Update()
    {
        texture.SetPixel(0, 1, ColorShip1);
        texture.SetPixel(1, 1, ColorShip2);
        texture.SetPixel(0, 0, ColorShip3);
        texture.SetPixel(1, 0, ColorShip3);
        texture.Apply();
    }
}