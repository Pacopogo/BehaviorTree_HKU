using UnityEngine;
using System;
using System.Diagnostics;

public class BehaviorTree : PacoNode
{
    public BehaviorTree(string name) : base(name) { }

    //this is for displaying the current node at work
    public PacoNode currentNode = new PacoNode();
    public PacoNode childNode = new PacoNode();

    public override Status Process()
    {
        Childs[currentChild].Process();


        currentNode = Childs[currentChild];
        if(currentNode.currentChild < currentNode.Childs.Count)
            childNode = currentNode.Childs[currentNode.currentChild];

        int nextChild = currentChild + 1;

        if (nextChild > Childs.Count)
            Reset();

        return Status.Running;
    }

    public override void Reset()
    {
        UnityEngine.Debug.Log("Tree Reset");
        currentChild = 0;
        base.Reset();
    }

}
