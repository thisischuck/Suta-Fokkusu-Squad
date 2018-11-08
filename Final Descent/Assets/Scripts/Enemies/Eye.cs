using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eye : Enemy
{
    protected override void Start()
    {
        base.Start();
        Velocity = Vector3.forward;
        MaxVelocity = 10.0f;
        Attacks = new List<Attack>();
    }

    protected override void Update()
    {
        Velocity = EnemyBehaviours.Wander(transform, Velocity);
        base.Update();
    }
}
