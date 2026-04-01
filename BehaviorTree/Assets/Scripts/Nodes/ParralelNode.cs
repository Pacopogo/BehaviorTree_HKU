public class ParralelNode : PacoNode
{
    public ParralelNode(string name = "Parralel", int priority = 0) : base(name, priority) { }

    public override Status Process()
    {

        foreach (var child in Childs)
        {
            if (child.Process() == Status.Failure)
                Reset();
        }

        return Status.Success;
    }


}
