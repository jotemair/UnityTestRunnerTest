using UnityEngine;

public class FollowPosition : MonoBehaviour
{
    public Transform attached;

    void Update()
    {
        transform.position = attached.position;
    }
}
