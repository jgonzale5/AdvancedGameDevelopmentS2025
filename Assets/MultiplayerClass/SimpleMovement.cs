using UnityEngine;
//We add the Netcode library
using Unity.Netcode;
using System.Security.Principal;

//You need to change the inherited class since it will inteface with the network scripts
public class SimpleMovement : NetworkBehaviour
{
    [Header("Movement")]
    //The variable controlling the speed of the player
    public float speed = 2f;

    [Header("Online Variable")]
    //The variable that we will use to update the player position
    public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>();

    // Update is called once per frame
    void Update()
    {

        //If this is the local player, we read the player input and update it
        if (IsOwner)
        {
            Debug.Log("Local");
            //
            Vector3 movement = new Vector3(
                Input.GetAxis("Horizontal") * speed,
                0,
                Input.GetAxis("Vertical") * speed);

            //We can do it here because players are not supposed to have access to their own object, but request the server to make the changes
            //networkPosition.Value = transform.position + movement;

            //We multiply the movement by delta time
            movement *= Time.deltaTime;

            //We displace the transform by the specified movement
            transform.position += movement;
        }
        else
        {
            //If this is an instance of this object that doesn't belong to the local player, it's position is determined by the network position given to it
            transform.position = networkPosition.Value;
        }
    }

    //We submit the position request at fixed update so we send less messages. 
    private void FixedUpdate()
    {
        if (IsOwner)
        {
            //Ask the server to update the player position
            SubmitPositionRequestRpc(transform.position);
        }
    }

    [Rpc(SendTo.Server)]
    private void SubmitPositionRequestRpc(Vector3 newPosition, RpcParams rpcParams = default)
    {
        Debug.Log("Moving " + this.name + " to " + newPosition.ToString());
        //Update the value of the network position to match the new position
        networkPosition.Value = newPosition;
    }


}
