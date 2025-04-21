using UnityEngine;
//We add the netcode library to access network behaviours
using Unity.Netcode;
//We add this to be able to use the LoadSceneMode enumerator
using UnityEngine.SceneManagement;
//We add this to be able to use lists
using System.Collections.Generic;

public class MultiplayerPlayerSpawner : NetworkBehaviour
{
    //The prefab of the player 
    public Transform playerPrefab;

    //Adding it on enabled so as soon as the server loads the scene we tell the network manager to tell it when all players are loaded
    private void OnEnable()
    {
        //We call SpawnPlayers when the load event is completed on all clients and the server
        NetworkManager.SceneManager.OnLoadEventCompleted += SpawnPlayers;
    }

    //Adding this so we don't try to call a function when this object doesn't exist or is disabled
    private void OnDisable()
    {
        //We do this so we don't try to call the function when this object is disabled or doesn't exist
        NetworkManager.SceneManager.OnLoadEventCompleted -= SpawnPlayers;
    }

    //Spawn the players connected
    public void SpawnPlayers(string loadedSceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        Debug.Log("Test");
        //If the player running this script is not the server, don't run it
        if (!IsHost)
            return;

        //For each player connected
        foreach (var playerID in clientsCompleted)
        {
            //Instantiate a player
            GameObject m_PrefabInstance = Instantiate(playerPrefab).gameObject;

            //Randomize the position of the object a little
            m_PrefabInstance.transform.position += new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));

            //Get the network object on the spawned player object
            var m_SpawnedNetworkObject = m_PrefabInstance.GetComponent<NetworkObject>();
            //Spawn it accross the network
            m_SpawnedNetworkObject.Spawn();

            m_SpawnedNetworkObject.ChangeOwnership(playerID);
        }
    }
}
