using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustField : MonoBehaviour
{
    public Transform target;
    ParticleSystem.Particle[] particles;
    private ParticleSystem ps;

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
		ps = GetComponent<ParticleSystem>();
        starDistanceSqr = starDistance * starDistance;
        starClipDistanceSqr = starClipDistance * starClipDistance;
    }

    // Update is called once per frame
    void Update()
    {
		particles = new ParticleSystem.Particle[ps.particleCount];
		ps.GetParticles(particles);

		for (int i = 0; i < particles.Length; i++)
        {
			ParticleSystem.Particle p = particles[i];

			if ((p.position - target.position).sqrMagnitude > starDistanceSqr)
                p.position = Random.insideUnitSphere * starDistance + target.position;

            if ((p.position - target.position).sqrMagnitude <= starClipDistanceSqr)
            {
                float percent = (p.position - target.position).sqrMagnitude / starClipDistanceSqr;
                p.startSize = percent * starSize;
                p.startColor = particleColorGradient.Evaluate(Random.Range(0f, 1f));
            }

			particles[i] = p;
        }
        ps.SetParticles(particles, particles.Length);
    }
}
