using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class AgentAnimator : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void FixedUpdate()
    {
        animator.SetFloat("Moving", agent.velocity.magnitude);
    }
}
