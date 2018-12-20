using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Butterfly : Enemy
{
    public Material material;
    public Vector3 attackwingsLocation;
    private Color colorEmission;
    private Color colorEmissionMax;
    private float attackCooldowns = 0.0f;
    private int nextAttack = 0;
    private bool spawn;

    private AnimationClip animIdle, animSpawn, animMove, animMelee, animLaser, animWings, animDeath;
    private ParticleSystem[] ps;

    protected override void Start()
    {
        base.Start();
        spawn = false;
        ps = GetComponentsInChildren<ParticleSystem>();
        /*animController["Spawn"].wrapMode = WrapMode.Loop;
        animController["Death"].wrapMode = WrapMode.ClampForever;*/
        animController["Idle"].wrapMode = WrapMode.Loop;
        animController["Mvmnt"].wrapMode = WrapMode.Loop;

        colorEmission = new Color(1.114f, 2.996f, 0.0f, 1.0f);
        material.SetColor("_EmissionColor", colorEmission);
        colorEmissionMax = colorEmission * 100.0f;
        colorEmissionMax.a = 1.0f;
        MaxVelocity = 0.2f;
        MaxRotationSpeed = 50.0f;
        Velocity = Vector3.forward * MaxVelocity;

        Action a_AnimSpawn = () => { PlayAnimation("Spawn"); };
        Action a_AnimIdle = () => { PlayAnimation("Idle"); };
        Action a_AnimMove = () => { PlayAnimation("Mvmnt"); };
        Action a_AnimMelee = () => { PlayAnimation("Melee"); };
        Action a_AnimLaser = () => { PlayAnimation("LaserAntenas"); };
        Action a_AnimWings = () => { PlayAnimation("WingsAttack"); };
        Action a_AnimDeath = () => { PlayAnimation("Death"); };

        Action a_AttackCooldown = () => { attackCooldowns += Time.deltaTime; };
        Action a_AttackCooldownReset = () => { attackCooldowns = 0.0f; nextAttack = 0; };
        Action a_ChooseAttack = () => { if (IsAttackCooldownOver()) nextAttack = UnityEngine.Random.Range(0, 10); };
        Action a_FaceEnemy = () => { transform.rotation = EnemyBehaviours.RotateTowards(transform, player.transform, MaxRotationSpeed); };
        Action a_StopMoving = () => { Velocity = Vector3.zero; };
        Action a_PursuitPlayer = () => { Velocity = EnemyBehaviours.Pursuit(transform, Velocity, player.transform, 1.5f); };
        Action a_MoveSpeedUp = () => { MaxVelocity = 1.0f; };
        Action a_MoveSpeedReset = () => { MaxVelocity = 0.2f; };

        Action a_MoveWithVelocity = () => { transform.position += Velocity * MaxVelocity; };
        Action a_SeekWingsLocation = () => { Velocity = EnemyBehaviours.Seek(transform, Velocity, attackwingsLocation); };
        Action a_WingsCharge = () => { material.SetColor("_EmissionColor", material.GetColor("_EmissionColor") + colorEmission); };
        Action a_WingsShoot = () => { foreach (ParticleSystem p in ps) p.Play(); };
        Action a_WingsStopShooting = () => { foreach (ParticleSystem p in ps) p.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); };
        Action a_WingsDischarge = () => { material.SetColor("_EmissionColor", material.GetColor("_EmissionColor") - colorEmission); };

        StateMachine_Node Spawn = new StateMachine_Node("Spawn",
            new List<Action>(new Action[] { () => { spawn = true; }, a_AnimSpawn }),
            new List<Action>(new Action[] { a_AnimSpawn }),
            null);

        StateMachine_Node Idle = new StateMachine_Node("Idle",
            new List<Action>(new Action[] { a_FaceEnemy, a_AttackCooldown, a_ChooseAttack }),
            new List<Action>(new Action[] { a_AnimIdle/*, a_StopMoving*/ }),
            null);

        StateMachine_Node Move = new StateMachine_Node("Move",
            new List<Action>(new Action[] { a_FaceEnemy, a_AttackCooldown, a_ChooseAttack }),
            new List<Action>(new Action[] { a_AnimMove, a_MoveWithVelocity }),
            null);

        StateMachine_Node Dead = new StateMachine_Node("Dead",
            null,
            new List<Action>(new Action[] { a_AnimDeath }),
            null);

        /**
        * Charge melee Attack nodes
        * -Charge to player
        * -Attack anim
*/

        StateMachine_Node ChargeToPlayer = new StateMachine_Node("Charge to Player",
            new List<Action>(new Action[] { a_PursuitPlayer, a_FaceEnemy, a_MoveWithVelocity }),
            new List<Action>(new Action[] { a_MoveSpeedUp }),
            new List<Action>(new Action[] { a_MoveSpeedReset }));

        //MELEE ANIMATION REFUSES TO WAIT UNTIL IT'S DONE FOR SOME REASON
        StateMachine_Node AttackMelee = new StateMachine_Node("Attack Melee",
            null,
            new List<Action>(new Action[] { a_AnimMelee, }),
            new List<Action>(new Action[] { a_AttackCooldownReset, a_MoveSpeedReset }));

        /**
        * Wings Attack nodes
        * -Move to location
        * -Charge
        * -Attack
        * -Discharge
*/
        StateMachine_Node MoveToWings = new StateMachine_Node("Move to Wings",
            new List<Action>(new Action[] { a_SeekWingsLocation, a_MoveWithVelocity }),
            null,
            null);

        StateMachine_Node WingsCharge = new StateMachine_Node("Wings Charge",
            new List<Action>(new Action[] { a_WingsCharge }),
            new List<Action>(new Action[] { a_AnimMove }),
            null);

        StateMachine_Node AttackWings = new StateMachine_Node("Attack Wings",
            null,
            new List<Action>(new Action[] { a_AnimWings, a_WingsShoot }),
            new List<Action>(new Action[] { a_AttackCooldownReset, a_WingsStopShooting, a_WingsStopShooting, a_WingsStopShooting }));

        StateMachine_Node WingsDischarge = new StateMachine_Node("Wings Discharge",
            new List<Action>(new Action[] { a_WingsDischarge }),
            new List<Action>(new Action[] { a_AnimMove }),
            null);

        /**
        * Antenas Attack nodes
*/

        StateMachine_Node AttackAntenas = new StateMachine_Node("Attack Antenas",
            null,
            new List<Action>(new Action[] { a_AnimLaser }),
            new List<Action>(new Action[] { a_AttackCooldownReset }));


        StateMachine_Transition SpawnToIdle = new StateMachine_Transition("Spawn to Idle", () => { return IsAnimationOver("Spawn") && spawn; }, Idle, null);
        StateMachine_Transition ToMove = new StateMachine_Transition("To Move", () => IsMoving(), Move, null);
        StateMachine_Transition ToMeleeCharge = new StateMachine_Transition("To Melee Charge", () => DoChargeAttack(), ChargeToPlayer, null);
        StateMachine_Transition ToMelee = new StateMachine_Transition("To Melee", () => IsMeleeRange(), AttackMelee, null);
        StateMachine_Transition ToAntenas = new StateMachine_Transition("To Antenas", () => DoAntenasAttack(), AttackAntenas, null);
        StateMachine_Transition ToWingsLocation = new StateMachine_Transition("To Wings Location", () => DoWingsAttack(), MoveToWings, null);
        StateMachine_Transition ToWingsCharge = new StateMachine_Transition("To Wings Charge", () => IsInWingsAttackRange(), WingsCharge, null);
        StateMachine_Transition ToWings = new StateMachine_Transition("To Wings", () => IsWingsChargeDone(), AttackWings, null);
        StateMachine_Transition ToWingsDischarge = new StateMachine_Transition("To Wings Discharge", () => IsAnimationOver("WingsAttack"), WingsDischarge, null);
        StateMachine_Transition OutWingsDischarge = new StateMachine_Transition("Out of Wings Discharge", () => IsWingsDischargeDone(), Idle, new List<Action>(new Action[] { () => { material.SetColor("_EmissionColor", colorEmission); } }));
        StateMachine_Transition ToDead = new StateMachine_Transition("To Dead", () => IsDead(), Dead, null);
        StateMachine_Transition ToIdle = new StateMachine_Transition("Move to Idle", () => !IsMoving(), Idle, null);
        StateMachine_Transition AntenasToMove = new StateMachine_Transition("Antenas to Move", () => IsAnimationOver("LaserAntenas"), Move, null);
        StateMachine_Transition MeleeToMove = new StateMachine_Transition("Melee to Move", () => IsAnimationOver("Melee"), Move, null);

        Spawn.AddTransition(SpawnToIdle);
        Idle.AddTransition(ToMove, ToMeleeCharge, ToAntenas, ToDead, ToWingsLocation);
        Move.AddTransition(ToIdle, ToMeleeCharge, ToAntenas, ToDead, ToWingsLocation);
        AttackAntenas.AddTransition(ToIdle, AntenasToMove);
        AttackMelee.AddTransition(ToIdle, MeleeToMove);

        ChargeToPlayer.AddTransition(ToMelee);

        MoveToWings.AddTransition(ToWingsCharge);
        WingsCharge.AddTransition(ToWings);
        AttackWings.AddTransition(ToWingsDischarge);
        WingsDischarge.AddTransition(OutWingsDischarge);

        stateMachine = new StateMachine(Spawn);
    }

    protected override void Update()
    {
        base.Update();
    }

    private bool IsMoving() { return Velocity != Vector3.zero; }
    private bool IsMeleeRange() { return Vector3.Distance(transform.position, player.transform.position) < 5.0f; }
    private bool IsAntenaRange() { float distance = Vector3.Distance(transform.position, player.transform.position); return distance > 30.0f && distance < 60.0f; }
    private bool IsDead() { return false; } //health < 0
    private bool IsAnimationOver(string animation) { return !animController.IsPlaying(animation); }
    private bool IsAttackCooldownOver() { return attackCooldowns > 2.0f; }
    private bool IsWingsChargeDone() { return material.GetColor("_EmissionColor").g > colorEmissionMax.g; }
    private bool IsWingsDischargeDone() { return material.GetColor("_EmissionColor").g < colorEmission.g; }
    private bool DoWingsAttack() { return nextAttack > 8; }
    private bool DoChargeAttack() { return nextAttack > 5 && nextAttack <= 8; }
    private bool DoAntenasAttack() { return nextAttack > 2 && nextAttack <= 5; }
    private bool IsInWingsAttackRange() { return Vector3.Distance(transform.position, attackwingsLocation) < 15.0f; }
}
