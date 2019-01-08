using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PsGunManager : MonoBehaviour
{
    public ParticleSystem system, ultraSystem, ultraChargeSystem;
    public GameObject fireObject, ultraObject;
    private List<ParticleCollisionEvent> collisionEvents;

    public bool canFire, isActive, canUltraFire;
    private bool wasHoldingUltra;
    public int bulletsPerClick = 1, ultraBulletsPerClick = 1;
    public float fireRate = 0.2f, maxFireRate = 0.2f, ultrafireRate = 0.1f, ultraChargeRate = 5.0f; //segundos
    private float ultraTimeCharged = 0.0f, startFireRate;
    private float currentFireRate = 0.6f, currentUltraFireRate = 0.6f;
    public float ultraCoolDown = 0.0f;
    private float ultraTimeUp = 0.0f, ultraAvailable;
    public int weaponBehaviour = 1;
    private bool toggleOn = false;
    private Transform target;

    // Use this for initialization
    void Start()
    {
        isActive = this.GetComponent<MeshRenderer>().enabled;

        canFire = true;
        canUltraFire = true;
        wasHoldingUltra = false;
        collisionEvents = new List<ParticleCollisionEvent>();

        startFireRate = fireRate;
        ultraAvailable = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        isActive = this.GetComponent<MeshRenderer>().enabled;
        if (isActive)
        {
            switch (weaponBehaviour)
            {
                case 1:
                    Weapon1();
                    break;
                case 2:
                    Weapon2();
                    break;
                case 3:
                    Weapon3();
                    break;
                case 4:
                    Weapon4();
                    break;
                case 5:
                    RotatingLaser();
                    break;
                case 6:

                    break;
            }
        }
    }

    //Laser1
    private void Weapon1()
    {
        if (Input.GetButton("Fire1") && canFire)
        {
            if (fireObject != null)
            {
                GameObject obj = Instantiate(fireObject, this.transform.position, Quaternion.identity);
                obj.GetComponent<LaserForward>().Velocity = this.transform.forward;
            }
            else
                system.Emit(bulletsPerClick);
            StartCoroutine(FireRateIE());

            SendMessage("PlayShotOnceSound");
        }
        else if (wasHoldingUltra && canUltraFire)
        {
            if (ultraChargeSystem != null && !ultraChargeSystem.isPlaying)
            {
                ultraChargeSystem.Play();
                SendMessage("PlayChargeAudio");
            }
            if (!Input.GetButton("Fire2") || ultraTimeCharged > ultraChargeRate)
            {
                if (fireObject != null)
                {
                    float scale = ultraTimeCharged * 3.0f;
                    GameObject obj = Instantiate(fireObject, this.transform.position + (this.transform.forward * scale), Quaternion.identity);
                    obj.GetComponent<LaserForward>().Velocity = this.transform.forward;
                    obj.transform.localScale *= 1.0f + scale;

                    SendMessage("StopChargeAudio");
                    SendMessage("PlayUltraLaserAudio", ultraTimeCharged);
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
        if (Input.GetButton("Fire2"))
        {
            wasHoldingUltra = true;
            ultraTimeCharged += Time.deltaTime;
        }
    }

    //MachineGun
    private void Weapon2()
    {
        if (!toggleOn && Input.GetButton("Fire1") && canFire)
        {
            system.Emit(bulletsPerClick);
            StartCoroutine(FireRateIE());
            FireRateIncrease();
            currentFireRate = fireRate;
            SendMessage("PlayShotOnceSound");
        }
        else if (toggleOn && Input.GetButton("Fire1") && canUltraFire && Time.time >= ultraAvailable)
        {
            ultraSystem.Emit(ultraBulletsPerClick);
            StartCoroutine(UltraFireRateIE());
            ultraTimeUp += Time.deltaTime;
            SendMessage("PlayUltraOnceSound");
        }
        else if (!Input.GetButton("Fire1") && !Input.GetButton("Fire2"))
        {
            fireRate = startFireRate;
            currentFireRate = startFireRate;
            ultraTimeUp = 0.0f;
        }
        if (Input.GetButtonDown("Fire2"))
            toggleOn = true;
        if (ultraTimeUp >= 0.7f)
        {
            toggleOn = false;
            ultraAvailable = Time.time + ultraCoolDown;
        }
    }

    //Shotgun/Harpoon
    private void Weapon3()
    {
        if (Input.GetButton("Fire1") && canFire)
        {
            system.Emit(bulletsPerClick);
            this.transform.Find("ShotgunForceArea").gameObject.SetActive(true);
            StartCoroutine(FireRateIE());
            SendMessage("PlayShotOnceSound");
        }
        else if (Input.GetButton("Fire2") && canUltraFire && Time.time >= ultraAvailable)
        {
            GameObject obj = Instantiate(ultraObject, this.transform.position, Quaternion.identity);
            obj.GetComponent<Hook>().Forward = this.transform.forward;
            obj.GetComponent<Hook>().weaponPos = this.transform;
            ultraAvailable = Time.time + ultraCoolDown;
            StartCoroutine(UltraFireRateIE());
            SendMessage("PlayUltraOnceSound");
        }
        if (!Input.GetButton("Fire1"))
            this.transform.Find("ShotgunForceArea").gameObject.SetActive(false);
    }

    //Missiles
    private void Weapon4()
    {
        if (Input.GetButton("Fire1") && canFire)
        {
            GameObject obj = Instantiate(fireObject, this.transform.position, Quaternion.identity);
            obj.GetComponent<missileAI>().Velocity = this.transform.forward;
            obj.transform.rotation = this.transform.rotation;
            StartCoroutine(FireRateIE());
            SendMessage("PlayShotOnceSound");
        }
        else if (Input.GetButton("Fire2") && canUltraFire && Time.time >= ultraAvailable)
        {
            GameObject obj = Instantiate(ultraObject, this.transform.position, Quaternion.identity);
            obj.GetComponent<missileAI>().Velocity = this.transform.forward;
            obj.transform.rotation = this.transform.rotation;
            ultraAvailable = Time.time + ultraCoolDown;
            StartCoroutine(UltraFireRateIE());
            SendMessage("PlayUltraOnceSound");
        }
    }

    private void RotatingLaser()
    {
        if (Input.GetButton("Fire1") && canFire)
        {
            if (fireObject != null)
            {
                GameObject obj = Instantiate(fireObject, this.transform.position, Quaternion.identity);
                obj.GetComponent<LaserForward>().Velocity = this.transform.forward;
            }
            else
                system.Emit(bulletsPerClick);
            StartCoroutine(FireRateIE());

            SendMessage("PlayShotOnceSound");
        }
        else if (wasHoldingUltra && canUltraFire)
        {
            if (ultraChargeSystem != null && !ultraChargeSystem.isPlaying)
            {
                ultraChargeSystem.Play();

            }
            if (!Input.GetButton("Fire2") || ultraTimeCharged > ultraChargeRate)
            {
                if (fireObject != null)
                {
                    float scale = ultraTimeCharged * 3.0f;
                    GameObject obj = Instantiate(fireObject, this.transform.position + (this.transform.forward * scale), Quaternion.identity);
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
        if (Input.GetButton("Fire2"))
        {
            wasHoldingUltra = true;
            ultraTimeCharged += Time.deltaTime;
        }
    }

    private bool LookForEnemy(ParticleSystem.Particle p)
    {
        float distance = 0.0f;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject e in enemies)
        {
            float d = Vector3.Distance(p.position, e.transform.position);
            if (distance > d || distance == 0)
            {
                distance = d;
                target = e.transform;
            }
        }
        if (distance > 30f)
            return false;
        else return true;
    }

    private void Pursuit(ParticleSystem ps)
    {
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.particleCount];
        ps.GetParticles(particles);

        for (int i = 0; i < particles.Length; i++)
        {
            ParticleSystem.Particle p = particles[i];

            if (LookForEnemy(p))
            {
                Vector3 dirToTarget = (target.position - p.position).normalized; // senao a força aplicada iria ser maior consoante a distancia
                Vector3 seekTarget = (dirToTarget * ps.main.startSpeed.constant) * Time.deltaTime;

                p.velocity = seekTarget;

                particles[i] = p;
            }
            else
                continue;
        }
        ps.SetParticles(particles, particles.Length);
    }

    public void OnParticleCollision(GameObject other)
    {
        Debug.Log("COLLISION CARALHO");
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
            else if (other.transform.parent != null && other.transform.parent.name == "Eel")
            {
                GameObject eel = other.transform.parent.Find("Head").gameObject;
                eel.GetComponent<HealthEnemy>().TakeDamage(15);
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

    private void FireRateIncrease()
    {
        fireRate = Mathf.Lerp(currentFireRate, maxFireRate, Time.deltaTime * 12f);
    }

    private void UltraFireRateIncrease()
    {
        ultrafireRate = Mathf.Lerp(currentUltraFireRate, maxFireRate, Time.deltaTime * 5f);
    }
}