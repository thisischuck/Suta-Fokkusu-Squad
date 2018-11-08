﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthEnemy : BaseStats {

    private void Start()
    {
        GenerateVariables(100, 0);
        GetComponent<HBController>().StartHPBar(health);
    }

    // Update is called once per frame
    void Update () {
        GetComponent<HBController>().SetCurrentHealth(health);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Bullet")
        {
            float damage = col.gameObject.GetComponent<Mover>().Damage;
            ApplyDamage(damage);
        }
    }

    private void ApplyDamage(float damage)
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Hit");
            health -= damage;
        }
    }
}
