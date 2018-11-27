using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine_Node
{
    public string name;
    public List<StateMachine_Transition> transitions;
    public List<Action> actions;
    public List<Action> entryActions;
    public List<Action> leaveActions;

    public StateMachine_Node(string name, List<Action> actions,
                    List<Action> entryActions, List<Action> leaveAction)
    {
        this.name = name;
        this.actions = actions;
        this.entryActions = entryActions;
        this.leaveActions = leaveAction;
        this.transitions = new List<StateMachine_Transition>();
    }

    public void AddTransition(params StateMachine_Transition[] T)
    {
        transitions.AddRange(T);
    }

    public override string ToString()
    {
        return name;
    }
}
