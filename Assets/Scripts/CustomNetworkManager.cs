using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CustomNetworkManager : NetworkManager
{
    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);

        /*// Spawn an instance of Enemy onto client
        GameObject enemyPrefab = spawnPrefabs[0];
        GameObject enemy = Instantiate(enemyPrefab, new Vector3(0f,2f,0f), Quaternion.identity);

        NetworkServer.Spawn(enemy);*/
    }
}
