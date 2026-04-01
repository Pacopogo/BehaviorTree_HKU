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
    [SerializeField] private Transform lastKnownTrans;
    private Vector3 lastKnownVector;

    [Header("Patrol Settings")]
    [SerializeField] private float walkSpeed = 3;
    [SerializeField] private List<Transform> movePoints;

    [Header("Behavior logic")]
    [SerializeField] private TMP_Text behaviorText;
    [SerializeField] private GameObject weaponTextObj;
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
        ActionNode Patrol           = new ActionNode("Patrol", new PatrolingStrat(transform, agent, movePoints, walkSpeed));
        ActionNode DisplayPatrolUI  = new ActionNode("PatrolUI", new RepeaterStrat(() => behaviorText.text = "Patroling"));

        //Find weapon/Alert Logic
        ActionNode Alerted = new ActionNode("Alert",                new ConditionStrat(() => IsAlerted && !HasWeapon));
        ActionNode TextSearch = new ActionNode("ChaseUI",           new ActionStrat(() => behaviorText.text = "Alert:\nLook for weapon"));
        ActionNode MoveToWeapon = new ActionNode("MoveToWeapon",    new ChaseTarget(agent, ClosestWeapon.transform, walkSpeed, 0.05f));
        ActionNode LastLocationUI = new ActionNode("LastLocationUI", new ActionStrat(() => behaviorText.text = "Search Last known location"));
        

        ActionNode SeePlayer    = new ActionNode("SeePlayer",          new ConditionStrat(() => SeesPlayer));
        ActionNode TextChase    = new ActionNode("ChaseUI",            new ActionStrat(() => behaviorText.text = "Chase"));
        ActionNode MoveToPlayer = new ActionNode("MoveToPlayer",    new ChaseTarget(agent, playerObj.transform, chaseSpeed, 0.05f));
        ActionNode HitTarget    = new ActionNode("HitPlayer",          new ActionStrat(() => OnHitPlayer?.Invoke()));


        //Moves the agent to the last know location given
        ActionNode MoveToLastLocation = new ActionNode("MoveToLastLocation", new ChaseTarget(agent, lastKnownTrans, walkSpeed, 0.05f));

        //bunching the patrol actions parralel from each other to display the parralel node 
        ParralelNode parralel = new ParralelNode("Patrolling");
        parralel.AddChild(Patrol);
        parralel.AddChild(DisplayPatrolUI);


        ChasePlayer = new SequenceNode("Chase Sequence",1);
        ChasePlayer.AddChild(SeePlayer);
        ChasePlayer.AddChild(TextChase);
        ChasePlayer.AddChild(MoveToPlayer);
        ChasePlayer.AddChild(HitTarget);

        SequenceNode TrackPlayer = new SequenceNode("Track Player");
        //TrackPlayer.AddChild(SeePlayer);
        TrackPlayer.AddChild(LastLocationUI);
        TrackPlayer.AddChild(MoveToLastLocation);

        //Chase logic
        PriorityNode ChaseOrTrack = new PriorityNode("Chase Or Track");
        ChaseOrTrack.AddChild(ChasePlayer);
        ChaseOrTrack.AddChild(TrackPlayer);

        SequenceNode AlertSequence = new SequenceNode("AlertSequence", 1); //priority 1 to be higher the Chasing
        AlertSequence.AddChild(Alerted);
        AlertSequence.AddChild(TextSearch);
        AlertSequence.AddChild(MoveToWeapon);
        AlertSequence.AddChild(LastLocationUI);
        AlertSequence.AddChild(MoveToLastLocation);

        PriorityNode AlertAndChase = new PriorityNode("AlertAndChase", 1); //priority 1 to be higher the patroling
        AlertAndChase.AddChild(ChaseOrTrack);
        AlertAndChase.AddChild(AlertSequence);


        //adding to the priority list
        redGuyDefault.AddChild(AlertAndChase);
        redGuyDefault.AddChild(parralel);

        tree.AddChild(redGuyDefault);
    }


    private void FixedUpdate()
    {
        CheckForPlayer();
        CheckForWeapons();

        weaponTextObj.SetActive(HasWeapon);

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

    private void CheckForWeapons()
    {
        Debug.Log(IsAlerted);
        Debug.Log(SeesPlayer);
        Collider[] colliders = Physics.OverlapSphere(transform.position, minDist);

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.GetComponent<WeaponTag>())
            {
                ClosestWeapon = collider.gameObject;
                return;
            }  
        }

    }

    private void CheckForPlayer()
    {
        if (Vector3.Distance(transform.position, playerObj.transform.position) > minDist)
        {
            SeesPlayer = false;
            return;
        }

        RaycastHit hit;
        Vector3 dir = playerObj.transform.position - transform.position;
        if (Physics.Raycast(transform.position, dir, out hit, minDist))
        {
            SeesPlayer = hit.collider.gameObject == playerObj ? true : false;

            Color rayColor = SeesPlayer ? Color.red : Color.yellow;
            Debug.DrawRay(transform.position, dir, rayColor);

            if (SeesPlayer)
                IsAlerted = true;

            lastKnownVector = playerObj.transform.position;
            lastKnownTrans.position = lastKnownVector;
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minDist);
    }
}
