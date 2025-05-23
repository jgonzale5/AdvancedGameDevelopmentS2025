using Unity.Netcode;
using UnityEngine;
//We add a reference to the TMPro library so we can use its scripts
using TMPro;
//
using UnityEngine.UI;
//
using Unity.Netcode.Transports.UTP;

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

    /// <summary>
    /// A reference to the label that tells the player whether they're the client or the server
    /// </summary>
    public TextMeshProUGUI statusLabel;
    /// <summary>
    /// A reference to the button that starts the game
    /// </summary>
    public Button StartButton;

    /// <summary>
    /// This will tell the script the name of the scene we're trying to load
    /// </summary>
    public string arenaSceneName;


    [Header("IP Port Settings")]
    ///The field where the user enters the IP they want to connect to
    public TMP_InputField IPField;
    ///The field where the user enters the port they want to connect through
    public TMP_InputField PortField;
    //
    private UnityTransport uTransport;

    //
    private void Start()
    {
        //Get get the transport from the same object as the network manager
        uTransport = networkManager.gameObject.GetComponent<UnityTransport>();
        //We set the input field of the IP to hold the IP specified by the transport
        IPField.text = uTransport.ConnectionData.Address.ToString();
        //We set the input field of the port to hold the port specified by the transport
        PortField.text = uTransport.ConnectionData.Port.ToString();
    }

    private void Update()
    {
        //If the player is not the client or the server, that means they're not connected
        if (networkManager.IsClient == false && networkManager.IsServer == false)
        {
            connectionButtonParent.SetActive(true);
        }
        //If the player is either the client or the server, that means they are connected to the session
        else
        {
            connectionButtonParent.SetActive(false);
        }

        //Update the status labels
        StatusLabels();
        //
        StartButtonStatus();
    }

    /// <summary>
    /// Updates the label telling the player whether they are the client or the host
    /// </summary>
    private void StatusLabels()
    {
        if (networkManager.IsHost)
        {
            statusLabel.text = "Host";
        }
        else if (networkManager.IsClient)
        {
            statusLabel.text = "Client";
        }
        else
        {
            statusLabel.text = "Not Connected";

        }
    }

    /// <summary>
    /// This function will make the start button interactable only for the server and only when there are at least two players
    /// </summary>
    private void StartButtonStatus()
    {
        //Get the number of players by looking at the list of IDs
        int numberOfPlayers = networkManager.ConnectedClientsIds.Count;

        //Make the button interactable if the local networkmanager is the host and there are more than one player in the session
        StartButton.interactable = networkManager.IsHost && (numberOfPlayers > 1);
        
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

    /// <summary>
    /// This function will load the given scene
    /// </summary>
    public void LoadArenaLevel()
    {
        //If this is the host
        if (networkManager.IsHost)
        {
            //We load the scene specified
            networkManager.SceneManager.LoadScene(arenaSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

    //Will set hte IP of the Unity transport to match the one given by the caller of this function
    public void SetIP(string to)
    {
        uTransport.ConnectionData.Address = to.Trim();
    }

    //Will set the port of the Unity transport to match the one given by the caller of this function
    public void SetPort(string to)
    {
        if (ushort.TryParse(to, out ushort port))
            uTransport.ConnectionData.Port = port;
        else
        {
            Debug.LogError("Wrong port format. Couldn't be parsed.");
        }
    }
}
