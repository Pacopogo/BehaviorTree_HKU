using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolingStrat : IStrategy
{
    readonly Transform Body;
    readonly NavMeshAgent Agent;

    readonly List<Transform> Points;
    public int CurrentIndex;

    readonly float MoveSpeed;

    private bool isCalculating;

    public PatrolingStrat(Transform body, NavMeshAgent agent, List<Transform> points, float moveSpeed = 4f)
    {
        Body = body;
        Agent = agent;
        Points = points;
        MoveSpeed = moveSpeed;
    }

    public PacoNode.Status Process()
    {
        if (CurrentIndex == Points.Count)
        {
            Reset();
        }

        Transform target = Points[CurrentIndex];

        Agent.SetDestination(target.position);
        Agent.speed = MoveSpeed;

        if (isCalculating && Agent.remainingDistance < 0.1)
        {
            ++CurrentIndex;
            isCalculating = false;
        }

        if (Agent.pathPending)
        {
            isCalculating = true;
        }

        return PacoNode.Status.Running;
    }

    public void Reset() => CurrentIndex = 0;

}
