using UnityEngine;

public class PlayerNameInput : MonoBehaviour
{
    //A static variable to keep track of what name the player entered
    public static string localPlayerName;

    //This function will change the localPlayerName to the sent value
    public void SetName(string to)
    {
        localPlayerName = to;
    }
}
