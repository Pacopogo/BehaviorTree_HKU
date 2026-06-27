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
    [SerializeField] private GameObject weaponObj;
    public string stateName = "null";
    [SerializeField] private float chaseSpeed = 4;

    private BehaviorTree tree;

    [SerializeField] private GameObject ClosestWeapon;
    [SerializeField] private bool hasWeapon = false;

    [Header("Chase logic")]
    [SerializeField] private float minDist = 3;
    private bool dummySee;
    [SerializeField] private bool SeesPlayer;
    [SerializeField] private bool IsAlerted;

    [SerializeField] private UnityEvent OnAttack;


    private void Awake()
    {
        
        tree = new BehaviorTree("Red Guy");
        UnsafeSequenceNode Base = new UnsafeSequenceNode("Base");

        //Patrolling
        ActionNode DisplayPatrolUI  = new ActionNode("DisplayPatrolUI", new ActionStrat(() => behaviorText.text = "Patroling"));
        ActionNode Patrol           = new ActionNode("Patrol",          new PatrolingStrat(transform, agent, movePoints, walkSpeed));

        //Find weapon/Alert Logic
        ActionNode NoWeapon_Con = new ActionNode("NoWeapon",new ConditionStrat(() => !hasWeapon));
        ActionNode HasWeapon_Con = new ActionNode("HasWeapon",          new ConditionStrat(() => hasWeapon), 10);
        ActionNode TextSearch = new ActionNode("ChaseUI",               new ActionStrat(() => behaviorText.text = "Alert:\nLook for weapon"));
        ActionNode MoveToWeapon = new ActionNode("MoveToWeapon",        new ChaseTarget(agent,() => ClosestWeapon.transform, walkSpeed, 0.05f));

        ActionNode SeePlayer_Con = new ActionNode("SeePlayer", new ConditionStrat(() => { return SeesPlayer; }));
        ActionNode NoSeePlayer_Con = new ActionNode("NoSeePlayer", new ConditionStrat(() => { return !SeesPlayer; }));

        ActionNode Alerted_Con = new ActionNode("Alerted",              new ConditionStrat(() => IsAlerted));
        ActionNode NoAlerted_Con = new ActionNode("NoAlerted_Con", new ConditionStrat(() => !IsAlerted));

        ActionNode TextChase    = new ActionNode("ChaseUI",             new ActionStrat(() => behaviorText.text = "Chase"));
        ActionNode MoveToPlayer = new ActionNode("MoveToPlayer",        new ChaseTarget(agent, () => playerObj.transform, chaseSpeed, 0.5f,() => !SeesPlayer));

        ActionNode TextAttack = new ActionNode("ChaseUI",               new ActionStrat(() => behaviorText.text = "Attack"));
        ActionNode AttackPlayer    = new ActionNode("AttackPlayer",     new ActionStrat(() => OnAttack?.Invoke()));

        ActionNode AlertTrackChase_Con = new ActionNode("AlertTrackChase",  new ConditionStrat(() => IsAlerted || SeesPlayer), 1);

        //Moves the agent to the last know location given
        ActionNode LastLocationUI = new ActionNode("LastLocationUI",            new ActionStrat(() => behaviorText.text = "Searching"));
        ActionNode MoveToLastLocation = new ActionNode("MoveToLastLocation",    new ChaseTarget(agent, () => lastKnownTrans, walkSpeed, 0.1f,() => !IsAlerted));
        ActionNode UnAlert = new ActionNode("UnAlert",                          new ActionStrat(() => IsAlerted = false));

     
        SequenceNode Patrolling = new SequenceNode("Patrolling", 0);
        Patrolling.AddChild(NoAlerted_Con);
        Patrolling.AddChild(NoSeePlayer_Con);
        Patrolling.AddChild(DisplayPatrolUI);
        Patrolling.AddChild(Patrol);

        SequenceNode MoveToLast = new SequenceNode("MoveToLast", 1);
        MoveToLast.AddChild(HasWeapon_Con);
        MoveToLast.AddChild(NoSeePlayer_Con);
        MoveToLast.AddChild(LastLocationUI);
        MoveToLast.AddChild(MoveToLastLocation);
        MoveToLast.AddChild(UnAlert);

        SequenceNode LookForWeapon = new SequenceNode("LookForWeapon", 2);
        LookForWeapon.AddChild(NoWeapon_Con);
        LookForWeapon.AddChild(TextSearch);
        LookForWeapon.AddChild(MoveToWeapon);

        SequenceNode ChasePlayer = new SequenceNode("ChasePlayer", 0);
        ChasePlayer.AddChild(SeePlayer_Con);
        ChasePlayer.AddChild(TextChase);
        ChasePlayer.AddChild(MoveToPlayer);
        ChasePlayer.AddChild(TextAttack);
        ChasePlayer.AddChild(AttackPlayer);

        UnsafeSequenceNode HuntPlayer = new UnsafeSequenceNode("HuntPlayer");
        HuntPlayer.AddChild(LookForWeapon);
        HuntPlayer.AddChild(MoveToLast);
        HuntPlayer.AddChild(ChasePlayer);

        SequenceNode LookForPlayer = new SequenceNode("LookForPlayer", 1);
        LookForPlayer.AddChild(Alerted_Con);
        LookForPlayer.AddChild(HuntPlayer);

        Base.AddChild(LookForPlayer);
        Base.AddChild(Patrolling);

        tree.AddChild(Base);
    }

    private void FixedUpdate()
    {
        CheckForWeapons();
        CheckForPlayer();

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
            hasWeapon = true;
           
            weaponObj.SetActive(hasWeapon);
            weaponTextObj.SetActive(hasWeapon);
            
            other.gameObject.SetActive(false);
        }
    }

    private void CheckForWeapons()
    {
        //Debug.Log(IsAlerted);
        //Debug.Log(SeesPlayer);
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
        float dist = Vector3.Distance(transform.position, playerObj.transform.position);
        

        RaycastHit hit;

        Vector3 dir = playerObj.transform.position - transform.position;

        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, out hit, minDist))
        {
            SeesPlayer = hit.collider.gameObject == playerObj ? true : false;

            Color rayColor = SeesPlayer ? Color.red : Color.yellow;
            Debug.DrawRay(transform.position + Vector3.up * 0.5f, dir, rayColor);
        
            if (SeesPlayer) 
            { 
                IsAlerted = true;
                lastKnownVector = playerObj.transform.position;
                lastKnownTrans.position = lastKnownVector;
            }
        }
       

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minDist);
    }
}
