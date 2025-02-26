using UnityEngine;

public class DungeonPlayerSpawner : MonoBehaviour
{
    /// <summary>
    /// The prefab that will be spawned. This is the player character
    /// </summary>
    public Transform playerPrefab;
    /// <summary>
    /// The location of the player spawn
    /// </summary>
    public Transform playerSpawnPoint;

    //
    private void Awake()
    {
        Debug.Log("assigning player spawner");
        DungeonGameManager.Instance.playerSpawner = this;
    }

    /// <summary>
    /// This function forces this spawner to spawn the player
    /// </summary>
    /// <returns>Returns the spawned player</returns>
    public DungeonPlayerScript SpawnPlayer()
    {
        if (Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation)
            .TryGetComponent<DungeonPlayerScript>(out DungeonPlayerScript player))
        {
            return player;
        }

        return null;
    }


}
