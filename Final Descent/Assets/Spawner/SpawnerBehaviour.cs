using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBehaviour : MonoBehaviour {
    public Transform player;
    public bool active;
    public StateMachine sM;
    public bool alive = true;
    private Animator animator;

    public float count = 0;
    public float spawningRechargeTime = 10.0f;

	// Use this for initialization
	void Start () {

        animator = GetComponent<Animator>();

        #region STATE MACHINE SCHEME
        /*
         * Non Active (Player to far away)
         *     |
         *     \/
         *     Active (Player in range)
         * while active cicles between
         * Spawning <-> Active (being only active means the spawner is recharging
         * 
         * if killed (no matter in what state the spawner is currently in) it goes to the state Killed 
         * 
         * */
        #endregion


        //Actions
        Action a_active = () => { active = true; };
        Action a_toofar = () => { active = false; };
        Action a_spawn = () => { animator.SetBool("Spawn", true); };
        Action a_resetTimer = () => { count = 0; };
        Action a_dead = () => { alive = false; };
        
        //Nodes
        StateMachine_Node n_nonActive = new StateMachine_Node("Non Active", null, null, null);
        StateMachine_Node n_active = new StateMachine_Node("Active", null, new List<Action>(new Action[] { a_resetTimer }), null);
        StateMachine_Node n_spawning = new StateMachine_Node("Spawning", null, null, null);
        StateMachine_Node n_dead = new StateMachine_Node("Dead", null, null, null);

        //Transitions
        StateMachine_Transition t_becomingActive = new StateMachine_Transition("nonActive to Active", () => { return Vector3.Distance(transform.position, player.position) <= 50; },
            n_active, new List<Action>(new Action[] { a_active })); //Player is close, spawner activates

        StateMachine_Transition t_activeToNonActive = new StateMachine_Transition("Active to nonActive", () => { return Vector3.Distance(transform.position, player.position) > 50; }, 
            n_nonActive, new List<Action>(new Action[] { a_toofar })); //Player is far away, spawner deactivates

        StateMachine_Transition t_activeToSpawn = new StateMachine_Transition("Active to Spawning", () => { return count >= spawningRechargeTime; }, n_spawning, new List<Action>(new Action[] { a_spawn })); //Spawner is done recharging, it spawns an enemy
        StateMachine_Transition t_spawnToActive = new StateMachine_Transition("Spawning to Active", () => { return !animator.GetBool("Spawn"); }, n_active, null); //Spawner just spawned an enemy, back to recharging
        StateMachine_Transition t_gotKilled = new StateMachine_Transition("Got Killed", () => { return alive == false; }, n_dead, new List<Action>(new Action[] { a_dead })); //Spawner got killed

        n_nonActive.AddTransition(t_becomingActive, t_gotKilled);
        n_active.AddTransition(t_activeToSpawn, t_gotKilled);
        n_spawning.AddTransition(t_spawnToActive, t_gotKilled);

        sM = new StateMachine(n_nonActive);

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        //Runs through the actions
        if (alive)
        {
            List<Action> actions = sM.Run();
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

            //Recharging counter
            if (active)
            {
                if (count < spawningRechargeTime)
                {
                    count += 1 * Time.deltaTime;
                }
            }
        }
    }

    void EndSpawn()
    {
        animator.SetBool("Spawn", false);
    }

    void SpawnEnemy()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(transform.position.x,  transform.position.y + 1, transform.position.z);

    }
}
