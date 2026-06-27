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
            CurrentIndex = 0;
            
        }

        Transform target = Points[CurrentIndex];
        

        if (!Agent.pathPending && Agent.remainingDistance < 0.1)
        {

            Agent.isStopped = false;
            Agent.speed = MoveSpeed;
            Agent.SetDestination(target.position);

            ++CurrentIndex;
        }

        return PacoNode.Status.Success;
    }

    public void Reset()
    {

    }

}
