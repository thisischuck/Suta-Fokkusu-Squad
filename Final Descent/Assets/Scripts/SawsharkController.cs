using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawsharkController : Enemy {

	public ParticleSystem p1;
	float shootCooldown;
	float timer;

	protected override void Start()
	{
		base.Start();
		Velocity = Vector3.forward;
		MaxVelocity = 8.0f;
		shootCooldown = 1.8f;

		enemyName = "SAWSHARK";
		Action a_AnimMove = () => { PlayAnimation("MovingShark"); };
		Action a_MoveWithVelocity = () => { transform.position += Velocity * Time.deltaTime * MaxVelocity; };
		Action a_FaceVelocity = () => { transform.forward = Velocity.normalized; };
		Action a_Shoot = () => { if (timer > shootCooldown) { p1.Emit(1); timer = 0.0f; } };
		Action a_ShootCooldown = () => { timer += Time.deltaTime; };

		//Wander
		Action a_Wander = () => { Velocity = EnemyBehaviours.AvoidObstacles(transform, Velocity, ref isThereAnything) * MaxVelocity; if (!isThereAnything)
			{ Velocity = EnemyBehaviours.Wander(transform, Velocity); } };

		//Pursuit
		Action a_Pursuit = () => { Velocity = EnemyBehaviours.AvoidObstacles(transform, Velocity, ref isThereAnything) * MaxVelocity; if (!isThereAnything)
			{ Velocity = EnemyBehaviours.Pursuit(this.transform, Velocity, player.transform, 2.0f); } };

		StateMachine_Node pursuit = new StateMachine_Node("Pursuit", new List<Action>(new Action[] { a_Pursuit, a_MoveWithVelocity, a_FaceVelocity, a_Shoot, a_ShootCooldown }), 
		new List<Action>(new Action[] { a_AnimMove }),null);

		StateMachine_Node wander = new StateMachine_Node("Wander", new List<Action>(new Action[] { a_Wander, a_MoveWithVelocity, a_FaceVelocity }),
		new List<Action>(new Action[] { a_AnimMove }), null);

		StateMachine_Transition WanderToPursuit = new StateMachine_Transition("Wander to Pursuit", () => ToPursuit(), pursuit, null);
		StateMachine_Transition AnyToWander = new StateMachine_Transition("To Wander", () => ToWander(), wander, null);

		wander.AddTransition(WanderToPursuit);
		pursuit.AddTransition(AnyToWander);

		AssignState(wander);
		GameObject.Find("Teeth").SetActive(false);
	}

	protected override void Update()
	{
		base.Update();
	}

	private bool ToWander() { return Vector3.Distance(transform.position, player.transform.position) > 60.0f; }
	private bool ToPursuit() { return Vector3.Distance(transform.position, player.transform.position) < 50.0f; }
	private bool IsAnimationOver(string animation) { return !animController.IsPlaying(animation); }

	//Teeth disc
	private void OnParticleCollision(GameObject other)
	{
		Debug.Log(other.transform.name);
		if (other.transform.name == "AircraftController")
			other.transform.GetComponent<HealthPlayer>().TakeDamage(3f);
	}

	//Ram into player
	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.name == "AircraftController")
		{
			other.transform.GetComponent<HealthPlayer>().TakeDamage(5f);
		}
	}
}
