using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HBController : MonoBehaviour {
    public RectTransform hp;
    RectTransform x;
    public Transform camara;
    public RectTransform canvas;
    private float maxHealth;
    private float currentHealth;

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
        if (currentHealth > 0)
        {
            x.transform.position = (Vector3.up * 1.5f) + transform.position;

            Quaternion targetRotation = Quaternion.LookRotation(camara.position - x.position);
            // Smoothly rotate towards the target point.
            x.transform.rotation = Quaternion.Slerp(x.transform.rotation, targetRotation, 1f);

            x.GetComponent<HealthBar>().realAmout = Mathf.Lerp(x.GetComponent<HealthBar>().realAmout, currentHealth, 5f * Time.deltaTime);
        }
        else
        {
            Destroy(this);
        }
    }
}
