using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eye : Enemy
{
    private float rayCooldown;
    public float rayTimer;
    private float rayDuration;

    protected override void Start()
    {
        base.Start();
        enemyName = "EYE";
        MaxVelocity = 10.0f;
        MaxRotationSpeed = 25.0f;
        Velocity = Vector3.forward * MaxVelocity;
        rayDuration = 3.0f;
        rayCooldown = 0.0f;

        Action a_ResetRayCooldown = () => { rayCooldown = 0.0f; };
        Action a_CountRayCooldown = () => { rayCooldown += Time.deltaTime; };
        Action a_FaceEnemyRay = () =>
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.LookRotation(player.transform.position - transform.position),
                Time.deltaTime * MaxRotationSpeed);
        }; //transform.rotation.SetLookRotation(player.transform.position); };
        Action a_Wander = () => { Velocity = EnemyBehaviours.AvoidObstacles(transform, Velocity, ref isThereAnything) * MaxVelocity; if (!isThereAnything) { Velocity = EnemyBehaviours.Wander(transform, Velocity); } };
        Action a_Move = () => { transform.position += Velocity * Time.deltaTime; };
        Action a_Pursuit = () => { Velocity = EnemyBehaviours.AvoidObstacles(transform, Velocity, ref isThereAnything) * MaxVelocity; if (!isThereAnything) { Velocity = EnemyBehaviours.Pursuit(this.transform, Velocity, player.transform, 2.0f); } };
        Action a_FaceVelocity = () => { transform.forward = Velocity.normalized; };
        Action a_PlayExplosion = () => { PlayAnimation("Explosion"); };
        Action a_RayTimerUpdate = () => { rayTimer += Time.deltaTime; };
        Action a_RayTimerReset = () => { rayTimer = 0.0f; };

        //Debug actions
        Action a_LogRayCooldown = () => { };//Debug.Log(rayCooldown); };
        Action a_LogRayTimer = () => { };// Debug.Log(rayTimer); };

        StateMachine_Node wander = new StateMachine_Node("Wander",
            new List<Action>(new Action[] { a_Wander, a_Move, a_FaceVelocity, a_CountRayCooldown, a_LogRayCooldown }),
            null,
            null);

        StateMachine_Node pursuit = new StateMachine_Node("Pursuit",
            new List<Action>(new Action[] { a_Pursuit, a_Move, a_FaceVelocity, a_CountRayCooldown }),
            null,
            null);

        StateMachine_Node attackExplosion = new StateMachine_Node("Attack Explosion",
            new List<Action>(new Action[] { a_PlayExplosion, a_CountRayCooldown }),
            new List<Action>(new Action[] { a_PlayExplosion, a_CountRayCooldown }),
            null);

        StateMachine_Node attackRay = new StateMachine_Node("Attack Ray",
            new List<Action>(new Action[] { a_FaceEnemyRay, a_RayTimerUpdate, a_LogRayTimer }),
            new List<Action>(new Action[] { a_FaceEnemyRay, a_RayTimerReset }),
            new List<Action>(new Action[] { a_ResetRayCooldown }));

        StateMachine_Transition WanderToPursuit = new StateMachine_Transition("Wander to Pursuit", () => ToPursuit(), pursuit, null);
        StateMachine_Transition AnyToWander = new StateMachine_Transition("To Wander", () => ToWander(), wander, null);
        StateMachine_Transition PursuitToExplosion = new StateMachine_Transition("Pursuit to Explosion", () => ToExplosion(), attackExplosion, null);
        StateMachine_Transition PursuitToRay = new StateMachine_Transition("Pursuit to Ray", () => ToRay(), attackRay, null);
        StateMachine_Transition ExplosionToPursuit = new StateMachine_Transition("Explosion To Pursuit", () => IsExplosionOver(), pursuit, null);
        StateMachine_Transition RayToPursuit = new StateMachine_Transition("Ray to Pursuit", () => IsRayOver(), pursuit, null);

        wander.AddTransition(WanderToPursuit);
        pursuit.AddTransition(PursuitToExplosion, PursuitToRay, AnyToWander);
        attackExplosion.AddTransition(ExplosionToPursuit);
        attackRay.AddTransition(RayToPursuit);

        AssignState(wander);
    }

    protected override void Update()
    {
        /*GameObject player = GetClosestPlayer();
        if (Vector3.Distance(transform.position, player.transform.position) < 10.0f)
            MeleeAttack();
        else Velocity = EnemyBehaviours.Wander(transform, Velocity);*/
        //Velocity = EnemyBehaviours.AvoidObstacles(transform, Velocity, ref isThereAnything) * MaxVelocity;

        base.Update();
    }

    private void MeleeAttack()
    {
        PlayAnimation("Explosion");
    }

    //Functions
    private bool ToPursuit() { return Vector3.Distance(transform.position, player.transform.position) < 50.0f; }

    private bool ToExplosion() { return Vector3.Distance(transform.position, player.transform.position) < 10.0f; }

    private bool ToRay() { return Vector3.Distance(transform.position, player.transform.position) > 10.0f && rayCooldown > 2.0f; }

    private bool ToWander() { return Vector3.Distance(transform.position, player.transform.position) > 60.0f; }

    private bool IsExplosionOver() { return !animController.IsPlaying("Explosion"); }
    //private bool IsExplosionOver() { return !animController.isPlaying; }

    private bool IsRayOver() { return rayTimer > rayDuration; }
}
