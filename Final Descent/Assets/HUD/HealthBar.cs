using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    public float realAmout;

    [SerializeField]
    private float amount;
    private float maxHealth;

    [SerializeField]
    private Image content;

	// Use this for initialization
	void Start () {
		
	}
	
    public void SetMaxHealth(float health)
    {
        maxHealth = health;
    }

	// Update is called once per frame
	void Update () {
        amount = ScaleValues(realAmout, maxHealth);
        UpdateBar();
	}

    private void UpdateBar()
    {
        content.fillAmount = amount;
    }

    private float ScaleValues(float value,  float inMax, float inMin = 0, float outMin = 0, float outMax = 1)
    {
        return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }
}
