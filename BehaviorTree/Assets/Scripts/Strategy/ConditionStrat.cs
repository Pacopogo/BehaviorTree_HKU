using System;
using UnityEngine;

public class ConditionStrat : IStrategy 
{
    readonly Func<bool> predicate;

    public ConditionStrat(Func<bool> predicate)
    {
        this.predicate = predicate;
    }

    public PacoNode.Status Process() => predicate() ? PacoNode.Status.Success : PacoNode.Status.Failure;    
}
