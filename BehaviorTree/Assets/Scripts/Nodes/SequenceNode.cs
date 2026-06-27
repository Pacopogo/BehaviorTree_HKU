using UnityEngine;

public class SequenceNode : PacoNode
{
    public SequenceNode(string name = "Sequence", int priority = 0) : base(name, priority) { }

    public override Status Process()
    {
        Debug.Log(NodeName);
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

/// <summary>
/// This is to handle sequences that aren't allowed to Fail or Success or reset
/// </summary>
public class UnsafeSequenceNode : PacoNode
{
    public UnsafeSequenceNode(string name = "UnsafeSequenceNode", int priority = 0) : base(name, priority) { }

    public override Status Process()
    {
        Debug.Log(NodeName);
        if (currentChild < Childs.Count)
        {
            //switch (Childs[currentChild].Process())
            //{
            //    case Status.Running:
            //        return Status.Running;
            //    default:
            //        currentChild++;
            //        return currentChild == Childs.Count ? Status.Success : Status.Running;
            //}

            if(Childs[currentChild].Process() == Status.Running)
                return Status.Running;
            
            ++currentChild;
            return Status.Running;
        }

        Reset();
        return Status.Success;
    }
}