using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPlayer : BaseStats {

	public GameObject hpBar, shieldBar;
	private bool exploded = false;
	public ParticleSystem damagedShip, areaPulse;
	public bool collision = false;
	public GameObject Shield;

	// Use this for initialization
	void Start () {
        hpBar.GetComponent<HealthBar>().SetMaxHealth(base_maxHealth);
        shieldBar.GetComponent<HealthBar>().SetMaxHealth(base_maxShield);
    }
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown(KeyCode.F) && health > 0)
			TakeDamage(50);

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

	override public void TakeDamage(float damage)
	{
		if (shield > 0)
		{
			if (!Shield.GetComponent<ShieldController>().fadeIn)
			{
				Shield.GetComponent<ShieldController>().fadeIn = true;
				Shield.GetComponent<ShieldController>().fadeOut = false;
				TakeDamageShield(damage * 2);
			}
			else
			{
				Shield.GetComponent<ShieldController>().upTime = 0.0f;
				Shield.GetComponent<ShieldController>().fadeOut = false;
				TakeDamageShield(damage);
			}
		}
		else
			TakeDamageHealth(damage);
	}

	private void TakeDamageShield(float damage)
	{
		shield -= damage;
	}

    private void TakeDamageHealth(float damage)
    {
        health -= damage;
        if (invulnerabilityTime != 0)
            IsInvulnerable = true;

        if (health <= 0 && lives > 0)
        {
			lives--;
			SpawnLifePod();
            DeathExplosion();
			KillAround();
		}
		else if (health <= 0 && lives <= 0 && !exploded)
		{
			IsAlive = false;
			DeathExplosion();
			GameObject.Find("InGame_MainCanvas").GetComponent<MenuController>().GameOver = true;
		}
		else if (transform.Find("LifePod").gameObject.activeSelf == true)
		{
			IsAlive = false;
			GameObject.Find("InGame_MainCanvas").GetComponent<MenuController>().GameOver = true;
		}
	}

    private void UpdateBars()
    {
        hpBar.GetComponent<HealthBar>().currentAmout = Mathf.Lerp(hpBar.GetComponent<HealthBar>().currentAmout, health, 5f * Time.deltaTime);
        shieldBar.GetComponent<HealthBar>().currentAmout = Mathf.Lerp(shieldBar.GetComponent<HealthBar>().currentAmout, shield, 5f * Time.deltaTime);
    }

    private void DeathExplosion()
    {
		Transform t = GameObject.Find("Player_aircraft").transform;

		//Desativar arma
		ActivateChildScripts(t.Find("WeaponHolder"), false);

		//Desativar aircraft e explodir todaspeças
		ActivateChildScripts(t.Find("Aircraft"), false);
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
        lifepod.gameObject.SetActive(true);

		invulnerabilityTime = 2f;
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

    private void ActivateChildScripts(Transform p, bool active)
    {
		if(p.name == "Aircraft")
		{
			for (int i = p.childCount - 1; i >= 0; i--)
			{
				Transform child = p.GetChild(i);

				if (child.tag == ("PlayerPart"))
				{
					if (child.GetComponent<TurbineMovement>() != null)
						child.GetComponent<TurbineMovement>().enabled = active;
					if (!active && IsAlive)
					{
						child.GetComponent<ShipPart>().Explode();
						if (IsAlive)
							child.GetComponent<ShipPart>().defDeath = false;
						else
							child.GetComponent<ShipPart>().defDeath = true;
					}
				}
			}
		}
		else
		{
			p.GetComponent<PsWeaponSwitching>().enabled = active;

			for (int i = p.childCount - 1; i >= 0; i--)
			{
				Transform child = p.GetChild(i);
				if (child.GetComponent<Morph>() != null)
				{
					foreach (Transform c in child)
					{
						if (c.GetComponent<PsGunManager>() != null)
							c.GetComponent<PsGunManager>().enabled = active;
					}
					child.GetComponent<Morph>().enabled = active;
				}
			}
			//Add the explosion
			if (!active && IsAlive)
			{
				p.GetComponent<ShipPart>().Explode();
				if (IsAlive)
					p.GetComponent<ShipPart>().defDeath = false;
				else
					p.GetComponent<ShipPart>().defDeath = true;
			}
		}
	}

	private void KillAround()
	{
		LayerMask mask = LayerMask.GetMask("Enemy");
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, 26, mask);
		Debug.Log(hitColliders.Length);
		if(hitColliders != null)
		{
			for (int i = 0; i <= hitColliders.Length - 1; i++)
			{
				Debug.Log(hitColliders[i].name);
				if (hitColliders[i].GetComponent<HealthEnemy>())
				{
					Debug.Log("has he");
					hitColliders[i].GetComponent<HealthEnemy>().health = 0;
				}
				else if(hitColliders[i].transform.parent != null)
				{
					Debug.Log("parent has he");
					hitColliders[i].transform.parent.GetComponent<HealthEnemy>().health = 0;
				}
			}
		}
	}
}
