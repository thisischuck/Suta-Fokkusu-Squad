using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Network_Enemy : NetworkBehaviour {
    protected StateMachine stateMachine;
    protected Vector3 Velocity;
    protected float MaxVelocity;
    public bool isThereAnything = false;
    protected float MaxRotationSpeed;
    [SyncVar]
    public GameObject player;
    protected Network_EnemyHealth healthEnemy;
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
        healthEnemy = GetComponent<Network_EnemyHealth>();
        animController = GetComponentInChildren<Animation>();
        particleSys = GetComponent<ParticleSystem>();

        if (isServer)
            player = GetClosestPlayer();
    }

    protected virtual void AssignState(StateMachine_Node start)
    {
        stateMachine = new StateMachine(start);
    }

    protected virtual void Update()
    {
        //Debug.Log(stateMachine.currentNode.ToString());
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

        if (healthEnemy.currentHealth <= 0)
            Destroy(this.gameObject);

        player = GetClosestPlayer();
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

    public virtual GameObject GetClosestPlayer()
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