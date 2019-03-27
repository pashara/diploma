using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowController : MonoBehaviour
{
    public Transform objectToFollow;
    public Vector3 offset;
    public float followSpeed = 10;
    public float lookSpeed = 10;
    public void LookAtTarget()
    {
        Vector3 _lookDirection = objectToFollow.position - transform.position;
        Quaternion _rot = Quaternion.LookRotation(_lookDirection, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, _rot, lookSpeed * Time.deltaTime);
        // transform.rotation = objectToFollow.transform.localRotation;
    }

    public void MoveToTarget()
    {
        Vector3 _targetPos = objectToFollow.position +
                             objectToFollow.forward * offset.z +
                             objectToFollow.right * offset.x +
                             objectToFollow.up * offset.y;
        transform.position = Vector3.Lerp(transform.position, _targetPos, followSpeed * Time.deltaTime);
        // transform.position = _targetPos;
    }

    private void FixedUpdate()
    {
        LookAtTarget();
        MoveToTarget();
    }


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            offset = new Vector3(7f, 2f, 0f);
        }
        if(Input.GetKeyDown(KeyCode.W))
        {
            offset = new Vector3(0f, 2f, 7f);
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            offset = new Vector3(-7f, 2f, 0f);
        }
        if(Input.GetKeyDown(KeyCode.T))
        {
            offset = new Vector3(0f, 2f, -7f);
        }
    }

}