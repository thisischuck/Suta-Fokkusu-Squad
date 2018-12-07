using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPlayer : BaseStats {

    public GameObject hpBar, shieldBar;

	// Use this for initialization
	void Start () {
        GenerateVariables(100,100);
        hpBar.GetComponent<HealthBar>().SetMaxHealth(health);
        shieldBar.GetComponent<HealthBar>().SetMaxHealth(shield);
        //GetComponent<HBController>().SetMaxHealth(health);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.F))
        {
            TakeDamage(50);
        }

        if (CheckIfOnPlace() && Input.GetKey(KeyCode.G))
        {
            Transform lifepod = transform.Find("LifePod");
            GameObject aircraft = GameObject.Find("Aircraft");

            OrganizeParts();
            lifepod.position = aircraft.transform.position;
            lifepod.rotation = aircraft.transform.rotation;
            transform.rotation = aircraft.transform.rotation;
            aircraft.transform.parent = transform;
            lifepod.gameObject.SetActive(false);
            transform.Find("Aircraft").GetComponent<Ship>().enabled = true;
            ActivateChildScripts(transform.Find("Aircraft"), true);
            GetComponent<PlayerMovement>().lifePodActive = false;
        }

        UpdateBars();
    }

    override public void TakeDamage(float damage)
    {
        health -= damage;
        if (invulnerabilityTime != 0)
        {
            IsInvulnerable = true;
            if (this.lives <= 0)
            {
                IsAlive = false;
            }
        }
        if (health <= 0)
        {
            SpawnLifePod();
            DeathExplosion();
        }
    }

    private void UpdateBars()
    {
        hpBar.GetComponent<HealthBar>().currentAmout = Mathf.Lerp(hpBar.GetComponent<HealthBar>().currentAmout, health, 5f * Time.deltaTime);
        shieldBar.GetComponent<HealthBar>().currentAmout = Mathf.Lerp(hpBar.GetComponent<HealthBar>().currentAmout, shield, 5f * Time.deltaTime);
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
        transform.rotation = lifepod.rotation;
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
                if (child.GetComponent<TurbineMovement>() != null)
                    child.GetComponent<TurbineMovement>().enabled = active;
            if (child.GetComponent<WeaponSwitching>() != null)
            {
                foreach (Transform c in child)
                {
                    if (c.GetComponent<Gun>() != null)
                        c.GetComponent<Gun>().enabled = active;
                }
                child.GetComponent<WeaponSwitching>().enabled = active;
            }
            if (!active)
                child.GetComponent<ShipPart>().Explode();
            }
        }
    }
}
