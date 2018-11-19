using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected Vector3 Velocity;
    protected float MaxVelocity;
    public List<Attack> Attacks;
    public AnimationClip IdleClip; // Not sure if this is needed. You can just make it as an attack with 0 dmg and 0 knockback
    public AnimationClip DeathClip;// Same
    [HideInInspector]
    public Animation Idle, Death;

    [HideInInspector]
    public Animation animController;

    public bool isPlaying;

    protected virtual void Start()
    {
        //Idle = new Animation();
        //Idle.clip = IdleClip;
        //Death = new Animation();
        //Death.clip = DeathClip;

        animController = GetComponentInChildren<Animation>();
        isPlaying = false;
    }

    protected virtual void Update()
    {
        if (GetComponent<HealthEnemy>().health <= 0)
            Destroy(this.gameObject);

        transform.position += Velocity.normalized * Time.deltaTime;
        transform.forward = Velocity.normalized;

        if (!animController.isPlaying)
            isPlaying = false;
    }

    public void PlayAnimation(string name)
    {
        if (!isPlaying)
            animController.Play(name);
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
    public string Name;
    public int Damage;
    public float Knockback;
    public Status StatusEffect;

    //This two are not needed. You need to add a Animation Component Somewhere and add all the clips through that.
    //When you want to play it you just need to give it a name so we need to keep the names the same.
    /* 
    public AnimationClip Clip;
    public Animation Animation;
    */
}

public enum Status { NONE, POISON, STUN }


