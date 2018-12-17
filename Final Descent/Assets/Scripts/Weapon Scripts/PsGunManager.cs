using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PsGunManager : MonoBehaviour
{
    public ParticleSystem system, ultraSystem, ultraChargeSystem;
    public GameObject bulletObject;
    private List<ParticleCollisionEvent> collisionEvents;

    public bool canFire, isActive, canUltraFire;
    private bool wasHoldingUltra;
    public int bulletsPerClick = 1, ultraBulletsPerClick = 1;
    public float fireRate = 0.2f, ultrafireRate = 2f, ultraChargeRate = 5.0f; //segundos
    private float ultraTimeCharged = 0.0f;

    // Use this for initialization
    void Start()
    {
        isActive = this.GetComponent<MeshRenderer>().enabled;

        canFire = true;
        canUltraFire = true;
        wasHoldingUltra = false;
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
                if (bulletObject != null)
                {
                    GameObject obj = Instantiate(bulletObject, this.transform.position, Quaternion.identity);
                    obj.GetComponent<LaserForward>().Velocity = this.transform.forward;
                }
                else
                    system.Emit(bulletsPerClick);
                StartCoroutine(FireRateIE());
            }
            else if (wasHoldingUltra && canUltraFire)
            {
                if (ultraChargeSystem != null && !ultraChargeSystem.isPlaying)
                    ultraChargeSystem.Play();
                if (!Input.GetButton("Fire2") || ultraTimeCharged > ultraChargeRate)
                {
                    if (bulletObject != null)
                    {
                        float scale = ultraTimeCharged * 3.0f;
                        GameObject obj = Instantiate(bulletObject, this.transform.position + (this.transform.forward * scale), Quaternion.identity);
                        obj.GetComponent<LaserForward>().Velocity = this.transform.forward;
                        obj.transform.localScale *= 1.0f + scale;
                    }
                    else
                        ultraSystem.Emit(ultraBulletsPerClick);
                    wasHoldingUltra = false;
                    ultraTimeCharged = 0.0f;
                    ultraChargeSystem.Stop();
                    ultraChargeSystem.Clear();
                }
                StartCoroutine(UltraFireRateIE());
            }
        }

        if (Input.GetButton("Fire2"))
        {
            wasHoldingUltra = true;
            ultraTimeCharged += Time.deltaTime;
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
    IEnumerator ChargeUltraIE()
    {
        yield return new WaitForSeconds(ultraChargeRate);
        canUltraFire = true;
    }
}
