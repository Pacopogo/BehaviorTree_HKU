using UnityEngine;

public class ActionNode : PacoNode
{
    IStrategy strategy;

    public ActionNode(string name, IStrategy strategy, int priority = 0) : base(name, priority)
    {
        this.strategy = strategy;
    }

    public override Status Process() => strategy.Process();
    public override void Reset() => strategy.Reset();


}
