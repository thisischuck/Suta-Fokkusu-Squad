using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStats : MonoBehaviour {
    public int lives = 1;
    public float shield;
    public float health;
    public float base_maxHealth;

    public float invulnerabilityTime = 0.0f;
    [SerializeField]
    public float invCount = 0.0f;
    public bool IsInvulnerable = false;

    public bool IsAlive = true;

    public virtual void TakeDamage(float health)
    {
        this.health -= health;
        if (invulnerabilityTime != 0)
        {
            IsInvulnerable = true;
            if (this.health <= health)
            {
                IsAlive = false;
            }
        }
    }

    public void GenerateVariables(float health, float shield, int lives = 1, float invTime = 0.0f)
    {
        this.lives = lives;
        this.health = health;
        this.shield = shield;
        invulnerabilityTime = invTime;
    }

    public void InvulnerableController()
    {
        if (IsInvulnerable)
        {
            invCount += 1 * Time.deltaTime;
            if (invCount >= invulnerabilityTime)
            {
                IsInvulnerable = false;
            }
        }
    }


}
