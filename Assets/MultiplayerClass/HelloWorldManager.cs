using Unity.Netcode;
using UnityEngine;

public class HelloWorldManager : MonoBehaviour
{
    /// <summary>
    /// The parent of the join and host buttons
    /// </summary>
    public GameObject connectionButtonParent;
    /// <summary>
    /// A reference to the network manager
    /// </summary>
    public NetworkManager networkManager;

    private void Update()
    {
        if (networkManager.IsClient == false && networkManager.IsServer == false)
        {
            connectionButtonParent.SetActive(true);
        }
        else
        {
            connectionButtonParent.SetActive(false);
        }
    }

    /// <summary>
    /// Call this function to start the session as a host
    /// </summary>
    public void StartHost()
    {
        networkManager.StartHost();
    }

    /// <summary>
    /// Call this function to join the session as a client
    /// </summary>
    public void StartClient()
    {
        networkManager.StartClient();
    }
}
