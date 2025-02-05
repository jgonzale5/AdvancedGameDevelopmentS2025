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
        //If the local exit cannot connect with the other exit, return false
        if (!localExit.CanConnect(otherExit))
            return false;

        //We make some temporary variables so we can switch the child and the parent
        Transform alignmedParent = localExit.alignmentObject.parent;
        Transform roomParent = this.transform.parent;

        //We make the alignment the parent of the room
        localExit.alignmentObject.SetParent(roomParent);
        this.transform.SetParent(localExit.alignmentObject);

        //We align the alignment arrow to face into the available exit
        localExit.alignmentObject.position = otherExit.alignmentObject.position;
        localExit.alignmentObject.forward = -otherExit.alignmentObject.forward;

        //The room and the alignment parenting is set back to normal
        this.transform.SetParent(roomParent);
        localExit.alignmentObject.SetParent(alignmedParent);

        //Once the room is placed here, the room checks if it's overlapping with another room
        return CheckOverlap(GetComponentInChildren<Collider>());
    }

    /// <summary>
    /// Checks if this room is currently overlapping with another room
    /// </summary>
    /// <param name="thisRoomCollider">The box collider of this room</param>
    /// <returns>Returns true if this room is not overlapping with another room</returns>
    public bool CheckOverlap(Collider thisRoomCollider)
    {
        Vector3 colliderDimensions = thisRoomCollider.bounds.extents;

        thisRoomCollider.enabled = false;

        bool isOverlapping = Physics.CheckBox(transform.position, colliderDimensions);

        thisRoomCollider.enabled = true;

        return !isOverlapping;
    }
}
