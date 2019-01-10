using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Network_EnemyParticleCollision : NetworkBehaviour {

    public ParticleSystem system;
    private List<ParticleCollisionEvent> collisionEvents;
    public float damage;
    // Use this for initialization
    void Start()
    {
        system = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }



    public void OnParticleCollision(GameObject other)
    {
        Debug.Log(other.transform.name);
        if (isServer)
        {
            if (other.transform.name == "AircraftController(Clone)")
            {
                other.transform.GetComponent<Network_PlayerHealth>().TakeDamage(damage);
            }
        }
    }
}