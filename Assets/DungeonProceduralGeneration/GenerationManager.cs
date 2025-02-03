using System.Collections.Generic;
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

    public Dictionary<string, List<RoomScript.ExitClass>> availableExits = new Dictionary<string, List<RoomScript.ExitClass>>();

    private void Start()
    {
        //We chose a random number between 0 and rooms.Length - 1
        int randRoom = Random.Range(0, rooms.Length);

        //We spawn the room
        SpawnRoom(rooms[randRoom]);

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
    public void SpawnRoom(RoomScript room)
    {

        //We instantiate the room
        RoomScript newRoom = Instantiate(room, Vector3.zero, Quaternion.identity, transform);

        bool success = false;

        //
        foreach (var exit in newRoom.exits)
        {
            foreach (string keyword in exit.keywords)
            {
                if (availableExits.TryGetValue(keyword, out var exitsWithKeyword))
                {
                    foreach (var otherExit in exitsWithKeyword)
                    {
                        success = newRoom.TryConnect(exit, otherExit);

                        if (success)
                            break;
                    }

                    if (success)
                        break;
                }

                if (success)
                    break;
            }

            if (success)
                break;
        }
        
        //For each exit in this room
        foreach (var exit in newRoom.exits)
        {
            //For each keyword in each exit
            foreach (string keyword in exit.keywords)
            {
                //We register the exit
                RegisterExit(exit, keyword);
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
        if (availableExits.ContainsKey(keyword))
        {
            availableExits[keyword].Add(exit);
        }
        else
        {
            List<RoomScript.ExitClass> newList = new List<RoomScript.ExitClass>();
            newList.Add(exit);
            availableExits.Add(keyword, newList);
        }
    }
}
