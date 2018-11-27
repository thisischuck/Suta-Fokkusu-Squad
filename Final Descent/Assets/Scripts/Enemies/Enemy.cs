using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected StateMachine stateMachine;
    protected Vector3 Velocity;
    protected float MaxVelocity;
    public List<Attack> Attacks;
    public string IdleClip;
    public string DeathClip;
    public string Movement;
    public string Spawn;

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

    protected virtual void AssignState(StateMachine_Node start)
    {
        stateMachine = new StateMachine(start);
    }

    protected virtual void Update()
    {
        List<Action> actions = stateMachine.Run();
        if (actions != null)
        {
            foreach (var a in actions)
            {
                if (a != null)
                {
                    a.Invoke();
                }
            }
        }
        Debug.Log(stateMachine.currentNode.ToString());

        if (GetComponent<HealthEnemy>().health <= 0)
            Destroy(this.gameObject);

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
}

public enum Status { NONE, POISON, STUN, SLOW }


