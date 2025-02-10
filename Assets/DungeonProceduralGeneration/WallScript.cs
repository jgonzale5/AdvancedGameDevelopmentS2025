using UnityEngine;

public class WallScript : MonoBehaviour
{
    /// <summary>
    /// The renderer of this wall so we can adjust the texture if we need to
    /// </summary>
    public MeshRenderer wallRenderer;

    /// <summary>
    /// An array of the compatible exits for this wall
    /// </summary>
    public string[] possibleExits;

    /// <summary>
    /// Tries to connect this wall to the given exit
    /// </summary>
    /// <param name="exit">The exit that this wall is trying to connect to</param>
    /// <returns>Returns false if this wall cannot connect to the exit, true if it successfully managed to move to that alignment object and block the exit.</returns>
    public bool TryConnect(RoomScript.ExitClass exit)
    {
        //For each keyword that this exit is compatible with
        foreach (string keyword in possibleExits)
        {
            //If the exit can connect to that keyword
            if (exit.keywords.Contains(keyword))
            {
                //Move this wall to that exit
                transform.position = exit.alignmentObject.transform.position;
                //And align it so it faces inwards
                transform.forward = -exit.alignmentObject.transform.forward;

                return true;
            }
        }

        //If no keyword was found to be compatible, this wall cannot connect to that exit
        return false;
    }
}
