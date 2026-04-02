using UnityEngine;

public class SelectorNode : PacoNode
{
    public SelectorNode(string name, int p, int priority = 0) : base(name, priority) { }

    public override Status Process()
    {
        if (currentChild < Childs.Count)
        {
            switch (Childs[currentChild].Process())
            {
                case Status.Success:
                    //Reset();
                    return Status.Success;

                case Status.Running:
                    return Status.Running;

                default:
                    currentChild++;
                    return Status.Running;
            }
        }

        Reset();
        return Status.Failure;
    }
}
