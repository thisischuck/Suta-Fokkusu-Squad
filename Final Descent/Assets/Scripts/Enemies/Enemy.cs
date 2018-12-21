using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected StateMachine stateMachine;
    protected Vector3 Velocity;
    protected float MaxVelocity;
    public bool isThereAnything = false;
    protected float MaxRotationSpeed;
    protected GameObject player;
    protected HealthEnemy healthEnemy;
    protected ParticleSystem particleSys;
    public List<Attack> Attacks;
    public string IdleClip;
    public string DeathClip;
    public string Movement;
    public string Spawn;

    public string enemyName = "";

    [HideInInspector]
    public Animation animController;

    protected virtual void Start()
    {
        player = GetClosestPlayer();
        healthEnemy = GetComponent<HealthEnemy>();
        animController = GetComponentInChildren<Animation>();
        particleSys = GetComponent<ParticleSystem>();
    }

    protected virtual void AssignState(StateMachine_Node start)
    {
        stateMachine = new StateMachine(start);
    }

    protected virtual void Update()
    {
        Debug.Log(stateMachine.currentNode.ToString());
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

        if (healthEnemy.health <= 0)
            Destroy(this.gameObject);

    }

    public void PlayAnimation(string name)
    {
        animController.CrossFade(name, 0.2f, PlayMode.StopAll);

    }

    public void StopOrPlayParticleSystem(bool playPs, ParticleSystem ps)
    {
        if (playPs && !ps.isPlaying)
        {
            ps.Play();
        }

        if (ps.isPlaying && !playPs)
        {
            ps.Clear(); ps.Stop();
        }
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


