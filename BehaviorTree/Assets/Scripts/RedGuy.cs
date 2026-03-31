using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

/// <summary>
/// This class is the Antagonistic enemy, 
/// that is going to look and chase the player once it has a weapon otherwise it will patrol.
/// 
/// [Behaviors: Patrol, Look for Weapon and Chase player]
/// </summary>
public class RedGuy : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject playerObj;
    [SerializeField] private NavMeshAgent agent;

    [Header("Patrol Settings")]
    [SerializeField] private float walkSpeed = 3;
    [SerializeField] private List<Transform> movePoints;

    [Header("Behavior logic")]
    [SerializeField] private TMP_Text behaviorText;
    public string stateName = "null";
    [SerializeField] private float chaseSpeed = 4;

    private BehaviorTree tree;

    [SerializeField] private float minDist = 3;
    private bool SeesPlayer = false;

    [SerializeField] private UnityEvent OnHitPlayer;

    private void Awake()
    {
        tree = new BehaviorTree("Red Guy");

        ActionNode Patrol = new ActionNode("Patrol", new PatrolingStrat(transform, agent, movePoints, walkSpeed));

        //See & Chase player Logic
        ActionNode SeePlayer    = new ActionNode("SeePlayer", new ConditionStrat(() => SeesPlayer));
        ActionNode MoveToPlayer = new ActionNode("MoveToPlayer", new ChaseTarget(agent, playerObj.transform, chaseSpeed));
        ActionNode HitTarget    = new ActionNode("HitPlayer", new ActionStrat(() => OnHitPlayer?.Invoke()));

        SequenceNode ChasePlayer = new SequenceNode("ChasePlayer", 1);
        ChasePlayer.AddChild(SeePlayer);
        ChasePlayer.AddChild(MoveToPlayer);
        ChasePlayer.AddChild(HitTarget);

        PriorityNode redGuyDefault = new PriorityNode("RedGuyDefault");
        redGuyDefault.AddChild(Patrol);
        redGuyDefault.AddChild(ChasePlayer);

        tree.AddChild(redGuyDefault);
    }

    private void Update()
    {
        if (tree.currentChild < tree.Childs.Count)
        {

            behaviorText.text =
                tree.currentNode.NodeName
                + "\n" +
                tree.childNode.NodeName;
        }

        LookForPlayer();

        tree.Process();
    }

    private void LookForPlayer()
    {
        float dist = Vector3.Distance(transform.position, playerObj.transform.position);

        SeesPlayer = dist < minDist ? true : false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minDist);
    }
}
