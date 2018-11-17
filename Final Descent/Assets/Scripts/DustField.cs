using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustField : MonoBehaviour
{
    public Transform target;
    ParticleSystem.Particle[] points;
    public ParticleSystem particleSystem;

    public Gradient particleColorGradient;

    public int starMax = 100;
    public float starDistance = 10;
    public float starSize = 0.2f; //0.2f
    float starDistanceSqr;
    public float starClipDistance = 1;
    float starClipDistanceSqr;

    // Use this for initialization
    void Start()
    {
        starDistanceSqr = starDistance * starDistance;
        starClipDistanceSqr = starClipDistance * starClipDistance;

        /*var no = particleSystem.noise;
        no.enabled = true;
        no.strength = 1.0f;
        no.quality = ParticleSystemNoiseQuality.High;*/
    }

    void CreateStars()
    {
        points = new ParticleSystem.Particle[starMax];

        for (int i = 0; i < starMax; i++)
        {
            points[i].position = Random.insideUnitSphere * starDistance + target.position;
            points[i].startColor = particleColorGradient.Evaluate(Random.Range(0f, 1f));
            points[i].startSize = starSize;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (points == null)
            CreateStars();

        for (int i = 0; i < starMax; i++)
        {
            if ((points[i].position - target.position).sqrMagnitude > starDistanceSqr)
                points[i].position = Random.insideUnitSphere * starDistance + target.position;

            if ((points[i].position - target.position).sqrMagnitude <= starClipDistanceSqr)
            {
                float percent = (points[i].position - target.position).sqrMagnitude / starClipDistanceSqr;
                points[i].startSize = percent * starSize;
                points[i].startColor = particleColorGradient.Evaluate(Random.Range(0f, 1f));
            }
        }
        particleSystem.SetParticles(points, points.Length);
    }
}
