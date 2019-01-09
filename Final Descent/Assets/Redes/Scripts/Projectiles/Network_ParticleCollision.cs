using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Network_ParticleCollision : NetworkBehaviour {
    public ParticleSystem system;
    private List<ParticleCollisionEvent> collisionEvents;
    public float damage;
    // Use this for initialization
    void Start()
    {
        system = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnParticleCollision(GameObject other)
    {
        int eventCount = system.GetCollisionEvents(other, collisionEvents);

        for (int i = 0; i < eventCount; i++)
        {
            if (other.GetComponent<HealthEnemy>())
            {
                if (isServer)
                {
                    other.GetComponent<HealthEnemy>().TakeDamage(damage);
                    other.GetComponent<HealthEnemy>().FlashOnHit();
                }
                if (isLocalPlayer)
                {
                    GameObject stats = GameObject.Find("Stats");

                    float enemyCurrenhp = other.GetComponent<HealthEnemy>().health;
                    float enemyMaxhp = other.GetComponent<HealthEnemy>().base_maxHealth;
                    string enemyName = "";
                    if (other.GetComponent<Enemy>())
                    {
                        enemyName = other.GetComponent<HealthEnemy>().name;
                        stats.GetComponent<DynamicHud>().SetEnemyStats(enemyName, enemyMaxhp, enemyCurrenhp);
                    }
                    else if (other.GetComponentInChildren<SpawnerBehaviour>())
                    {
                        enemyName = other.GetComponentInChildren<SpawnerBehaviour>().spawnerName;
                        stats.GetComponent<DynamicHud>().SetEnemyStats(enemyName, enemyMaxhp, enemyCurrenhp);
                    }
                }
            }
        }
    }
}
