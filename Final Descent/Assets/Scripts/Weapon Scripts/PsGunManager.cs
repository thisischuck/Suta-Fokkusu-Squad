using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PsGunManager : MonoBehaviour
{
    public ParticleSystem system, ultraSystem;
    private List<ParticleCollisionEvent> collisionEvents;

    public bool canFire, isActive, canUltraFire;
    public int bulletsPerClick = 1, ultraBulletsPerClick = 1;
    public float fireRate = 0.45f, ultrafireRate = 2f; //segundos

    // Use this for initialization
    void Start()
    {
        isActive = this.GetComponent<MeshRenderer>().enabled;

        canFire = true;
        canUltraFire = true;
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    // Update is called once per frame
    void Update()
    {
        isActive = this.GetComponent<MeshRenderer>().enabled;

        if (isActive)
        {
            if (Input.GetButton("Fire1") && canFire)
            {
                system.Emit(bulletsPerClick);
                StartCoroutine(FireRateIE());
            }
            else if (Input.GetButton("Fire2") && canUltraFire)
            {
                ultraSystem.Emit(ultraBulletsPerClick);
                StartCoroutine(UltraFireRateIE());
            }
        }

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
                other.GetComponent<HealthEnemy>().TakeDamage(15);
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
        }
    }

    IEnumerator FireRateIE()
    {
        canFire = false;
        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }
    IEnumerator UltraFireRateIE()
    {
        canUltraFire = false;
        yield return new WaitForSeconds(ultrafireRate);
        canUltraFire = true;
    }
}
