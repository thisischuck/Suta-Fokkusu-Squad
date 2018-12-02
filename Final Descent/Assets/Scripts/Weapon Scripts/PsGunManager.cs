using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PsGunManager : MonoBehaviour
{
    private ParticleSystem system;
    private List<ParticleCollisionEvent> collisionEvents;

    public bool canFire, isActive;
    public int bulletsPerClick = 1;
    public float fireRate = 0.45f; //segundos

    // Use this for initialization
    void Start()
    {
        isActive = this.GetComponentInChildren<MeshRenderer>().enabled;

        canFire = true;
        system = this.gameObject.GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    // Update is called once per frame
    void Update()
    {
        isActive = this.GetComponentInChildren<MeshRenderer>().enabled;

        if (isActive)
            if (Input.GetButton("Fire1") && canFire)
            {
                system.Emit(bulletsPerClick);
                StartCoroutine(FireRateIE());
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
            if (other.GetComponent<EnemyCollision>())
            {
                other.GetComponent<EnemyCollision>().TakeDamage(15);
            }
        }
    }

    IEnumerator FireRateIE()
    {
        canFire = false;
        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }
}
