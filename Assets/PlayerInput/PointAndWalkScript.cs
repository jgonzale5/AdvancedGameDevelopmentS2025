using UnityEngine;
//
using UnityEngine.AI;

//This component can only be added to game objects with the navmeshagent component
[RequireComponent(typeof(NavMeshAgent))]
public class PointAndWalkScript : MonoBehaviour
{
    //A reference to the navmeshagent of this player
    public NavMeshAgent agent;
    //The layers where the player can click for the player to move there. This is so we can ignore objects like glass, enemies, or the player themselves.
    public LayerMask walkableLayers;

    //At the start, if the agent hasn't been set we get the navmeshagent on this object and assign it to it
    private void Awake()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
    }

    void Update()
    {
        //When the player clicks
        if (Input.GetMouseButtonDown(0))
        {
            //We cast a ray from the camera in the direction that they clicked
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //If it hits something in one of the layers in the walkableLayers layermask
            if (Physics.Raycast(ray, out RaycastHit info, Mathf.Infinity, walkableLayers))
            {
                //We tell the navmeshagent to take this object there.
                agent.SetDestination(info.point);
            }
        }
    }
}
