using UnityEngine;

public class Network_PlayerHealth : Network_BaseHealth
{
    public GameObject hpBar, shieldBar;

    // Use this for initialization
    void Start()
    {
        currentHealth = maxHeatlh;
        currentShield = maxShield;
        hpBar = GameObject.Find("HpBar_background");
        shieldBar = GameObject.Find("ShieldBar_background");
        hpBar.GetComponent<HealthBar>().SetMaxHealth(maxHeatlh);
        shieldBar.GetComponent<HealthBar>().SetMaxHealth(maxShield);

    }

    // Update is called once per frame
    void Update()
    {
        if (hpBar == null && shieldBar == null)
        {
            if (hasAuthority)
            {
                hpBar = GameObject.Find("HpBar_background");
                shieldBar = GameObject.Find("ShieldBar_background");
                hpBar.GetComponent<HealthBar>().SetMaxHealth(maxHeatlh);
                shieldBar.GetComponent<HealthBar>().SetMaxHealth(maxShield);
            }
        }
        else
        {
            UpdateBars();
        }
    }

    public void TakeDamage(float damage)
    {
        if (!isServer)
        {
            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                isAlive = false;
                //transform.parent.Find("PlayerController").GetComponent<PlayerMovement>().enabled = false;
                //DeathExplosion();
            }
        }
    }

    public void TakeDamageShield(float damage)
    {
        currentShield -= damage;
    }

    private void UpdateBars()
    {
        hpBar.GetComponent<HealthBar>().currentAmout = Mathf.Lerp(hpBar.GetComponent<HealthBar>().currentAmout, currentHealth, 5f * Time.deltaTime);
        shieldBar.GetComponent<HealthBar>().currentAmout = Mathf.Lerp(hpBar.GetComponent<HealthBar>().currentAmout, currentShield, 5f * Time.deltaTime);
    }
}
