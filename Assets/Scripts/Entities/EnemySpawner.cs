using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EnemySpawner : NetworkBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 1.0f;

    public override void OnStartServer()
    {
        InvokeRepeating("SpawnEnemy", this.spawnInterval, this.spawnInterval);
    }

    void SpawnEnemy()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-4f, 4f), this.transform.position.y, Random.Range(-4f, 4f));
        GameObject enemy = Instantiate<GameObject>(enemyPrefab, spawnPosition, Quaternion.identity);
        NetworkServer.Spawn(enemy);
        Destroy(enemy, 10f);
    }
}
