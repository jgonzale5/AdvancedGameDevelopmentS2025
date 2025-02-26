using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class DungeonGameManager : MonoBehaviour
{
    /// <summary>
    /// The script in charge of generating the dungeon
    /// </summary>
    public GenerationManager generationManager;
    /// <summary>
    /// The spawner that places the player into the map
    /// </summary>
    public DungeonPlayerSpawner playerSpawner;
    /// <summary>
    /// The currently active player
    /// </summary>
    public DungeonPlayerScript player;
    /// <summary>
    /// The nav mesh for this dungeon, in case we figure out how to build it
    /// </summary>
    public NavMeshSurface navMesh;
    /// <summary>
    /// An instance of this script to be easily referrenced by others
    /// </summary>
    public static DungeonGameManager Instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;

        //Tell the generation manager to generate the dungeon
        generationManager.GenerateDungeon();

        //After the dungeon has been generated
        player = playerSpawner.SpawnPlayer();

        //
        //navMesh.BuildNavMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
