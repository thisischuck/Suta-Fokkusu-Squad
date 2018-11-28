using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Butterfly : Enemy
{
    protected override void Start()
    {
        base.Start();
        MaxVelocity = 20.0f;
        Velocity = Vector3.forward * MaxVelocity;

        StateMachine_Node Spawn = new StateMachine_Node("Spawn", null, null, null);
        StateMachine_Node Idle = new StateMachine_Node("Idle", null, null, null);
        StateMachine_Node Move = new StateMachine_Node("Move", null, null, null);
        StateMachine_Node AttackMelee = new StateMachine_Node("Attack Melee", null, null, null);
        StateMachine_Node AttackWings = new StateMachine_Node("Attack Wings", null, null, null);
        StateMachine_Node AttackAntenas = new StateMachine_Node("Attack Antenas", null, null, null);
        StateMachine_Node Dead = new StateMachine_Node("Dead", null, null, null);

        StateMachine_Transition SpawnToIdle = new StateMachine_Transition("Spawn to Idle", () => IsSpawnDone(), Idle, null);
        StateMachine_Transition IdleToMove = new StateMachine_Transition("Idle to Move", () => IsMoving(), Move, null);
        StateMachine_Transition MoveToMelee = new StateMachine_Transition("Move to Melee", () => IsMeleeRange(), AttackMelee, null);
        StateMachine_Transition MoveToAntenas = new StateMachine_Transition("Move to Antenas", () => IsAntenaRange(), AttackAntenas, null);
        StateMachine_Transition MoveToDead = new StateMachine_Transition("Move to Dead", () => IsDead(), Dead, null);
    }

    protected override void Update()
    {
        base.Update();
    }

    private bool IsSpawnDone() { return !animController.IsPlaying("Spawn"); }
    private bool IsMoving() { return Velocity != Vector3.zero; }
    private bool IsMeleeRange() { return Vector3.Distance(transform.position, player.transform.position) < 30.0f; }
    private bool IsAntenaRange() { return Vector3.Distance(transform.position, player.transform.position) > 30.0f; }
    private bool IsDead() { return false; } //health < 0 
}
