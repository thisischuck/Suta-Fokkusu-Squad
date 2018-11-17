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
    }

    protected override void Update()
    {
        GameObject player = GetClosestPlayer();
        if (Vector3.Distance(transform.position, player.transform.position) < 10.0f)
            MeleeAttack();
        Velocity = EnemyBehaviours.Wander(transform, Velocity);
        base.Update();
    }

    private void MeleeAttack()
    {
        Attacks[0].Animation.Play();
    }
}
