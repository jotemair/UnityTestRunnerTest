using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SyncTransform : NetworkBehaviour
{
    public float lerpRate = 15;

    public float positionTreshold = 0.5f;
    public float rotationThreshold = 5.0f;

    private Vector3 lastPosition;
    private Quaternion lastRotation;

    [SyncVar] private Vector3 syncPosition;
    [SyncVar] private Quaternion syncRotation;

    private Rigidbody rigid;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LerpPosition()
    {
        if (!isLocalPlayer)
        {
            rigid.position = Vector3.Lerp(rigid.position, syncPosition, Time.deltaTime * lerpRate);
        }
    }

    void LerpRotation()
    {
        if (!isLocalPlayer)
        {
            rigid.rotation = Quaternion.Lerp(rigid.rotation, syncRotation, Time.deltaTime * lerpRate);
        }
    }

    [Command] void CmdSendPositionToServer(Vector3 _position)
    {
        syncPosition = _position;
    }

    [Command]
    void CmdSendRotationToServer(Quaternion _rotation)
    {
        syncRotation = _rotation;
    }

    [ClientCallback] void TransmitPosition()
    {
        if (isLocalPlayer && Vector3.Distance(rigid.position, lastPosition) > positionTreshold)
        {
            CmdSendPositionToServer(rigid.position);
            lastPosition = rigid.position;
        }
    }

    [ClientCallback]
    void TransmitRotation()
    {
        if (isLocalPlayer && (Quaternion.Angle(rigid.rotation, lastRotation) > rotationThreshold))
        {
            CmdSendRotationToServer(rigid.rotation);
            lastRotation = rigid.rotation;
        }
    }

    private void FixedUpdate()
    {
        TransmitPosition();
        LerpPosition();

        TransmitRotation();
        LerpRotation();
    }
}
