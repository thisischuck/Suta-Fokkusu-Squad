using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollision : MonoBehaviour
{
	[HideInInspector]
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
		if (other.tag == "Terrain")
			Destroy(this.gameObject);
		else
		{
			Debug.Log(other.transform.name);
			int collCount = system.GetSafeCollisionEventSize();

			//if (collCount > collisionEvents.Count)
			//    collisionEvents = new ParticleCollisionEvent[collCount];

			int eventCount = system.GetCollisionEvents(other, collisionEvents);

			for (int i = 0; i < eventCount; i++)
			{
				if (other.GetComponent<HealthEnemy>())
				{
					Debug.Log("damage taken");
					other.GetComponent<HealthEnemy>().TakeDamage(damage);

					GameObject stats = GameObject.Find("Stats");

					float enemyCurrenhp = other.GetComponent<HealthEnemy>().health;
					float enemyMaxhp = other.GetComponent<HealthEnemy>().base_maxHealth;
					string enemyName = "";
					if (other.GetComponent<Enemy>())
					{
						enemyName = other.GetComponent<Enemy>().enemyName;
						stats.GetComponent<DynamicHud>().SetEnemyStats(enemyName, enemyMaxhp, enemyCurrenhp);
					}
					else if (other.GetComponentInChildren<SpawnerBehaviour>())
					{
						enemyName = other.GetComponentInChildren<SpawnerBehaviour>().spawnerName;
						stats.GetComponent<DynamicHud>().SetEnemyStats(enemyName, enemyMaxhp, enemyCurrenhp);
					}
				}
				else if (other.transform.name == "Eel")
				{
					GameObject eel = other.transform.Find("Head").gameObject;
					eel.GetComponent<HealthEnemy>().TakeDamage(damage);

					GameObject stats = GameObject.Find("Stats");

					float enemyCurrenhp = eel.GetComponent<HealthEnemy>().health;
					float enemyMaxhp = eel.GetComponent<HealthEnemy>().base_maxHealth;
					string enemyName = "";
					if (eel.GetComponent<Enemy>())
					{
						enemyName = eel.GetComponent<Enemy>().enemyName;
						stats.GetComponent<DynamicHud>().SetEnemyStats(enemyName, enemyMaxhp, enemyCurrenhp);
					}
					else if (eel.GetComponentInChildren<SpawnerBehaviour>())
					{
						enemyName = eel.GetComponentInChildren<SpawnerBehaviour>().spawnerName;
						stats.GetComponent<DynamicHud>().SetEnemyStats(enemyName, enemyMaxhp, enemyCurrenhp);
					}
				}
			}
		}
	}
}