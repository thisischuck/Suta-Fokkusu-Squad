using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy
{
    public int Health;
    public Vector3 Velocity;
    public List<Attack> Attacks;
    public Animation Idle;
    public Animation Death;

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


