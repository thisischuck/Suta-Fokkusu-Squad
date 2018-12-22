using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPlayer : BaseStats {

	public GameObject hpBar, shieldBar;
	private bool exploded = false;
	public ParticleSystem damagedShip, areaPulse;
	public bool collision = false;

	// Use this for initialization
	void Start () {
        GenerateVariables(100,100);
        hpBar.GetComponent<HealthBar>().SetMaxHealth(health);
        shieldBar.GetComponent<HealthBar>().SetMaxHealth(shield);
        //GetComponent<HBController>().SetMaxHealth(health);
    }
	
	// Update is called once per frame
	void Update () {

		if (exploded && CheckIfOnPlace() && Input.GetKey(KeyCode.G))
        {
            Transform lifepod = transform.Find("LifePod");
            GameObject playerAircraft = GameObject.Find("Player_aircraft");
			GameObject aircraft = GameObject.Find("Aircraft");

			OrganizeParts(aircraft.transform);
            lifepod.position = aircraft.transform.position;
            lifepod.rotation = aircraft.transform.rotation;
            transform.rotation = aircraft.transform.rotation;
            aircraft.transform.parent = playerAircraft.transform;
			playerAircraft.transform.parent = transform;
			playerAircraft.transform.localPosition = Vector3.zero;
			//playerAircraft.transform.position = transform.position;
			transform.GetComponent<Ship>().enabled = true;
			lifepod.gameObject.SetActive(false);
            playerAircraft.GetComponent<ShipRotation>().enabled = true;
            ActivateChildScripts(aircraft.transform, true);
			ActivateChildScripts(playerAircraft.transform.Find("WeaponHolder"), true);
			exploded = false;
			areaPulse.gameObject.SetActive(false);

			health = base_maxHealth;
			shield = base_maxShield;
		}
		if (!CheckIfOnPlace() && exploded)
			damagedShip.gameObject.SetActive(true);
		else if (CheckIfOnPlace() && exploded)
		{
			damagedShip.gameObject.SetActive(false);
			areaPulse.gameObject.SetActive(true);
		}

		if(exploded)
		{
			hpBar.SetActive(false);
			shieldBar.SetActive(false);
		}
		else
		{
			hpBar.SetActive(true);
			shieldBar.SetActive(true);
			UpdateBars();
		}
    }

	public void TakeDamageShield(float damage)
	{
		shield -= damage;
	}

    override public void TakeDamage(float damage)
    {
        health -= damage;
        if (invulnerabilityTime != 0)
        {
            IsInvulnerable = true;
            /*if (this.lives <= 0)
            {
                IsAlive = false;
            }*/
        }
        if (health <= 0 && lives > 0)
        {
			lives--;
			SpawnLifePod();
			Debug.Log("spawnlifepod");
            DeathExplosion();
        }
		else if (health <= 0 && lives <= 0)
		{
			IsAlive = false;
			lives--;
			//transform.parent.Find("PlayerController").GetComponent<PlayerMovement>().enabled = false;
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
        Transform t = GameObject.Find("Player_aircraft").transform;
		Transform player = t.Find("Aircraft");
		//Remove every child inside Aircraft transform
		ActivateChildScripts(t.Find("WeaponHolder"), false);

		ActivateChildScripts(player, false);
		t.GetComponent<ShipRotation>().enabled = false;
		if (IsAlive)
			t.parent = null;
		exploded = true;
		areaPulse.gameObject.SetActive(true);
    }

    private void SpawnLifePod()
    {
        Vector3 pos = transform.Find("Player_aircraft").position;
        Quaternion rot = transform.Find("Player_aircraft").rotation;
        Transform lifepod = transform.Find("LifePod");

        //set lifepod active and position and rotation right 
        //lifepod.position = pos;
        //lifepod.rotation = rot;
        lifepod.gameObject.SetActive(true);
        //GetComponent<PlayerMovement>().lifePodActive = true; //Change object for camera to follow
        //transform.rotation = lifepod.rotation;
    }

    private void OrganizeParts(Transform parent)
    {
        GameObject[] playerPart = GameObject.FindGameObjectsWithTag("PlayerPart");

        foreach( GameObject part in playerPart)
        {
			if(part.name == "WeaponHolder")
				part.transform.parent = parent.parent;
			else
			part.transform.parent = parent;
        }
    }

    private bool CheckIfOnPlace()
    {
        Transform aircraft = GameObject.Find("Player_aircraft").transform.Find("Aircraft");

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
            if (!active && IsAlive)
                child.GetComponent<ShipPart>().Explode();
				child.GetComponent<ShipPart>().defDeath = false;
			}
			else if (!active && !IsAlive)
			{
				child.GetComponent<ShipPart>().Explode();
				child.GetComponent<ShipPart>().defDeath = true;
			}
		}
	}
}
