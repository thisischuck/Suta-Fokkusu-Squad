using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 9/10/2018
 Marcio- we gucci on all
 */
public class Gun : MonoBehaviour {

    public Transform bullet;
    public List<Transform> shootPos;
    public Transform weaponHolder;
    public bool isLaser;

    //Values dependent of the weapon
    public float fireRate = .25f;
    public int maxAmmo;
    public float reloadTime = 1f;

    public int currentAmmo;
    private bool isReloading = false;

    private int shot_pos_iterator = 0;

    float nextTimeToFire = 0f;

    private void Start()
    {
        currentAmmo = maxAmmo;

        if (GetComponent<MeshFilter>() == null)
            isLaser = true;
        else isLaser = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isReloading)
            return;

        if (currentAmmo <= 0)
        {
            weaponHolder.GetComponent<WeaponSwitching>().SetAmmoActive(false);
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + fireRate;
            Debug.Log("currentAmmo:" + currentAmmo + " shotpos:" + shootPos[shot_pos_iterator].position + " shotRot:" + shootPos[shot_pos_iterator].position);
            weaponHolder.GetComponent<WeaponSwitching>().Shoot(currentAmmo, shootPos[shot_pos_iterator].position, shootPos[shot_pos_iterator].rotation);
            UpdateShootPos();
            currentAmmo--;
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading");

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;
    }

    void UpdateShootPos()
    {
        if (shot_pos_iterator >= shootPos.Count - 1)
            shot_pos_iterator = 0;
        else
            shot_pos_iterator++;
    }
}
