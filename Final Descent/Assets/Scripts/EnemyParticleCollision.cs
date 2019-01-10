using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyParticleCollision : MonoBehaviour {

    public ParticleSystem system;
    private List<ParticleCollisionEvent> collisionEvents;
    public float damage;
    // Use this for initialization
    void Start () {
        system = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }
	


    public void OnParticleCollision(GameObject other)
    {
		Debug.Log(other.transform.name);
		if (other.transform.name == "AircraftController")
		{
			other.transform.GetComponent<HealthPlayer>().TakeDamage(damage);

		}
	}
}
