using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MantaRay : Enemy
{
    public ParticleSystem p1, p2;
    float shootCooldown;
    float timer;

    protected override void Start()
    {
        base.Start();
        Velocity = Vector3.forward;
        MaxVelocity = 8.0f;
        shootCooldown = 1.5f;

        enemyName = "MANTA RAY";
        Action a_AnimMove = () => { PlayAnimation("Moving"); };
        Action a_MoveWithVelocity = () => { transform.position += Velocity * Time.deltaTime * MaxVelocity; };
        Action a_FaceVelocity = () => { transform.forward = Velocity.normalized; };
        Action a_Shoot = () => { if (timer > shootCooldown) { p1.Emit(1); p2.Emit(1); timer = 0.0f; } };
        Action a_ShootCooldown = () => { timer += Time.deltaTime; };

        //Wander
        Action a_Wander = () => { Velocity = EnemyBehaviours.AvoidObstacles(transform, Velocity, ref isThereAnything) * MaxVelocity; if (!isThereAnything) { Velocity = EnemyBehaviours.Wander(transform, Velocity); } };

        //Pursuit
        Action a_Pursuit = () => { Velocity = EnemyBehaviours.AvoidObstacles(transform, Velocity, ref isThereAnything) * MaxVelocity; if (!isThereAnything) { Velocity = EnemyBehaviours.Pursuit(this.transform, Velocity, player.transform, 2.0f); } };

        StateMachine_Node pursuit = new StateMachine_Node("Pursuit",
            new List<Action>(new Action[] { a_Pursuit, a_MoveWithVelocity, a_FaceVelocity, a_Shoot, a_ShootCooldown }),
            new List<Action>(new Action[] { a_AnimMove }),
            null);

        StateMachine_Node wander = new StateMachine_Node("Wander",
            new List<Action>(new Action[] { a_Wander, a_MoveWithVelocity, a_FaceVelocity }),
            new List<Action>(new Action[] { a_AnimMove }),
            null);

        StateMachine_Transition WanderToPursuit = new StateMachine_Transition("Wander to Pursuit", () => ToPursuit(), pursuit, null);
        StateMachine_Transition AnyToWander = new StateMachine_Transition("To Wander", () => ToWander(), wander, null);

        wander.AddTransition(WanderToPursuit);
        pursuit.AddTransition(AnyToWander);

        AssignState(wander);
    }

    protected override void Update()
    {
        base.Update();
    }

    private bool ToWander() { return Vector3.Distance(transform.position, player.transform.position) > 60.0f; }
    private bool ToPursuit() { return Vector3.Distance(transform.position, player.transform.position) < 50.0f; }


}
