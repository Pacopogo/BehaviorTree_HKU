using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PriorityNode : PacoNode
{
    private List<PacoNode> sortedChilds;

    //lazy load, to resort the childern properly again
    private List<PacoNode> SortedChilds => sortedChilds ??= SortChildren();
    protected virtual List<PacoNode> SortChildren() => Childs.OrderByDescending(child => child.priority).ToList();

    public PriorityNode(string name, int priority = 0) : base(name, priority) { }

    public override void Reset()
    {
        base.Reset();
        sortedChilds = null;
    }

    public override Status Process()
    {
        foreach (PacoNode child in SortedChilds)
        {
            switch (child.Process())
            {
                case Status.Success:
                    return Status.Success;
                case Status.Running:
                    return Status.Running;
                default:
                    continue;
            }
        }
        return Status.Failure;
    }
}
