using System;
using UnityEngine;

public class RepeaterStrat : IStrategy
{
    readonly Action action;

    public RepeaterStrat(Action action)
    {
        this.action = action;
    }

    public PacoNode.Status Process()
    {
        action();
        return PacoNode.Status.Running;
    }
}
