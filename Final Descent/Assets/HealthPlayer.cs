using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPlayer : MonoBehaviour {

    public int Lives = 3;
    public float Shield;
    public float Health = 100f;

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.F))
        {
            ApplyDamage(50);
        }

        if (CheckIfOnPlace() && Input.GetKey(KeyCode.G))
        {
            Transform lifepod = transform.Find("LifePod");
            GameObject aircraft = GameObject.Find("Aircraft");

            OrganizeParts();
            lifepod.gameObject.SetActive(false);
            lifepod.position = aircraft.transform.position;
            lifepod.rotation = aircraft.transform.rotation;
            aircraft.transform.parent = transform;
            transform.Find("Aircraft").GetComponent<Ship>().enabled = true;
            ActivateChildScripts(transform.Find("Aircraft"), true);
            GetComponent<PlayerMovement>().lifePodActive = false;
        }
    }

    private void ApplyDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            SpawnLifePod();
            DeathExplosion();
        }
    }

    private void DeathExplosion()
    {
        Transform t = transform.Find("Aircraft");
        //Remove every child inside Aircraft transform
        ActivateChildScripts(t, false);
        t.GetComponent<Ship>().enabled = false;
        t.parent = null; // otherwise it will follow the lifepod's position
    }

    private void SpawnLifePod()
    {
        Vector3 pos = transform.Find("Aircraft").position;
        Quaternion rot = transform.Find("Aircraft").rotation;
        Transform lifepod = transform.Find("LifePod");

        //set lifepod active and position and rotation right 
        lifepod.position = pos;
        lifepod.rotation = rot;
        lifepod.gameObject.SetActive(true);
        GetComponent<PlayerMovement>().lifePodActive = true; //Change object for camera to follow
    }

    private void OrganizeParts()
    {
        Transform aircraft = GameObject.Find("Aircraft").transform;

        GameObject[] playerPart = GameObject.FindGameObjectsWithTag("PlayerPart");

        foreach( GameObject part in playerPart)
        {
            part.transform.parent = aircraft;
        }
    }

    private bool CheckIfOnPlace()
    {
        Transform aircraft = GameObject.Find("Aircraft").transform;

        GameObject[] playerPart = GameObject.FindGameObjectsWithTag("PlayerPart");

        foreach (GameObject part in playerPart)
        {
            if (part.GetComponent<ShipPart>().onPlace == false)
                return false;
        }
        return true;
    }

    private void ActivateChildScripts(Transform parent, bool active)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);

            if (child.tag == ("PlayerPart"))
            {
                if (child.GetComponent<Mover>() != null)
                    child.GetComponent<Mover>().enabled = active;
                if (child.GetComponent<TurbineMovement>() != null)
                    child.GetComponent<TurbineMovement>().enabled = active;
                if (child.GetComponent<WeaponSwitching>() != null)
                {
                    child.GetComponent<WeaponSwitching>().enabled = active;
                    foreach( Transform c in child)
                    child.parent.GetComponent<Gun>().enabled = active;
                }
                    
                if(!active)
                child.GetComponent<ShipPart>().Explode();
            }
        }

    }
}
