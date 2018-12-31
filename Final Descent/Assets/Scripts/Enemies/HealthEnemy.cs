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
			if (this.transform.parent != null && other.transform.parent.name == "Eel")
			{
				GameObject eel = other.transform.parent.Find("Head").gameObject;
				CheckWhatWepon(eel);
			}
			else
				CheckWhatWepon(other.gameObject);

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

		if (other.name == "missile" || other.name == "miniMissile")
		{
			other.GetComponent<HealthEnemy>().TakeDamage(10);
		}
		if (other.name == "missileBig")
		{
			other.GetComponent<HealthEnemy>().TakeDamage(20);
		}
		if(other.name == "EnergyBullet")
		{
			other.GetComponent<HealthEnemy>().TakeDamage(10);
		}
		Destroy(other);
	}
}
