using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

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

    private BehaviorTree tree;

    [SerializeField] private float minDist = 3;
    private bool SeesPlayer = false;

    private void Awake()
    {
        tree = new BehaviorTree("Red Guy");

        ActionNode Patrol = new ActionNode("Patrol", new PatrolingStrat(transform, agent, movePoints, walkSpeed));


        //See & Chase player Logic
        ActionNode SeePlayer = new ActionNode("SeePlayer", new ConditionStrat(() => SeesPlayer));
        ActionNode MoveToPlayer = new ActionNode("MoveToPlayer", new ActionStrat(() => agent.SetDestination(playerObj.transform.position)));

        SequenceNode ChasePlayer = new SequenceNode("ChasePlayer", 1);
        ChasePlayer.AddChild(SeePlayer);
        ChasePlayer.AddChild(MoveToPlayer);

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
