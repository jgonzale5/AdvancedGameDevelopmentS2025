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

    public void StartPlayerList()
    {
        if (runningList)
            return;

        //NetworkManager.OnClientConnectedCallback += RegisterPlayer;
        NetworkManager.OnClientDisconnectCallback += UnregisterPlayer;

        runningList = true;
    }

    public void StopPlayerList()
    {
        if (!runningList)
            return;

        //NetworkManager.OnClientConnectedCallback -= RegisterPlayer;
        NetworkManager.OnClientDisconnectCallback -= UnregisterPlayer;

        runningList = false;
    }


    public void RegisterPlayer(ulong playerID)
    {
        var registeredPlayerObject = NetworkManager.Singleton.ConnectedClients[playerID].PlayerObject;


    }

    public void UnregisterPlayer(ulong playerID)
    {

    }
}
