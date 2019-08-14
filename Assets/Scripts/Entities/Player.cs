﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

public class Player : NetworkBehaviour
{
    public GameObject bombPrefab = null;

    public Transform attachPoint = null;
    public Camera attachedCamera = null;

    public float speed = 10f, jump = 10f;
    public LayerMask ignoreLayers;
    public float rayDistance = 10f;
    public bool isGrounded = false;
    private Rigidbody rigid;
    #region Unity Events
    public void OnDestroy()
    {
        if (null != attachedCamera)
        {
            Destroy(attachedCamera.gameObject);
        }

        if (null != attachPoint)
        {
            Destroy(attachPoint.gameObject);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * rayDistance);
    }
    private void Start()
    {
        rigid = GetComponent<Rigidbody>();

        attachPoint.SetParent(null);
        attachedCamera.transform.SetParent(null);
        if (isLocalPlayer)
        {
            attachedCamera.enabled = true;
            //attachedCamera.rect = new Rect(0f, 0f, 0.5f, 1f);
            attachPoint.gameObject.SetActive(true);
        }
        else
        {
            attachedCamera.enabled = false;
            //attachedCamera.rect = new Rect(0.5f, 0f, 0.5f, 1f);
            attachPoint.gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        Ray groundRay = new Ray(transform.position, Vector3.down);
        isGrounded = Physics.Raycast(groundRay, rayDistance, ~ignoreLayers);
    }
    private void OnTriggerEnter(Collider col)
    {
        Item item = col.GetComponent<Item>();
        if (item)
        {
            item.Collect();
        }
    }
    private void Update()
    {
        if (isLocalPlayer)
        {
            float inputH = Input.GetAxis("Horizontal");
            float inputV = Input.GetAxis("Vertical");
            Move(inputH, inputV);
            if (Input.GetButtonDown("Jump"))
            {
                Jump();
                DropBomb();
            }
        }
    }
    #endregion

    #region Commands

    [Command]
    public void Cmd_SpawnBomb(Vector3 pos)
    {
        GameObject bomb = Instantiate(bombPrefab, pos, Quaternion.identity);
        NetworkServer.Spawn(bomb);
    }

    #endregion

    #region Custom

    public void DropBomb()
    {
        Cmd_SpawnBomb(transform.position);
    }
    public void Jump()
    {
        if (isGrounded)
        {
            rigid.AddForce(Vector3.up * jump, ForceMode.Impulse);
        }
    }

    public void Move(float inputH, float inputV)
    {
        Vector3 direction = new Vector3(inputH, 0, inputV);

        // [Optional] Move with camera
        Vector3 euler = Camera.main.transform.eulerAngles;
        direction = Quaternion.Euler(0, euler.y, 0) * direction; // Convert direction to relative direction to camera only on Y

        rigid.AddForce(direction * speed);
    }

	public bool Shoot()
	{
		Ray ray = new Ray(transform.position, Vector3.forward);
		RaycastHit hit;
		// Cast a ray forward
		if (Physics.Raycast(ray, out hit))
		{
			// If ray hit an item
			Item item = hit.collider.GetComponent<Item>();
			if (item)
			{
				// Collect it
				item.Collect();
				// Ray hit an item!
				return true;
			}
		}
		// Ray didnt hit anything
		return false;
	}
	#endregion
}

