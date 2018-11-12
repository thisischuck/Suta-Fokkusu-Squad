using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class UnderwaterFog : MonoBehaviour
{
    public Transform waterPlane;
    public PostProcessVolume postProcessVolume;
    public PostProcessProfile postProcessUnderwater;
    public PostProcessProfile postProcessDefault;
    private bool underwater;
    private bool defaultFog = false;
    private Color defaultFogColor = Color.black;
    private float defaultFogDensity = 0;

    public Color fogColor = Color.blue;
    public float density = 0.075f;
    void Start()
    {
        defaultFog = RenderSettings.fog;
        defaultFogColor = RenderSettings.fogColor;
        defaultFogDensity = RenderSettings.fogDensity;
    }

    void Update()
    {
        if (waterPlane == null) return;

        underwater = transform.position.y < waterPlane.position.y ? true : false;
        SetFog();
    }

    void SetFog()
    {
        RenderSettings.fog = underwater ? true : defaultFog;
        RenderSettings.fogColor = underwater ? fogColor : defaultFogColor;
        RenderSettings.fogDensity = underwater ? density : defaultFogDensity;
        postProcessVolume.profile = underwater ? postProcessUnderwater : postProcessDefault;
    }
}
