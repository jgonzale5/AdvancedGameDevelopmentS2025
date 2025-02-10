using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenerationManager : MonoBehaviour
{
    //
    public int seed = 0;    

    /// <summary>
    /// The amount of rooms that this dungeon is made of, including the spawn room
    /// </summary>
    public int roomAmount = 6;
    /// <summary>
    /// The rooms that this generator can chose from when making the dungeon
    /// </summary>
    public RoomScript[] rooms;

    [Header("Closing exits")]
    /// <summary>
    /// The walls that this generator can chose from when closing off the exits of the dungeon
    /// </summary>
    public WallScript[] walls;
    /// <summary>
    /// How close the position of the alignment of two exits must be to consider them connected
    /// </summary>
    public float exitProximityTolerance = 0.1f;

    /// <summary>
    /// Dictionary used to keep track of exits spawned, specifically for when creating and connecting rooms
    /// </summary>
    public Dictionary<string, Deck<RoomScript.ExitClass>> availableExits = new Dictionary<string, Deck<RoomScript.ExitClass>>();
    /// <summary>
    /// List of exits so we can close them off at the end
    /// </summary>
    [SerializeField]
    private List<RoomScript.ExitClass> openExits = new List<RoomScript.ExitClass>();


    private void Start()
    {
        Random.InitState(this.seed);

        //We chose a random number between 0 and rooms.Length - 1
        int randRoom = Random.Range(0, rooms.Length);

        //We spawn the room
        SpawnRoom(rooms[randRoom], true);

        for (int i = 1; i < roomAmount; i++)
        {
            randRoom = Random.Range(0, rooms.Length);
            SpawnRoom(rooms[randRoom]);
        }

        //
        ConnectExits();

        //Close off all exits left open
        CloseOffExits();
    }

    #region RoomSpawning

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
                        //Debug.Log("Trying to connect " + otherExit.name + " and " + exit.name);
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

            //We add the new exit to the list of exits that needs to be closed off by the end
            openExits.Add(newExit);
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

        //Remove the open exit from the list
        openExits.Remove(exit);
    }

    #endregion

    
    private void ConnectExits()
    {
        for (int i = openExits.Count - 1; i >= 0; i--)
        {
            for (int other = 0; other < i; other++)
            {
                if (!openExits[i].CanConnect(openExits[other]))
                    continue;

                if (Vector3.Distance(
                    openExits[i].alignmentObject.position,
                    openExits[other].alignmentObject.position) <= exitProximityTolerance)
                {
                    openExits.RemoveAt(i);
                    openExits.RemoveAt(other);
                    i--;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Spawns random walls and attempts to connect them with all of the available exits
    /// </summary>
    private void CloseOffExits()
    {
        int wallsToSpawn = openExits.Count;
        Debug.Log("Spawning " + wallsToSpawn + " walls");

        //For each exit available, we need to make a wall
        for (int n = 0; n < wallsToSpawn; n++)
        {
            //We make the new wall
            WallScript newWall = Instantiate(walls[Random.Range(0, walls.Length)]);
            //We initialize the connection succeeded variable
            bool connectionSucceeded = false;

            //And test it agaisnt every exit
            for (int i = openExits.Count - 1; i >= 0; i--)
            {
                connectionSucceeded = false;

                //If the new wall could be connected successfully to the exit
                if (newWall.TryConnect(openExits[i]))
                {
                    //The wall successfully connected
                    connectionSucceeded = true;

                    //Remove this exit from the list of open exits
                    openExits.Remove(openExits[i]);

                    //Break out of this loop
                    break;
                }
            }

            //If no fitting exit was found, we display a message
            if (!connectionSucceeded)
            {
                Debug.Log("Wall " + newWall.name + " failed to find a matching exit.");
            }
        }
    }
}
