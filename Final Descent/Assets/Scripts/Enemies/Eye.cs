using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eye : Enemy
{
    bool isAttacking;

    protected override void Start()
    {
        base.Start();
        Velocity = Vector3.forward;
        MaxVelocity = 10.0f;
        isAttacking = false;
    }


    /*
    This is not needed we just need to keep the names consistent in the attack class and in the animation clip 
    */
    private void FillAtacks()
    {
        /* 
        foreach (AnimationClip tmp in animController)
        {
            foreach (Attack atk in listAttacks)
            {
                if (tmp.name.Equals(atk.Name))
                {

                }
            }
        }
        */
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
        /*
        How you play it
        You can just create a main method to call all animations.
        I'm gonna do that.
        */
        PlayAnimation("Explosion");
        /* There you go clean */
    }
}
