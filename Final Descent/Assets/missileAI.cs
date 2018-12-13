using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class missileAI : MonoBehaviour {

	public Transform target;
	public float force = 10.0f;
	private ParticleSystem ps;
	// Use this for initialization
	void Start () {
		ps = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
		ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.particleCount];
		ps.GetParticles(particles);

		for( int i = 0; i < particles.Length; i++)
		{
			ParticleSystem.Particle p = particles[i];

			Vector3 dirToTarget = (target.position - p.position).normalized; // senao a força aplicada iria ser maior consoante a distancia
			Vector3 seekTarget = (dirToTarget * force) * Time.deltaTime;

			p.velocity = seekTarget;

			particles[i] = p;
		}

		ps.SetParticles(particles, particles.Length);
	}
}
