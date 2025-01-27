using UnityEngine;
//Note that we added this library to be able to use the navmesh components
using UnityEngine.AI;

//This script must be in the same game object as a navmeshagent
[RequireComponent(typeof(NavMeshAgent))]
public class FSM_EnemyScript : MonoBehaviour
{
    //The script in charge of controlling the navigation of this AI agent
    public NavMeshAgent navMeshAgent;

    [Header("Awareness")]
    //
    public EnemyAwarenessScript awarenessSphere;
    //
    public string playerTag;
    //
    public GameObject target;

    [Header("Vision")]
    //
    public float coneOfVisionAngle;
    //
    public LayerMask visionLayers;
    //
    public GameObject eyes;
    [Header("Hearing")]
    //
    public EnemyHearingScript hearingScript;

    [Header("States")]
    public StateBaseClass currentState;
    public StateBaseClass idleState;
    public StateBaseClass patrolState;
    public StateBaseClass chaseState;
    public StateBaseClass investigateState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        //Get the navmeshagent in this game object
        navMeshAgent = GetComponent<NavMeshAgent>();



        idleState = new FSM_IdleState(this);
        patrolState = new FSM_PatrolState(this);
        chaseState = new FSM_ChaseState(this);
        investigateState = new FSM_InvestigateState(this);

        //Entry state gets assigned
        currentState = idleState;
        currentState.OnEnterState();
    }

    private void OnEnable()
    {
        //If we dont have an awareness sphere assigned we look for it here
        if (awarenessSphere == null)
            awarenessSphere = GetComponentInChildren<EnemyAwarenessScript>();

        awarenessSphere.OnColliderEntersAwareness += TargetIfPlayer; 
        hearingScript.OnSoundHeard += InvestigateSound; 
    }

    private void OnDisable()
    {
        awarenessSphere.OnColliderEntersAwareness -= TargetIfPlayer; 
        hearingScript.OnSoundHeard -= InvestigateSound; 
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

    /// <summary>
    /// Checks if the object that entered is the player and displays a message if that's the case
    /// </summary>
    /// <param name="col"></param>
    public void TargetIfPlayer(Collider col)
    {
        if (col.gameObject.CompareTag(playerTag))
        {
            Debug.Log("Player is in awareness");
        }
    }

    /// <summary>
    /// Checks if the player is visible
    /// </summary>
    /// <returns>Whether the player is in range, in the FOV, and not blocked by a wall</returns>
    public bool CheckIfPlayerVisible()
    {
        Collider player;

        if (!awarenessSphere.IsTagInRange(playerTag, out player))
        {
            return false;
        }

        if (!IsObjectInRange(player.transform))
        {
            return false;
        }

        if (!IsObjectVisible(player.transform))
        {
            return false;
        }

        target = player.gameObject;
        //Debug.Log("Player detected!");

        return true;
    }

    /// <summary>
    /// Returns whether the rangle between obj and this object is less than the cone of vision angle.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns>Returns ture if obj is within the field of view of this enemy.</returns>
    public bool IsObjectInRange(Transform obj)
    {
        //Get us the angle between the direct line of vision of the eyes and the player in respect to the eyes
        float objAngle = Vector3.Angle(eyes.transform.forward, obj.position - eyes.transform.position);

        Debug.DrawRay(eyes.transform.position, obj.position - eyes.transform.position, (objAngle < coneOfVisionAngle) ? Color.red : Color.blue);

        return objAngle < coneOfVisionAngle;
    }

    /// <summary>
    /// Checks whether there's something blocking the view of obj from the enemy.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns>Returns true if this object has uninterrupted view of obj</returns>
    public bool IsObjectVisible(Transform obj)
    {
        Ray ray = new Ray(eyes.transform.position, obj.position - eyes.transform.position);

        if (Physics.Raycast(ray, out RaycastHit hitObject, Mathf.Infinity, visionLayers))
        {
            Debug.DrawRay(ray.origin, ray.direction, Color.red);
            return hitObject.transform == obj;
        }

        Debug.DrawRay(ray.origin, ray.direction, Color.blue);
        return false;
    }

    /// <summary>
    /// When called, will change the enemy to the investigate state and have them check the position of the sound played
    /// </summary>
    /// <param name="sound">The sound played that will be investigated</param>
    public void InvestigateSound(SoundClass sound)
    {
        if (currentState is FSM_ChaseState)
            return;

        ((FSM_InvestigateState)investigateState).SetTargetPos(sound.position);
        currentState.ChangeState(investigateState, ref currentState);
    }
}
