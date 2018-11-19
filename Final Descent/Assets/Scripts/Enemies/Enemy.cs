using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected Vector3 Velocity;
    protected float MaxVelocity;
    public List<Attack> Attacks;
    public AnimationClip IdleClip;
    public AnimationClip DeathClip;
    [HideInInspector]
    public Animation Idle, Death;

    protected virtual void Start()
    {
        //Idle = new Animation();
        //Idle.clip = IdleClip;
        //Death = new Animation();
        //Death.clip = DeathClip;
        foreach (Attack a in Attacks)
        {
            //a.Animation = new Animation();
            a.Animation.AddClip(a.Clip, "");
        }
    }

    protected virtual void Update()
    {
        if (GetComponent<HealthEnemy>().health <= 0)
            Destroy(this.gameObject);

        transform.position += Velocity.normalized * Time.deltaTime;
        transform.forward = Velocity.normalized;
    }

    protected GameObject GetClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int closest = 0;
        for (int i = 1; i < players.Length; i++)
        {
            if (Vector3.Distance(this.transform.position, players[i].transform.position) <
            Vector3.Distance(this.transform.position, players[closest].transform.position))
            {
                closest = i;
            }
        }
        return players[closest];
    }
}

[System.Serializable]
public class Attack
{
    public int Damage;
    public float Knockback;
    public Status StatusEffect;
    public AnimationClip Clip;
    public Animation Animation;
}

public enum Status { NONE, POISON, STUN }


