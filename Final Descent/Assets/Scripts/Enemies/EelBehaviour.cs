using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EelBehaviour : Enemy
{
    private BaseStats baseStats;
	private EelAttacks eelAttacks;

	public bool alive;
/*    public bool isTornado;
    public bool isShockWave;
    public bool isHighSpeed;
    public bool isExploding;
    public bool isCalling;
    public bool isCharging;
    public bool isBiting;*/

    public float eelHealth;
	public float eelMaxHealth;

    protected override void Start()
    {
		base.Start();
		alive = true;
		baseStats = GetComponent<BaseStats>();
		eelAttacks = GetComponent<EelAttacks>();
		baseStats.GenerateVariables(1000, 1000);
		eelHealth = baseStats.health;
		eelMaxHealth = eelHealth;

        //Actions
        Action a_active = () => { eelAttacks.Start(); };
        Action a_onEntryTornado = () => { eelAttacks.OnEntryTornado(); };
		Action a_tornado = () => { eelAttacks.Tornado(); };
		Action a_onEntryHighSpeed = () => { eelAttacks.OnEntryHighSpeed(); };
		Action a_highSpeed = () => {  eelAttacks.HighSpeed(); };
		Action a_onEntryExplode = () => { eelAttacks.OnEntryExplode(); };
		Action a_explode = () => { eelAttacks.Explode(); };
		Action a_onEntryShockWave = () => { eelAttacks.OnEntryShockWave(); };
		Action a_shockWave = () => { eelAttacks.ShockWave(); };
		Action a_onEntryCalling = () => { eelAttacks.OnEntryCalling(); };
		Action a_call = () => { eelAttacks.Calling(); };
		Action a_onEntryCharge = () => { eelAttacks.OnEntryCharge(); };
		Action a_charge = () => { eelAttacks.Charge(); };
		Action a_onEntryBite = () => { eelAttacks.OnEntryBite(); };
		Action a_bite = () => { eelAttacks.Bite(); };
        Action a_dead = () => { alive = false; };

        //Nodes
        StateMachine_Node n_active = new StateMachine_Node("Active", new List<Action> { a_active },null, null);
        StateMachine_Node n_tornado = new StateMachine_Node("Tornado", new List<Action> { a_tornado }, new List<Action> { a_onEntryTornado }, null);
        StateMachine_Node n_highSpeed = new StateMachine_Node("HighSpeed", new List<Action> { a_highSpeed }, new List<Action> { a_onEntryHighSpeed }, null);
        StateMachine_Node n_shockWave = new StateMachine_Node("ShockWave", new List<Action> { a_shockWave }, null, null);
        StateMachine_Node n_explode = new StateMachine_Node("Exploding", new List<Action> { a_explode }, null, null);
        StateMachine_Node n_call = new StateMachine_Node("Calling", new List<Action> { a_call }, null, null);
        StateMachine_Node n_Charge = new StateMachine_Node("Charging", new List<Action> { a_charge }, new List<Action> { a_onEntryCharge }, null);
        StateMachine_Node n_Bite = new StateMachine_Node("Biting", new List<Action> { a_bite }, new List<Action> { a_onEntryBite }, null);
        StateMachine_Node n_dead = new StateMachine_Node("Dead", null, null, null); //ainda sem açao

		//Transitions
		StateMachine_Transition t_ativeToHighSpeed = new StateMachine_Transition("Active To HighSpeed", () => { return alive == true; }, n_highSpeed,
			null);
		StateMachine_Transition t_highSpeedToTornado = new StateMachine_Transition("HighSpeed To Tornado", () => { return eelAttacks.isHighSpeed == false && 
			eelAttacks.MedPlayersPositions(transform.position) > 50f; }, n_tornado, null); //Player is far away, tornado is activated
        StateMachine_Transition t_highSpeedToShockwave = new StateMachine_Transition("HighSpeed To ShockWave", () => { return eelAttacks.isHighSpeed == false &&
			eelAttacks.MedPlayersPositions(transform.position) < 50; }, n_shockWave, null);
        StateMachine_Transition t_tornadoToExplode = new StateMachine_Transition("Tornado To Explode", () => { return eelAttacks.isTornado == false && eelAttacks.goToExplode == true; }, n_explode,
            null);
        StateMachine_Transition t_explodeToHighSpeed = new StateMachine_Transition("Explode To HighSpeed", () => { return eelAttacks.isExploding == false && eelAttacks.RandNum() == 0; }, n_highSpeed,
            null);
        StateMachine_Transition t_explodeToCharge = new StateMachine_Transition("Explode To Charge", () => { return eelAttacks.isExploding == false && eelAttacks.RandNum() == 1; }, n_Charge,
            null);
        StateMachine_Transition t_shockWaveToCharge = new StateMachine_Transition("ShockWave To Charge", () => { return eelAttacks.isShockWave == false && eelAttacks.RandNum() == 1; }, n_Charge,
            null);
        StateMachine_Transition t_shockWaveToHighSpeed = new StateMachine_Transition("ShockWave To HighSpeed", () => { return eelAttacks.isShockWave == false && eelAttacks.RandNum() == 0; }, n_highSpeed,
            null);
        StateMachine_Transition t_callingToCharge = new StateMachine_Transition(" Calling To Charge", () => { return eelAttacks.isCalling == false && eelAttacks.RandNum() == 1; }, n_Charge,
            null);
        StateMachine_Transition t_callingToTornado = new StateMachine_Transition("Calling To Tornado", () => { return eelAttacks.isCalling == false && eelAttacks.RandNum() == 0; }, n_tornado,
            null);
        StateMachine_Transition t_biteToHighSpeed = new StateMachine_Transition("Bite To HighSpeed", () => { return GoToHighSpeed() == true; }, n_highSpeed,
            null); //CODICAO: Se a Eel receber mais de 5% da vida desde q começou a atacar(charge) volta para o HighSpeed node
		StateMachine_Transition t_biteToCharge = new StateMachine_Transition("Bite To Charge", () => { return GoToHighSpeed() == false; }, n_Charge,
			null); //CODICAO: Se a Eel receber menos de 5% da vida desde q começou a atacar(charge) volta para o Charge node
		StateMachine_Transition t_anyToBite = new StateMachine_Transition("Bite", () => { return eelAttacks.isBiting == true; }, n_Bite,
			null); //CODICAO: Se alguem estiver em range de morder irá transicionar para Bite node
		StateMachine_Transition t_anyCalling = new StateMachine_Transition("Calling", () => { return activateCall() == true; }, n_call,
			null);
		StateMachine_Transition t_killed = new StateMachine_Transition("Killed", () => { return alive == false; }, n_dead, null); //Eel gets killed

		//Active
		n_active.AddTransition(t_ativeToHighSpeed, t_killed, t_anyCalling, t_anyToBite);

		//HighSpeed
		n_highSpeed.AddTransition(t_highSpeedToTornado, t_highSpeedToShockwave,t_killed, t_anyCalling, t_anyToBite);

		//Tornado
		n_tornado.AddTransition(t_tornadoToExplode, t_killed, t_anyCalling, t_anyToBite);

		//Explode
		n_explode.AddTransition(t_explodeToCharge, t_killed, t_explodeToHighSpeed, t_anyCalling, t_anyToBite);

		//ShockWave
		n_shockWave.AddTransition(t_shockWaveToCharge, t_shockWaveToHighSpeed, t_killed, t_anyCalling, t_anyToBite);

		//Bite
		n_Bite.AddTransition(t_biteToCharge, t_biteToHighSpeed, t_killed, t_anyCalling);

		//Call
		n_call.AddTransition(t_callingToCharge, t_callingToTornado, t_killed, t_anyToBite);

		AssignState(n_active);
    }

    protected override void Update()
    {
		base.Update();

		if (eelHealth <= 0)
			alive = false;
		else alive = true;
    }

	public bool activateCall()
	{
		if (eelHealth == eelMaxHealth / 0.5)//vida a 50%
			return true;
		else if (eelHealth == eelMaxHealth * 0.75)//vida a 75%
			return true;
		else if (eelHealth == eelMaxHealth * 0.25)//vida a 25%
			return true;
		else return false;
	}

	public bool GoToHighSpeed()
	{
		if (eelAttacks.currentHealth - eelHealth >= eelHealth * 0.05)
		{
			//eelAttacks.isHighSpeed = true;
			eelAttacks.sameCharge = false;
			return true;
		}
		else
		{
			//eelAttacks.isCharging = true;
			eelAttacks.sameCharge = true;
			return false;
		}
	}
}
