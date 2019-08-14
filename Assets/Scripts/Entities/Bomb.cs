using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

public class Bomb : NetworkBehaviour
{
    public float explosionRadius = 2f;
    public float explosionDelay = 3f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Boom());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Boom()
    {
        yield return new WaitForSeconds(explosionDelay);

        Cmd_Boom(transform.position, explosionRadius);
    }

    [Command]
    public void Cmd_Boom(Vector3 pos, float radius)
    {
        List<Collider> hits = new List<Collider>(Physics.OverlapSphere(pos, radius));

        foreach(var hit in hits)
        {
            NetworkServer.Destroy(hit.gameObject);
        }
    }
}
