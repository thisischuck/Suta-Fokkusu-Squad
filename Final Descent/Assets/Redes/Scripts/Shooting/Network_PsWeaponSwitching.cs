using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Network_PsWeaponSwitching : NetworkBehaviour
{
    public List<MeshRenderer> Weapons;
    [SyncVar]
    public GameObject slot1;
    [SyncVar]
    public GameObject slot2;
    [SyncVar]
    public GameObject slot3;

    public int previousWeapon, selectedWeapon;

    public void SetWeapons()
    {
        
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Weapons.Count <= 0)
        {
            Weapons.Add(slot1.GetComponent<MeshRenderer>());
            Weapons.Add(slot2.GetComponent<MeshRenderer>());
            Weapons.Add(slot3.GetComponent<MeshRenderer>());
        }
        if (hasAuthority)
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
            PlayerStatsInfo.selectedWeapon = selectedWeapon;

            //Only if we change weapons we call the SelectWapon() function to update which weapon is being used and activate it
            if (previousWeapon != selectedWeapon)
            {
                SelectWeapon();
            }
        }
    }


    void SelectWeapon()
    {
        if (Weapons.Count > 0)
        {
            foreach (MeshRenderer a in Weapons)
            {
                a.enabled = false;
            }

            Weapons[selectedWeapon].enabled = true;
        }
    }


}
