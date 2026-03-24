using UnityEngine;
using UnityEngine.AI;

public class MoveAgent : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    public void MoveTarget(Vector3 pos) => agent.SetDestination(pos);
}
