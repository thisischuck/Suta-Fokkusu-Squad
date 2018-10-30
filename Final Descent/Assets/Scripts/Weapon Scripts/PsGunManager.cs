using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PsGunManager : MonoBehaviour
{
    private ParticleSystem system;

    public bool canFire, isActive;
    public int bulletsPerClick = 1;
    public float fireRate = 0.45f; //segundos

    // Use this for initialization
    void Start()
    {
        isActive = this.GetComponentInChildren<MeshRenderer>().enabled;

        canFire = true;
        system = this.gameObject.GetComponent<ParticleSystem>();
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

    IEnumerator FireRateIE()
    {
        canFire = false;
        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }
}
