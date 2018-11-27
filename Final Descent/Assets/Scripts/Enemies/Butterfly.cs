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

        StateMachine_Node Idle = new StateMachine_Node("Idle", null, null, null);
        StateMachine_Node Move = new StateMachine_Node("Move", null, null, null);
        StateMachine_Node Attack = new StateMachine_Node("Attack", null, null, null);
    }

    protected override void Update()
    {
        base.Update();
    }
}
