using UnityEngine;

public class SequenceNode : PacoNode
{
    public SequenceNode(string name = "Sequence", int priority = 0) : base(name, priority) { }

    public override Status Process()
    {
        if (currentChild < Childs.Count)
        {
            switch (Childs[currentChild].Process())
            {
                case Status.Running:
                    return Status.Running;
                case Status.Failure:
                    Reset();
                    return Status.Failure;
                default:
                    currentChild++;
                    return currentChild == Childs.Count ? Status.Success : Status.Running;
            }
        }

        Reset();
        return Status.Success;
    }
}
