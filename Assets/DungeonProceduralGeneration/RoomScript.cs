using UnityEngine;
//We add this one so we can use C# lists.
using System.Collections.Generic;

public class RoomScript : MonoBehaviour
{
    //We make it serializable so we can edit it in the inspector
    [System.Serializable]
    public class ExitClass
    {
        /// <summary>
        /// 
        /// </summary>
        public string name;
        /// <summary>
        /// The object at that exit that must be aligned with a matching one 
        /// </summary>
        public Transform alignmentObject;
        /// <summary>
        /// The keywords that the exit will accept to be matched with. 
        /// </summary>
        public List<string> keywords = new List<string>();

        /// <summary>
        /// Returns whether this room can connect with the one given.
        /// </summary>
        /// <param name="other">The room we're trying to connect to.</param>
        /// <returns>Returns true if both rooms share at least one keyword, false otherwise.</returns>
        public bool CanConnect(ExitClass other)
        {
            foreach (var kw in other.keywords)
            {
                if (this.keywords.Contains(kw))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public ExitClass[] exits;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="localExit"></param>
    /// <param name="otherExit"></param>
    /// <returns></returns>
    public bool TryConnect(ExitClass localExit, ExitClass otherExit)
    {
        //
        if (!localExit.CanConnect(otherExit))
            return false;

        //
        Transform alignmedParent = localExit.alignmentObject.parent;
        Transform roomParent = this.transform.parent;

        //
        localExit.alignmentObject.SetParent(roomParent);
        this.transform.SetParent(localExit.alignmentObject);

        //
        localExit.alignmentObject.position = otherExit.alignmentObject.position;
        localExit.alignmentObject.forward = -otherExit.alignmentObject.forward;

        //
        this.transform.SetParent(roomParent);
        localExit.alignmentObject.SetParent(alignmedParent);

        return true;
    }
}
