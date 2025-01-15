using UnityEngine;
//Note that we added this library to be able to use the navmesh components
using UnityEngine.AI;

//This script must be in the same game object as a navmeshagent
[RequireComponent(typeof(NavMeshAgent))]
public class FSM_EnemyScript : MonoBehaviour
{
    //The script in charge of controlling the navigation of this AI agent
    public NavMeshAgent navMeshAgent;

    public StateBaseClass currentState;
    public StateBaseClass idleState;
    public StateBaseClass patrolState;
    //public StateBaseClass chaseState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        //Get the navmeshagent in this game object
        navMeshAgent = GetComponent<NavMeshAgent>();

        idleState = new FSM_IdleState(this);
        patrolState = new FSM_PatrolState(this);

        //Entry state gets assigned
        currentState = idleState;
        currentState.OnEnterState();
    }

    // Update is called once per frame
    private void Update()
    {
        //Every frame we call the corresponding function on the current state
        currentState.OnEveryFrame();
    }

    private void FixedUpdate()
    {
        currentState.OnEveryPhysicsFrame();
    }


}
