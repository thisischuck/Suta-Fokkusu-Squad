using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthEnemy : BaseStats
{
    private void Start()
    {
        GenerateVariables(100, 0);
        base_maxHealth = 100;
        //GetComponent<HBController>().StartHPBar(health);
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<HBController>() != null)
            GetComponent<HBController>().SetCurrentHealth(health);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet")
		{
			//Eel case
			if (transform.name == "Head")
			{
				Debug.Log("EEl case");
				//GameObject eel = other.transform.Find("Head").gameObject;
				CheckWhatWepon(other.gameObject);
			}
			else
			{
				CheckWhatWepon(other.gameObject);
				Debug.Log("no parent");
			}

			GameObject stats = GameObject.Find("Stats");

            float enemyCurrenhp = GetComponent<HealthEnemy>().health;
            float enemyMaxhp = GetComponent<HealthEnemy>().base_maxHealth;
            string enemyName = "";
            if (GetComponent<Enemy>())
            {
                enemyName = GetComponent<Enemy>().enemyName;
                stats.GetComponent<DynamicHud>().SetEnemyStats(enemyName, enemyMaxhp, enemyCurrenhp);
            }
            else if (GetComponentInChildren<SpawnerBehaviour>())
            {
                enemyName = GetComponentInChildren<SpawnerBehaviour>().spawnerName;
                stats.GetComponent<DynamicHud>().SetEnemyStats(enemyName, enemyMaxhp, enemyCurrenhp);
            }
        }
	}

	private void CheckWhatWepon(GameObject other)
	{
		if (other.name == "missile(Clone)" || other.name == "miniMissile")
		{
			TakeDamage(10);
		}
		if (other.name == "missileBig")
		{
			TakeDamage(20);
		}
		if(other.name == "EnergyBullet")
		{
			TakeDamage(10);
		}
		Destroy(other.gameObject);
	}
}
