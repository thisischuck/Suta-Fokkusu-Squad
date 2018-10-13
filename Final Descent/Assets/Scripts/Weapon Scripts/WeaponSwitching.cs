using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 8/10/2018
 Marcio- switching system works perfectly
 9/10/2018
 Marcio - changing switching system controls, from scrollwheel to buttons     
        - added ammo creation, storage and creation. List of ammo is not cleaning nor creating ammo when supposed
     */
public class WeaponSwitching : MonoBehaviour
{

    public int selectedWeapon = 0;
    public int previousWeapon;

    //Ammo Stuff
    int maxAmmo;
    public List<Transform> ammo;
    Transform bullet;

    public List<MeshFilter> ammoT;
    //MeshFilter mesh;

    // Use this for initialization
    void Start()
    {

        SelectWeapon();
        ammo = new List<Transform>();
        CreateAmmo();
        ChangeMesh(ammoT[0].sharedMesh);

    }

    // Update is called once per frame
    void Update()
    {

        previousWeapon = selectedWeapon;

        if (Input.GetKey(KeyCode.Alpha1))
        {
            selectedWeapon = 0;
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            selectedWeapon = 1;
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            selectedWeapon = 2;
        }

        if (Input.GetKey(KeyCode.Alpha4))
        {
            ChangeMesh(ammoT[1].sharedMesh);
            Debug.Log("bullet changed");
        }

        //Only if we change weapons we call the SelectWapon() function to update which weapon is being used and activate it
        if (previousWeapon != selectedWeapon)
        {
            ClearAmmo();
            SelectWeapon();
            CreateAmmo();
        }
    }

    void ChangeMesh(Mesh tmesh)
    {
        foreach (Transform t in ammo)
        {
            t.GetComponent<MeshFilter>().mesh = tmesh;
        }
    }

    //Find and activate the selected weapon
    void SelectWeapon()
    {
        int i = 0;

        //Goes through the transform children to activate and deactivate the weapons GameObject
        foreach (Transform weapon in transform)
        {
            if (i == selectedWeapon)
            {
                Gun g = weapon.GetComponent<Gun>();
                weapon.gameObject.SetActive(true);
                maxAmmo = g.maxAmmo;
                bullet = g.bullet;
            }
            else weapon.gameObject.SetActive(false);
            i++;
        }
    }

    void ClearAmmo()
    {
        for (int i = maxAmmo - 1; i >= 0; i--)
        {
            Destroy(ammo[i].gameObject);
        }
        ammo.Clear();
    }

    public void Shoot(int currentAmmo, Vector3 position, Quaternion rotation)
    {
        int nextActive = maxAmmo - currentAmmo;

        Mover mover = ammo[nextActive].gameObject.GetComponent<Mover>();

        mover.setPosition(position, rotation);
        ammo[nextActive].gameObject.SetActive(true);
        mover.isActive = true;
        mover.StartCour();
    }

    void CreateAmmo()
    {
        for (int i = 0; i <= maxAmmo - 1; i++)
        {
            ammo.Add(Instantiate(bullet));
        }

        SetAmmoActive(false);
    }

    public void SetAmmoActive(bool active)
    {
        foreach (Transform b in ammo)
        {
            b.gameObject.SetActive(active);
        }
    }
}
