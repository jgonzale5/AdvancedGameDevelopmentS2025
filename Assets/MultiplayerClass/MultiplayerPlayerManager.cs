using UnityEngine;
//We add a reference to netcode so we can use its classes
using Unity.Netcode;
//A reference to collections to use FixedString32Bytes
using Unity.Collections;

//We derive this class from the NetworkBehaviour instead of MonoBehaviour
public class MultiplayerPlayerManager : NetworkBehaviour
{
    //A network variable to keep track of the player name accross all instances of this object in the session
    private NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>();

    //

    //Read-Only property that returns the playerName for this object
    public string publicPlayerName
    {
        get
        {
            return playerName.Value.ToString();
        }

        //set
        //{
        //    playerName.Value = value;
        //}
    }

    //At start, ask to change the player name to the local instance
    private void Start()
    {
        //If this instance is the local one, request the name change
        if (IsLocalPlayer)
            ChangeName(PlayerNameInput.localPlayerName);

        //This game object will not destroy on load
        DontDestroyOnLoad(this.gameObject);


        //
        
        //FindObjectsByType<PlayerListHandler>(FindObjectsSortMode.None)
    }

    //A function to change the name, it asks the server for permission
    public void ChangeName(string name)
    {
        if (IsLocalPlayer)
        {
            RequestNameChangeRpc(name);
        }
    }

    //The RPC to ask the server for permission for this object to change its player name
    [Rpc(SendTo.Server)]
    public void RequestNameChangeRpc(string name, RpcParams rpcParams = default)
    {
        playerName.Value = name;
    }
}
