using UnityEngine;
using UnityEngine.AI;

public class ChaseTarget : IStrategy
{
    private NavMeshAgent agent;
    private Transform target;
    private float chaseSpeed;
    private float minimalDistance;

    public ChaseTarget(NavMeshAgent agent, Transform target, float chaseSpeed = 1, float minimalDistance = 0.1f)
    {
        this.agent              = agent;
        this.target             = target;
        this.chaseSpeed         = chaseSpeed;
        this.minimalDistance    = minimalDistance;
    }

    public PacoNode.Status Process()
    {
        if(!target.gameObject.activeSelf)
            return PacoNode.Status.Success;

        agent.speed = chaseSpeed;
        agent.SetDestination(target.position);

        float dist = Vector3.Distance(agent.transform.position, target.position);

        Debug.Log(dist + "||" + minimalDistance);
        if (dist <= minimalDistance)
        {
            return PacoNode.Status.Success;
        }

        return PacoNode.Status.Running;
    }
}
