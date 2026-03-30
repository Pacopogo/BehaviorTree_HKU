
public class BehaviorTree : PacoNode
{
    public BehaviorTree(string name) : base(name) { }
    
    //this is for displaying the current node at work
    public PacoNode currentNode = new PacoNode();
    public PacoNode childNode = new PacoNode();

    public override Status Process()
    {
        while(currentChild < Childs.Count)
        {
            Status status = Childs[currentChild].Process();

            currentNode = Childs[currentChild];
            childNode = currentNode.Childs[currentNode.currentChild];

            if (status != Status.Success)
            {
                return status;
            }
            ++currentChild;
        }
        return Status.Success;
    }
}
