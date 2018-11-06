using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected int Health;
    protected Vector3 Velocity;
    protected float MaxVelocity;
    protected List<Attack> Attacks;
    protected Animation Idle;
    protected Animation Death;

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        if (Health <= 0)
            Destroy(this.gameObject);

        transform.position += Velocity.normalized * Time.deltaTime;
        transform.forward = Velocity.normalized;
    }
}

[System.Serializable]
public struct Attack
{
    public int Damage;
    public float Knockback;
    public Status StatusEffect;
    public Animation Anim;
}

public enum Status { POISON, STUN }


