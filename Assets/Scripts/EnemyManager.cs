using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _Enemy;
    [SerializeField]
    private float s_SawnTime = 3f;
    [SerializeField]
    private Transform[] _SpawnPoints;


    void Start()
    {
        InvokeRepeating("Spawn", s_SawnTime, s_SawnTime);
    }


    void Spawn()
    {
        int spawnPointIndex = Random.Range(0, _SpawnPoints.Length);
        
        Instantiate(_Enemy, _SpawnPoints[spawnPointIndex].position, _SpawnPoints[spawnPointIndex].rotation);
    }
}
