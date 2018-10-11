using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicTexture : MonoBehaviour {
	Material mat;

	public Color ColorWindow = new Color(0.0f, 0.0f, 0.0f, 0.0f);
	public Color ColorShip = new Color(0.0f, 0.0f, 0.0f, 0.0f);
	Texture2D texture;
	void Start () 
	{
		mat = GetComponent<Renderer>().material;
		texture = new Texture2D(2, 2, TextureFormat.RGBA32, false, false);	
		texture.SetPixel(0, 0, ColorWindow);
		texture.SetPixel(0, 1, ColorShip);
		texture.filterMode = FilterMode.Point;
		texture.Apply();
		mat.SetTexture("_MainTex", texture);
	}
	
	// Update is called once per frame
	void Update () 
	{
		texture.SetPixel(0, 0, ColorWindow);
		texture.SetPixel(0, 1, ColorShip);
		texture.Apply();
	}
}
