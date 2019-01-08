using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Network_PsGunManager : NetworkBehaviour
{
    public ParticleSystem system, ultraSystem, ultraChargeSystem;
    public GameObject fireObject, ultraObject, leftClickObject, rightClickObject;
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
    [SyncVar]
    public GameObject player;

    public void CallPlayerForHelp()
    {
        player.GetComponent<Network_PlayerMovement>().CmdHelpingWeapon(transform.gameObject);
    }

    public void GearUpWeapon(GameObject l, GameObject r)
    {
        system = l.GetComponent<ParticleSystem>();
        ultraSystem = r.GetComponent<ParticleSystem>();
    }

    // Use this for initialization
    void Start()
    {
        isActive = GetComponent<MeshRenderer>().enabled;

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
        if (hasAuthority)
        {
            if (leftClickObject != null && rightClickObject != null && system == null && ultraSystem == null)
            {
                CallPlayerForHelp();
            }
        }
        isActive = GetComponent<MeshRenderer>().enabled;
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
                if (hasAuthority)
                    CmdShootWeapon1(0);
            }
            else
            {
                system.Emit(bulletsPerClick);
            }

            StartCoroutine(FireRateIE());

            SendMessage("PlayLaserAudio");
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
                    if (hasAuthority)
                        CmdShootWeapon1(1);

                    SendMessage("StopChargeAudio");
                    SendMessage("PlayUltraLaserAudio", ultraTimeCharged);
                }
                else
                {
                    ultraSystem.Emit(ultraBulletsPerClick);
                }

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

    [Command]
    private void CmdShootWeapon1(int option)
    {
        if (option == 0)
        {
            GameObject obj = Instantiate(fireObject, transform.position, Quaternion.identity);
            obj.GetComponent<Network_LaserForward>().Velocity = transform.forward;
            NetworkServer.Spawn(obj);
        }
        else if (option == 1)
        {
            float scale = ultraTimeCharged * 3.0f;
            GameObject obj = Instantiate(fireObject, transform.position + (transform.forward * scale), Quaternion.identity);
            obj.GetComponent<Network_LaserForward>().Velocity = transform.forward;
            obj.GetComponent<Network_LaserForward>().scale *= 1.0f + scale;
            NetworkServer.Spawn(obj);
        }
    }

    //MachineGun
    private void Weapon2()
    {
        if (Input.GetButton("Fire1"))
        {
            if (!toggleOn && canFire)
            {
                system.Emit(bulletsPerClick);
                CmdShootWeapon2(0);
                StartCoroutine(FireRateIE());
                FireRateIncrease();
                currentFireRate = fireRate;
                SendMessage("PlayMachineGunAudio");
            }
            else if (toggleOn && canUltraFire && Time.time >= ultraAvailable)
            {
                ultraSystem.Emit(ultraBulletsPerClick);
                CmdShootWeapon2(1);
                StartCoroutine(UltraFireRateIE());
                ultraTimeUp += Time.deltaTime;
            }
        }
        else if (!Input.GetButton("Fire1") && !Input.GetButton("Fire2"))
        {
            fireRate = startFireRate;
            currentFireRate = startFireRate;
            ultraTimeUp = 0.0f;
        }
        if (Input.GetButtonDown("Fire2"))
        {
            toggleOn = true;
        }

        if (ultraTimeUp >= 0.7f)
        {
            toggleOn = false;
            ultraAvailable = Time.time + ultraCoolDown;
        }
    }

    [Command]
    private void CmdShootWeapon2(int option)
    {
        RpcShootWeapon2(option, transform.gameObject);
    }

    [ClientRpc]
    private void RpcShootWeapon2(int option, GameObject thisObj)
    {
        if (option == 0)
        {
            thisObj.GetComponent<Network_PsGunManager>().system.Emit(bulletsPerClick);
        }
        else if (option == 1)
        {
            thisObj.GetComponent<Network_PsGunManager>().ultraSystem.Emit(ultraBulletsPerClick);
        }
    }

    //Shotgun/Harpoon
    private void Weapon3()
    {
        if (Input.GetButton("Fire1") && !canFire)
        {
            transform.Find("ShotgunForceArea").GetComponent<Shotgun>().ClearList();
        }
        else if (Input.GetButton("Fire1") && canFire)
        {
            system.Emit(bulletsPerClick);
            transform.Find("ShotgunForceArea").gameObject.SetActive(true);
            StartCoroutine(FireRateIE());
        }
        else if (Input.GetButton("Fire2") && canUltraFire && Time.time >= ultraAvailable)
        {
            CmdShootWeapon3();
            ultraAvailable = Time.time + ultraCoolDown;
            StartCoroutine(UltraFireRateIE());
        }
        if (!Input.GetButton("Fire1"))
        {
            transform.Find("ShotgunForceArea").gameObject.SetActive(false);
        }
    }

    [Command]
    private void CmdShootWeapon3()
    {
        GameObject obj = Instantiate(ultraObject, transform.position, Quaternion.identity);
        obj.GetComponent<Network_Hook>().Forward = transform.forward;
        obj.GetComponent<Network_Hook>().weaponPos = transform;
        NetworkServer.Spawn(obj);
    }

    //Missiles
    private void Weapon4()
    {
        if (Input.GetButton("Fire1") && canFire)
        {
            CmdShootWeapon4(0);
            StartCoroutine(FireRateIE());
        }
        else if (Input.GetButton("Fire2") && canUltraFire && Time.time >= ultraAvailable)
        {
            CmdShootWeapon4(1);
            ultraAvailable = Time.time + ultraCoolDown;
            StartCoroutine(UltraFireRateIE());
        }
    }

    [Command]
    private void CmdShootWeapon4(int option)
    {
        if (option == 0)
        {
            GameObject obj = Instantiate(fireObject, transform.position, Quaternion.identity);
            obj.GetComponent<Network_MissileAI>().Velocity = transform.forward;
            obj.transform.rotation = transform.rotation;
            NetworkServer.Spawn(obj);
        }
        else if (option == 1)
        {
            GameObject obj = Instantiate(ultraObject, transform.position, Quaternion.identity);
            obj.GetComponent<Network_MissileAI>().Velocity = transform.forward;
            obj.transform.rotation = transform.rotation;
            NetworkServer.Spawn(obj);
        }
    }

    private void RotatingLaser()
    {
        if (Input.GetButton("Fire1") && canFire)
        {
            if (fireObject != null)
            {
                CmdShootRotatingLaser(0);
            }
            else
            {
                system.Emit(bulletsPerClick);
            }

            StartCoroutine(FireRateIE());
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
                    CmdShootRotatingLaser(1);
                }
                else
                {
                    ultraSystem.Emit(ultraBulletsPerClick);
                }

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

    [Command]
    private void CmdShootRotatingLaser(int option)
    {
        if (option == 0)
        {
            GameObject obj = Instantiate(fireObject, transform.position, Quaternion.identity);
            obj.GetComponent<Network_LaserForward>().Velocity = transform.forward;
            NetworkServer.Spawn(obj);
        }
        else if (option == 1)
        {
            float scale = ultraTimeCharged * 3.0f;
            GameObject obj = Instantiate(fireObject, transform.position + (transform.forward * scale), Quaternion.identity);
            obj.GetComponent<Network_LaserForward>().Velocity = transform.forward;
            obj.transform.localScale *= 1.0f + scale;
            NetworkServer.Spawn(obj);
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
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void Pursuit(ParticleSystem ps)
    {
        if (!isServer)
        {
            return;
        }

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
            {
                continue;
            }
        }
        ps.SetParticles(particles, particles.Length);
    }

    public void OnParticleCollision(GameObject other)
    {
        if (!isServer)
        {
            return;
        }

        int collCount = system.GetSafeCollisionEventSize();

        //if (collCount > collisionEvents.Count)
        //    collisionEvents = new ParticleCollisionEvent[collCount];

        int eventCount = system.GetCollisionEvents(other, collisionEvents);

        for (int i = 0; i < eventCount; i++)
        {
            if (other.GetComponent<Network_EnemyHealth>())
            {
                other.GetComponent<Network_EnemyHealth>().TakeDamage(15);
                GameObject stats = GameObject.Find("Stats");

                float enemyCurrenhp = other.GetComponent<Network_EnemyHealth>().currentHealth;
                float enemyMaxhp = other.GetComponent<Network_EnemyHealth>().maxHeatlh;
                string enemyName = "";
                if (other.GetComponent<Network_Enemy>())
                {
                    enemyName = other.GetComponent<Network_Enemy>().enemyName;
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

    private void FireRateIncrease()
    {
        fireRate = Mathf.Lerp(currentFireRate, maxFireRate, Time.deltaTime * 12f);
    }

    private void UltraFireRateIncrease()
    {
        ultrafireRate = Mathf.Lerp(currentUltraFireRate, maxFireRate, Time.deltaTime * 5f);
    }
}