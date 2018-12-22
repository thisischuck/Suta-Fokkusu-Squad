using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollision : MonoBehaviour {
    public ParticleSystem system;
    private List<ParticleCollisionEvent> collisionEvents;
    public float damage;
    // Use this for initialization
    void Start () {
        system = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnParticleCollision(GameObject other)
    {
        int collCount = system.GetSafeCollisionEventSize();

        //if (collCount > collisionEvents.Count)
        //    collisionEvents = new ParticleCollisionEvent[collCount];

        int eventCount = system.GetCollisionEvents(other, collisionEvents);

        for (int i = 0; i < eventCount; i++)
        {
            if (other.GetComponent<HealthEnemy>())
            {
                other.GetComponent<HealthEnemy>().TakeDamage(damage);
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
