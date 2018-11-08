using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine_Transition{
    public string name;
    public Func<bool> Condition;
    public StateMachine_Node target;
    public List<Action> actions;

    public StateMachine_Transition(string name, Func<bool> condition, StateMachine_Node target, List<Action> actions)
    {
        this.name = name;
        Condition = condition;
        this.target = target;
        this.actions = actions;
    }

    public override string ToString()
    {
        return name;
    }
}
