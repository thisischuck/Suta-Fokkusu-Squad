﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HBController : MonoBehaviour {
    public RectTransform hp;
    RectTransform x;
    public Transform camara;
    public RectTransform canvas;
    private float maxHealth = 1;
    private float currentHealth = 1;

    public void SetCurrentHealth(float currentHealth)
    {
        this.currentHealth = currentHealth;
    }

	// Use this for initialization
	public void StartHPBar(float maxHealth) {

        x = Instantiate(hp);
        x.transform.SetParent(canvas.transform, true);

        this.maxHealth = maxHealth;
        x.GetComponent<HealthBar>().SetMaxHealth(maxHealth);

    }

    // Update is called once per frame
    void Update() {
        if (currentHealth >= 0)
        {
            x.transform.position = (Vector3.up * 1.5f) + transform.position;

            Quaternion targetRotation = Quaternion.LookRotation(camara.position - x.position);
            // Smoothly rotate towards the target point.
            x.transform.rotation = Quaternion.Slerp(x.transform.rotation, targetRotation, 1f);

            x.GetComponent<HealthBar>().currentAmout = Mathf.Lerp(x.GetComponent<HealthBar>().currentAmout, currentHealth, 5f * Time.deltaTime);
        }
        else
        {
            Destroy(this);
        }
    }
}