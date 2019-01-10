using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Network_Eye : Network_Enemy
{
    private float rayCooldown;
    public float rayTimer;
    private float rayDuration;
    private ParticleSystem ps;

    bool playPs;

    protected override void Start()
    {
        base.Start();
        ps = GetComponent<ParticleSystem>();
        enemyName = "EYE";
        MaxVelocity = 10.0f;
        MaxRotationSpeed = 15.0f;
        Velocity = Vector3.forward * MaxVelocity;
        rayDuration = 10.0f;
        rayCooldown = 5.0f;
        playPs = false;
        rayTimer = 0;

        Action a_ResetRayCooldown = () => { rayCooldown = 0.0f; };
        Action a_CountRayCooldown = () => { rayCooldown += Time.deltaTime; };
        Action a_ShootRay = () => { playPs = true; SendMessage("PlayEyeLaserAudio"); };
        Action a_StopShooting = () => { playPs = false; SendMessage("StopEyeLaserAudio"); };
        Action a_FaceEnemyRay = () =>
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.LookRotation(player.transform.position - transform.position),
                Time.deltaTime * MaxRotationSpeed);
        }; //transform.rotation.SetLookRotation(player.transform.position); };
        Action a_Wander = () => { Velocity = EnemyBehaviours.AvoidObstacles(transform, Velocity, ref isThereAnything) * MaxVelocity; if (!isThereAnything) { Velocity = EnemyBehaviours.Wander(transform, Velocity); } };
        Action a_Move = () => { transform.position += Velocity * Time.deltaTime; };
        Action a_Pursuit = () => { Velocity = EnemyBehaviours.AvoidObstacles(transform, Velocity, ref isThereAnything) * MaxVelocity; if (!isThereAnything) { Velocity = EnemyBehaviours.Pursuit(transform, Velocity, player.transform, 2.0f); } };
        Action a_FaceVelocity = () => { transform.forward = Velocity.normalized; };
        Action a_PlayExplosion = () => { PlayAnimation("Explosion"); };
        Action a_RayTimerUpdate = () => { rayTimer += Time.deltaTime; };
        Action a_RayTimerReset = () => { rayTimer = 0.0f; };

        StateMachine_Node wander = new StateMachine_Node("Wander",
            new List<Action>(new Action[] { a_Wander, a_Move, a_FaceVelocity, a_CountRayCooldown, a_StopShooting }),
            null,
            null);

        StateMachine_Node pursuit = new StateMachine_Node("Pursuit",
            new List<Action>(new Action[] { a_Pursuit, a_Move, a_FaceVelocity, a_CountRayCooldown, a_StopShooting }),
            null,
            null);

        StateMachine_Node attackExplosion = new StateMachine_Node("Attack Explosion",
            new List<Action>(new Action[] { a_PlayExplosion, a_CountRayCooldown, a_StopShooting }),
            new List<Action>(new Action[] { a_PlayExplosion, a_CountRayCooldown }),
            null);

        StateMachine_Node attackRay = new StateMachine_Node("Attack Ray",
            new List<Action>(new Action[] { a_FaceEnemyRay, a_RayTimerUpdate }),
            new List<Action>(new Action[] { a_FaceEnemyRay, a_RayTimerReset, a_ShootRay }),
            new List<Action>(new Action[] { a_StopShooting, a_ResetRayCooldown }));

        StateMachine_Transition WanderToPursuit = new StateMachine_Transition("Wander to Pursuit", () => ToPursuit(), pursuit, null);
        StateMachine_Transition AnyToWander = new StateMachine_Transition("To Wander", () => ToWander(), wander, null);
        StateMachine_Transition PursuitToExplosion = new StateMachine_Transition("Pursuit to Explosion", () => ToExplosion(), attackExplosion, null);
        StateMachine_Transition PursuitToRay = new StateMachine_Transition("Pursuit to Ray", () => ToRay(), attackRay, null);
        StateMachine_Transition ExplosionToPursuit = new StateMachine_Transition("Explosion To Pursuit", () => IsExplosionOver(), pursuit, null);
        StateMachine_Transition RayToPursuit = new StateMachine_Transition("Ray to Pursuit", () => IsRayOver(), pursuit, null);
        StateMachine_Transition RayToWander = new StateMachine_Transition("Ray to Wander", () => IsRayOver() && ToWander(), wander, null);

        wander.AddTransition(WanderToPursuit);
        pursuit.AddTransition(PursuitToExplosion, PursuitToRay, AnyToWander);
        attackExplosion.AddTransition(ExplosionToPursuit);
        attackRay.AddTransition(RayToPursuit);
        attackRay.AddTransition(RayToWander);

        AssignState(wander);
    }

    protected override void Update()
    {
        /*GameObject player = GetClosestPlayer();
        if (Vector3.Distance(transform.position, player.transform.position) < 10.0f)
            MeleeAttack();
        else Velocity = EnemyBehaviours.Wander(transform, Velocity);*/
        //Velocity = EnemyBehaviours.AvoidObstacles(transform, Velocity, ref isThereAnything) * MaxVelocity;

        if (isServer)
        {
            RpcExecuteParticles();
        }
        else
        {
            CmdExecuteParticleSystemAcrossNetwork();
        }
        //StopOrPlayParticleSystem(playPs, ps);

        base.Update();
    }

    [ClientRpc]
    public void RpcExecuteParticles()
    {
        StopOrPlayParticleSystem(playPs, ps);
    }

    [Command]
    public void CmdExecuteParticleSystemAcrossNetwork()
    {
        StopOrPlayParticleSystem(playPs, ps);
    }

    private void MeleeAttack()
    {
        PlayAnimation("Explosion");
    }

    //Functions
    private bool ToPursuit() { return Vector3.Distance(transform.position, player.transform.position) < 50.0f; }

    private bool ToExplosion() { return Vector3.Distance(transform.position, player.transform.position) < 10.0f; }

    private bool ToRay() { return Vector3.Distance(transform.position, player.transform.position) > 10.0f && rayCooldown > 5.0f; }

    private bool ToWander() { return Vector3.Distance(transform.position, player.transform.position) > 60.0f; }

    private bool IsExplosionOver() { return !animController.IsPlaying("Explosion"); }
    //private bool IsExplosionOver() { return !animController.isPlaying; }

    private bool IsRayOver() { return rayTimer > rayDuration; }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("ray is hitting");
        Debug.Log(other.transform.name);
        if (other.transform.name == "AircraftController(Clone)")
        {
            if (isServer)
            {
                other.transform.GetComponent<Network_PlayerHealth>().TakeDamage(1f);
            }
        }

    }

    private void OnCollisionEnter(Collision collision)
    {

        Debug.Log(collision.transform.name);
        if (collision.transform.GetComponent<Network_PlayerHealth>())
        {
            if (isServer)
            {
                collision.transform.GetComponent<Network_PlayerHealth>().TakeDamage(5f);
            }
        }
    }
}
