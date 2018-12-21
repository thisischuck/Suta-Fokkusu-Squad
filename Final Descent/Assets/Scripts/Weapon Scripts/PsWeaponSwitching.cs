using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PsWeaponSwitching : MonoBehaviour
{
    public List<MeshRenderer> Weapons;
    public GameObject slot1, slot2, slot3;

    int previousWeapon, selectedWeapon;

    // Use this for initialization
    void Start()
    {
        if (PlayerStatsInfo.currentWeapons[0] != null)
            slot1.GetComponent<Morph>().Morpher(PlayerStatsInfo.currentWeapons[0].weaponModel, out slot1);
        if (PlayerStatsInfo.currentWeapons[1] != null)
            slot2.GetComponent<Morph>().Morpher(PlayerStatsInfo.currentWeapons[1].weaponModel, out slot1);
        if (PlayerStatsInfo.currentWeapons[2] != null)
            slot3.GetComponent<Morph>().Morpher(PlayerStatsInfo.currentWeapons[2].weaponModel, out slot1);

        if (slot1.GetComponent<MeshRenderer>() != null)
            Weapons.Add(slot1.GetComponent<MeshRenderer>());
        if (slot2.GetComponent<MeshRenderer>() != null)
            Weapons.Add(slot2.GetComponent<MeshRenderer>());
        if (slot3.GetComponent<MeshRenderer>() != null)
            Weapons.Add(slot3.GetComponent<MeshRenderer>());

        selectedWeapon = 0;

        SelectWeapon();
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
        foreach (MeshRenderer a in Weapons)
        {
            a.enabled = false;
        }

        Weapons[selectedWeapon].enabled = true;
    }


}

