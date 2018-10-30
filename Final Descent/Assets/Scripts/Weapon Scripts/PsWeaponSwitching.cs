using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PsWeaponSwitching : MonoBehaviour
{
    public List<ParticleSystem> Weapons;

    int previousWeapon, selectedWeapon;

    // Use this for initialization
    void Start()
    {
        selectedWeapon = 0;

        foreach (ParticleSystem a in Weapons)
        {
            a.GetComponentInChildren<MeshRenderer>().enabled = false;
        }
        Weapons[selectedWeapon].GetComponentInChildren<MeshRenderer>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        previousWeapon = selectedWeapon;

        if (Input.GetKey(KeyCode.Alpha1))
        {
            selectedWeapon = 0;
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            selectedWeapon = 1;
        }
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            selectedWeapon = 2;
        }
        else if (Input.GetKey(KeyCode.Alpha4))
        {
            selectedWeapon = 3;
        }

        //Only if we change weapons we call the SelectWapon() function to update which weapon is being used and activate it
        if (previousWeapon != selectedWeapon)
        {
            SelectWeapon();
        }
    }


    void SelectWeapon()
    {
        foreach (ParticleSystem a in Weapons)
        {
            a.GetComponentInChildren<MeshRenderer>().enabled = false;
        }

        Weapons[selectedWeapon].GetComponentInChildren<MeshRenderer>().enabled = true;
    }


}

