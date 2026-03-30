using System;
using UnityEngine;

public class ActionStrat : IStrategy
{
    readonly Action action;

    public ActionStrat(Action action)
    {
        this.action = action;
    }

    public PacoNode.Status Process()
    {
        action();
        return PacoNode.Status.Success;
    }
}
