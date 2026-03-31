using UnityEngine;
using UnityEngine.AI;

public class ChaseTarget : IStrategy
{
    private NavMeshAgent agent;
    private Transform target;
    private float chaseSpeed;
    private bool isCalculating;

    public ChaseTarget(NavMeshAgent agent, Transform target, float chaseSpeed = 1)
    {
        this.agent = agent;
        this.target = target;
        this.chaseSpeed = chaseSpeed;
    }

    public PacoNode.Status Process()
    {
        agent.speed = chaseSpeed;
        agent.SetDestination(target.position);

        if (agent.remainingDistance < 0.1)
        {
            return PacoNode.Status.Success;
        }

        return PacoNode.Status.Running;
    }
}
