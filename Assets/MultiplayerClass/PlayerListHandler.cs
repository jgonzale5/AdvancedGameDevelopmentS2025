using System.Collections.Generic;
using UnityEngine;
//
using Unity.Netcode;

public class PlayerListHandler : NetworkBehaviour
{
    //The list of players
    public Dictionary<ulong, MultiplayerPlayerManager> playerList = new Dictionary<ulong, MultiplayerPlayerManager>();

    /// <summary>
    /// Keeps track of whether the player list is running, to prevent the event from being registered multiple times
    /// </summary>
    bool runningList = false;

    /// <summary>
    /// Will allow us to access the player list from anywhere else on the scene
    /// </summary>
    public static PlayerListHandler Singleton;

    //At start
    private void Start()
    {
        //If there's already a registered singleton, destroy this one
        if (Singleton != null)
            Destroy(this);

        //Otherwise, register this script as the singleton for the class
        Singleton = this;

        //Mark this script so it isn't destroyed when a new scene is loaded
        DontDestroyOnLoad(Singleton);
    }

    public void StartPlayerList()
    {
        if (runningList)
            return;

        NetworkManager.OnClientConnectedCallback += RegisterPlayer;
        NetworkManager.OnClientDisconnectCallback += UnregisterPlayer;

        runningList = true;
    }

    public void StopPlayerList()
    {
        if (!runningList)
            return;

        NetworkManager.OnClientConnectedCallback -= RegisterPlayer;
        NetworkManager.OnClientDisconnectCallback -= UnregisterPlayer;

        runningList = false;
    }

    /// <summary>
    /// When a client joins, we register them in the list
    /// </summary>
    /// <param name="playerID">The ID of the player that is being registered</param>
    public void RegisterPlayer(ulong playerID)
    {
        //Get the player object for the client with the given ID
        var registeredPlayerObject = NetworkManager.Singleton.ConnectedClients[playerID].PlayerObject;

        //Get the player manager attached to that object
        var registeredPlayer = registeredPlayerObject.GetComponent<MultiplayerPlayerManager>();

        //Register it
        playerList.Add(playerID, registeredPlayer);

        string debug = "";
        foreach (KeyValuePair<ulong, MultiplayerPlayerManager> p in playerList)
        {
            debug += p.Value.publicPlayerName + "\n";
        }
        Debug.Log(debug);

    }

    /// <summary>
    /// When a client leaves, this will unregister it from the list
    /// </summary>
    /// <param name="playerID">The ID of the player that is being unregistered</param>
    public void UnregisterPlayer(ulong playerID)
    {
        //By removing them
        playerList.Remove(playerID);
    }
}
