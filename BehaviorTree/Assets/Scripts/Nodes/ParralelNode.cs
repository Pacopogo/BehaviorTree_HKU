using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class ParralelNode : PacoNode
{
    public ParralelNode(string name = "Parralel", int priority = 0) : base(name, priority) { }

    public override Status Process()
    {
        if (currentChild < Childs.Count)
        {
            foreach (var child in Childs)
            {
                switch (child.Process())
                {
                    case Status.Success:
                        break;
                    case Status.Failure:
                        break;
                    case Status.Running:
                        break;
                    default:
                        break;
                }
            }
        }

        return Status.Success;
    }


}
