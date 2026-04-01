using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

    [SerializeField] private GameObject ClosestWeapon;
    [SerializeField] private bool HasWeapon = false;

    [Header("Chase logic")]
    [SerializeField] private float minDist = 3;
    private bool dummySee;
    private bool SeesPlayer
    {
        get
        {
            if (!dummySee)
                ChasePlayer.Reset();

            return dummySee;
        }
        set
        {
            dummySee = value;
        }
    }
    private bool IsAlerted;

    [SerializeField] private UnityEvent OnHitPlayer;


    private SequenceNode ChasePlayer;

    private void Awake()
    {
        tree = new BehaviorTree("Red Guy");
        PriorityNode redGuyDefault = new PriorityNode("RedGuyDefault");

        //Patrolling
        ActionNode Patrol = new ActionNode("Patrol",            new PatrolingStrat(transform, agent, movePoints, walkSpeed));
        ActionNode DisplayPatrolUI = new ActionNode("PatrolUI", new RepeaterStrat(() => behaviorText.text = "Patroling"));

        //Chase logic
        ActionNode Alerted = new ActionNode("Alert",                new ConditionStrat(() => IsAlerted || HasWeapon && SeesPlayer));
        ActionNode MoveToWeapon = new ActionNode("MoveToWeapon",    new ChaseTarget(agent, ClosestWeapon.transform, chaseSpeed, 0.05f));
        ActionNode SeePlayer = new ActionNode("SeePlayer",          new ConditionStrat(() => SeesPlayer));
        ActionNode MoveToPlayer = new ActionNode("MoveToPlayer",    new ChaseTarget(agent, playerObj.transform, chaseSpeed, 0.05f));
        ActionNode HitTarget = new ActionNode("HitPlayer",          new ActionStrat(() => OnHitPlayer?.Invoke()));
        ActionNode DisplayChaseUI = new ActionNode("ChaseUI",       new ActionStrat(() => behaviorText.text = "Chasing"));

        //bunching the patrol actions parralel from each other to display the parralel node 
        ParralelNode parralel = new ParralelNode("Patrolling");
        parralel.AddChild(Patrol);
        parralel.AddChild(DisplayPatrolUI);

        //The chase sequence
        ChasePlayer = new SequenceNode("ChasePlayer", 1);
        ChasePlayer.AddChild(Alerted);
        ChasePlayer.AddChild(MoveToWeapon);
        ChasePlayer.AddChild(SeePlayer);
        ChasePlayer.AddChild(DisplayChaseUI);
        ChasePlayer.AddChild(MoveToPlayer);
        ChasePlayer.AddChild(HitTarget);

        //adding to the priority list
        redGuyDefault.AddChild(parralel);
        redGuyDefault.AddChild(ChasePlayer);

        tree.AddChild(redGuyDefault);
    }

    private void Update()
    {
        CheckSphereAgent();

        if (!playerObj.activeSelf)
        {
            SeesPlayer = false;
            return;
        }

        tree.Process();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<WeaponTag>())
        {
            HasWeapon = true;
            other.gameObject.SetActive(false);
        }
    }

    private void CheckSphereAgent()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, minDist);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.GetComponent<WeaponTag>())
            {
                ClosestWeapon = collider.gameObject;
            }

            if (collider.gameObject.GetComponent<MoveAgent>())
            {
                RaycastHit hit;
                Vector3 dir = collider.gameObject.transform.position - transform.position;
                if (Physics.Raycast(transform.position, dir, out hit, Mathf.Infinity))
                {
                    SeesPlayer = hit.collider.gameObject.GetComponent<MoveAgent>() != null ? true : false;


                    Color rayColor = SeesPlayer ? Color.red : Color.green;
                    Debug.DrawRay(transform.position, dir, rayColor);

                    if(SeesPlayer)
                        IsAlerted = true;
                }
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minDist);
    }
}
