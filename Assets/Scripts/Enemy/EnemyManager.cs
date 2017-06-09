using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private Terrain _Terrain;
    [SerializeField] private GameObject _Wizard;
    private Vector3 _WizardPosition;
    [SerializeField] private GameObject[] _Enemies;
    [SerializeField] private float _SpawnTime = 3f;
    [SerializeField] private float _Radius = 10f;

    void Start()
    {
        _WizardPosition = _Wizard.transform.position;
        InvokeRepeating("Spawn", 1, _SpawnTime);
    }


    void Spawn()
    {
        Vector3 position = RandomCircle(_WizardPosition, _Radius);
        Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, _WizardPosition - position);
        GameObject enemy = _Enemies[Random.Range(0, _Enemies.Length)];
        GameObject spawnedEnemy = Instantiate(enemy, position, rotation);
        spawnedEnemy.GetComponent<EnemyMover>().SetWizardPosition(_WizardPosition);
    }

    Vector3 RandomCircle(Vector3 center, float radius)
    {
        float angle = Random.value * 360;
        Vector3 position;
        position.x = center.x + radius * Mathf.Sin(angle * Mathf.Deg2Rad);
        position.z = center.z + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
        position.y = center.y;
        position.y = _Terrain.SampleHeight(position);
        return position;
    }
}
