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

        if (Input.GetKey(KeyCode.G))
        {
            OrganizeParts();
            transform.Find("LifePod").gameObject.SetActive(false);
            transform.Find("LifePod").position = GameObject.Find("Aircraft").transform.position;
            transform.Find("LifePod").rotation = GameObject.Find("Aircraft").transform.rotation;
            GameObject.Find("Aircraft").transform.parent = transform;
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
        foreach (Transform child in t)
        {
            //deactivate all components
            //optimizar codigo
            if(child.GetComponent<Mover>() != null)
            child.GetComponent<Mover>().enabled = false;
            if (child.GetComponent<TurbineMovement>() != null)
                child.GetComponent<TurbineMovement>().enabled = false;
            if (child.GetComponent<WeaponSwitching>() != null)
                child.GetComponent<WeaponSwitching>().enabled = false;

            child.GetComponent<ShipPart>().Explode();
        }
        t.GetComponent<Ship>().enabled = false;
        t.parent = null; // otherwise it will follow the lifepod's position
    }

    private void SpawnLifePod()
    {
        Vector3 pos = transform.Find("Aircraft").position;
        Quaternion rot = transform.Find("Aircraft").rotation;

        //set lifepod active
        transform.Find("LifePod").position = pos;
        transform.Find("LifePod").rotation = rot;
        transform.Find("LifePod").gameObject.SetActive(true);
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
}
