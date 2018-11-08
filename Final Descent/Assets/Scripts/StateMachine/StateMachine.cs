using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine {
    public StateMachine_Node currentNode;

    public StateMachine(StateMachine_Node currentNode)
    {
        this.currentNode = currentNode;
    }

    public List<Action> Run()
    {
        List<Action> a = new List<Action>();
        foreach (var t in currentNode.transitions)
        {
            if (t.Condition())
            {
                currentNode = t.target;
                if (currentNode.leaveActions != null)
                    a.AddRange(currentNode.leaveActions);
                if (t.actions != null)
                    a.AddRange(t.actions);
                if (t.target.entryActions != null)
                    a.AddRange(t.target.entryActions);
                return a;
            }
        }
        return currentNode.actions;
    }
}
