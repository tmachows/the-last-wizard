using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _Enemy;                // The enemy prefab to be spawned.
    [SerializeField]
    private float s_SawnTime = 3f;            // How long between each spawn.
    [SerializeField]
    private Transform[] _SpawnPoints;         // An array of the spawn points this enemy can spawn from.


    void Start()
    {
        // Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time.
        InvokeRepeating("Spawn", s_SawnTime, s_SawnTime);
    }


    void Spawn()
    {

        // Find a random index between zero and one less than the number of spawn points.
        int spawnPointIndex = Random.Range(0, _SpawnPoints.Length);

        // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
        Instantiate(_Enemy, _SpawnPoints[spawnPointIndex].position, _SpawnPoints[spawnPointIndex].rotation);
    }
}
