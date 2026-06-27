using System;
using UnityEngine;
using UnityEngine.AI;

public class ChaseTarget : IStrategy
{
    private NavMeshAgent agent;
    private Func<Transform> target;
    private float chaseSpeed;
    private float minimalDistance;
    private Func<bool> failurePredicate;

    public ChaseTarget(NavMeshAgent agent, Func<Transform> target, float chaseSpeed = 1, float minimalDistance = 0.1f, Func<bool> failurePredicate = null)
    {
        this.agent              = agent;
        this.target             = target;
        this.chaseSpeed         = chaseSpeed;
        this.minimalDistance    = minimalDistance;
        this.failurePredicate   = failurePredicate;
    }

    public PacoNode.Status Process()
    {
        if(target == null)
            return PacoNode.Status.Failure;

        if (!target().gameObject.activeSelf)
            return PacoNode.Status.Success;

        agent.isStopped = false;

        agent.speed = chaseSpeed;
        agent.SetDestination(target().position);

        float dist = Vector3.Distance(agent.transform.position, target().position);

        //Debug.Log(dist + "||" + minimalDistance);
        if (dist <= minimalDistance)
        {
            agent.isStopped = true;
            return PacoNode.Status.Success;
        }

        if (failurePredicate != null && !failurePredicate())
        {
            return PacoNode.Status.Failure;
        }

        return PacoNode.Status.Running;
    }
}
