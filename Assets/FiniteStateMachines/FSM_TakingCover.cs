using UnityEngine;
using UnityEngine.AI;
public class FSM_TakingCover : StateBaseClass
{
    //How close the enemy has to be to their destination to consider to have "arrived"
    public float destinationTolerance = 1f;
    //The destination of this enemy
    public Vector3 targetPos;
    //the source of the explosion
    public ExplodeOnClickScript explosionSource;

    public FSM_TakingCover(FSM_EnemyScript enemyScript) : base(enemyScript) { }

    public override void ChangeState(StateBaseClass newState, ref StateBaseClass currentState)
    {
        //The current state executes its exit function
        currentState.OnExitState();
        //The current state is changed
        currentState = newState;
        //The new current state executes its enter function
        currentState.OnEnterState();
    }

    public void SetExplosion(ExplodeOnClickScript explosionSource)
    {
        this.explosionSource = explosionSource;
    }

    //When this enemy enters this state
    public override void OnEnterState()
    {
        Debug.Log("looking for cover");

        //We initialize these to find the closest node
        float shortestDistance = Mathf.Infinity;
        CoverScript targetNode = null;

        //This is so we can keep the current destination if nothing changes
        Vector3 currentDestination = enemyScript.navMeshAgent.destination;

        //For each node in the scene
        foreach (var node in GameObject.FindObjectsByType<CoverScript>(FindObjectsSortMode.None))
        {
            //If the node doesn't see the exploside (i.e. is blocked from it)
            if (!node.SeesPositionUninterrupted(explosionSource.transform.position))
            {
                //We set the destination so the navmesh calculates the path
                enemyScript.navMeshAgent.SetDestination(node.transform.position);

                //If the path is shorter than the current shortest distance
                if (enemyScript.navMeshAgent.remainingDistance < shortestDistance)
                {
                    //We keep track of that node
                    shortestDistance = enemyScript.navMeshAgent.remainingDistance;
                    targetNode = node;
                }
            }
        }

        //If no node was found
        if (targetNode == null)
        {
            Debug.Log("No cover found");
            //The enemy keeps going where it is
            enemyScript.navMeshAgent.SetDestination(currentDestination);
            return;
        }

        Debug.Log("Taking cover");
        //The new destination is set otherwise
        enemyScript.navMeshAgent.SetDestination(targetNode.transform.position);
    }

    public override void OnEveryFrame()
    {
        //If the distance between the enemy and its destination is less than the maximum accepted or the path cannot be reached
        if (Vector3.Distance(enemyScript.transform.position, targetPos) <= destinationTolerance
            || enemyScript.navMeshAgent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid)
        {
            Debug.Log("In Cover");

            //If the explosion is over, we can go back to chasing the player
            if (explosionSource.hasExploded)
                ChangeState(enemyScript.chaseState, ref enemyScript.currentState);
        }
        else
        {
            Debug.Log("Going to cover");
        }
    }

    public override void OnEveryPhysicsFrame()
    {

    }

    public override void OnExitState()
    {
        Debug.Log("Back into combat.");
    }
}
