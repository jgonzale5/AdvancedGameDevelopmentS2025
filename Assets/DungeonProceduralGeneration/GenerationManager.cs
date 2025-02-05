using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenerationManager : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    public int roomAmount = 6;
    /// <summary>
    /// 
    /// </summary>
    public RoomScript[] rooms;

    public Dictionary<string, Deck<RoomScript.ExitClass>> availableExits = new Dictionary<string, Deck<RoomScript.ExitClass>>();

    private void Start()
    {
        //We chose a random number between 0 and rooms.Length - 1
        int randRoom = Random.Range(0, rooms.Length);

        //We spawn the room
        SpawnRoom(rooms[randRoom], true);

        for (int i = 1; i < roomAmount; i++)
        {
            randRoom = Random.Range(0, rooms.Length);
            SpawnRoom(rooms[randRoom]);
        }
    }

    /// <summary>
    /// Spawns the specified room at the first available exit
    /// </summary>
    /// <param name="room">The room that we'll spawn</param>
    /// <param name="ignoreConnections">Whether we should ignore open connections and just place the room at (0,0,0). Mainly useful for the first room.</param>
    public void SpawnRoom(RoomScript room, bool ignoreConnections = false)
    {

        //We instantiate the room
        RoomScript newRoom = Instantiate(room, Vector3.zero, Quaternion.identity, transform);
        Debug.Log("Spawned " + newRoom.name);

        //For each exit in the newly spawned room
        foreach (var exit in newRoom.exits)
        {
            //For each keyword in that exit
            foreach (string keyword in exit.keywords)
            {
                Debug.Log("Looking for keyword " + keyword);

                //If we want to force this room to register its exits, we don't try to connect it to another room
                if (ignoreConnections)
                {
                    RegisterExitsInRoom(newRoom);
                    return;
                }

                //If there are available exits with that keyword
                //if (availableExits.TryGetValue(keyword, out var exitsWithKeyword))
                if (availableExits.TryGetValue(keyword, out var exitsWithKeyword))
                {
                    //For each exit available with that keyword
                    foreach (var otherExit in exitsWithKeyword)
                    {
                        Debug.Log("Trying to connect " + otherExit.name + " and " + exit.name);
                        //We try to connect the room
                        //If it fails, we move to the next exit within that keyword
                        if (!newRoom.TryConnect(exit, otherExit))
                        {
                            continue;
                        }

                        //If it doesn't fail, we register the exits from the recently spawned room
                        RegisterExitsInRoom(newRoom, exit);

                        //Remove the exit we connected to from the dictionary of available exits
                        RemoveExit(otherExit);

                        //End this function
                        return;
                    }
                }
            }
        }
        
        
    }
    
    /// <summary>
    /// Registers all exits in new room
    /// </summary>
    /// <param name="room"></param>
    private void RegisterExitsInRoom(RoomScript room, RoomScript.ExitClass exceptForExit = null)
    {
        //For each exit in this room
        foreach (var newExit in room.exits)
        {
            //If the current exit is the one we're told to not register, skip to the next exit
            if (newExit == exceptForExit)
            {
                continue;
            }

            //For each keyword in each exit
            foreach (string keyword in newExit.keywords)
            {                
                //We register the exit
                RegisterExit(newExit, keyword);
            }
        }
    }

    /// <summary>
    /// Registers the exit given under the keyword given
    /// </summary>
    /// <param name="exit">The exit to be registered under available exits.</param>
    /// <param name="keyword">The or one of the keywords to be used to locate that exit.</param>
    private void RegisterExit(RoomScript.ExitClass exit, string keyword)
    {
        //If there is deck of available exits registered with that keyword
        if (availableExits.ContainsKey(keyword))
        {
            //Add it to the deck
            availableExits[keyword].Add(exit);
        }
        //Otherwise
        else
        {
            //Make a new deck and register under that keyword with the newly added exit
            Deck<RoomScript.ExitClass> newList = new Deck<RoomScript.ExitClass>();
            newList.Add(exit);
            availableExits.Add(keyword, newList);
        }

        //Shuffle the exits for that keyword
        availableExits[keyword].Shuffle();
    }

    /// <summary>
    /// Removes the specified exit from the availableExits list
    /// </summary>
    /// <param name="exit">The exit that is being removed.</param>
    private void RemoveExit(RoomScript.ExitClass exit)
    {
        //For each keyword in the exit we're removing...
        foreach (var keyword in exit.keywords)
        {
            //If the keyword is in the dictionary 
            if (availableExits.TryGetValue(keyword, out Deck<RoomScript.ExitClass> value))
            {
                //Remove the exit from the list
                value.Remove(exit);
            }
        }
    }
}
