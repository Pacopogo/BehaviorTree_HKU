using System.Collections.Generic;

public class PacoNode
{
    public enum Status
    {
        Success,
        Failure,
        Running
    }

    public string NodeName;
    public int priority;

    public List<PacoNode> Childs = new List<PacoNode>();
    public int currentChild;

    public PacoNode(string name = "Node", int priority = 0)
    {
        NodeName = name;
        this.priority = priority;
    }

    public void AddChild(PacoNode node) => Childs.Add(node);

    //Updates the current index'd child logic
    public virtual Status Process() => Childs[currentChild].Process();

    //Reset all children and revert to the first index
    public virtual void Reset()
    {
        currentChild = 0;
        foreach (PacoNode node in Childs)
        {
            node.Reset();
        }
    }
}
