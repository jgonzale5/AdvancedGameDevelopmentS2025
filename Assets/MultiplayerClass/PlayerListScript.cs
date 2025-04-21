using UnityEngine;
//We use the tmpro library so we have access to the scripts
using TMPro;

public class PlayerListScript : MonoBehaviour
{
    //The element that will be displaying the player list
    public TextMeshProUGUI playerList;

    private void Update()
    {
        //We reset the playerlist
        playerList.text = string.Empty;

        //DO NOT DO IT THIS WAY
        //You do not want to use getcomponent or, even worse, find objects by type every frame
        //Ideally, you want the server to notice when a player joins, leaves, or changes its name and tell everyone to update their player list accordingly
        //We're doing it this way in the interest of time
        foreach (var playerObject in FindObjectsByType<MultiplayerPlayerManager>(FindObjectsSortMode.None))
        {
            playerList.text += playerObject.publicPlayerName + "\n";
        }
        
    }
}
