using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Player : CustomNetworkBehaviour
{
    [SerializeField] NetworkIdentity networkIdentity;
    [SerializeField] NetworkTransform networkTransform;
    [SerializeField] NetworkTransformChild networkTransformChild;
    [SerializeField] CarDriver prefab;


    //[SyncVar] string username;
    [SyncVar] int level;
    //[SyncVar(hook = "OnPointsChanged")] int points;


    //public int Points
    //{
    //    get
    //    {
    //        return points;
    //    }
    //    set
    //    {
    //        points = value;
    //    }
    //}


    //void OnPointsChanged(int points)
    //{

    //}



    [SerializeField] CarDriver currentCarDriver;

    Vector3 spawnPosition;


    public void Initialize()
    {
        spawnPosition = transform.position;
        currentCarDriver = Instantiate<CarDriver>(prefab);
        currentCarDriver.transform.SetParent(transform);
        currentCarDriver.transform.localPosition = Vector3.zero;
        networkTransformChild.target = currentCarDriver.MovablePart;
        currentCarDriver.Initialize();
        networkIdentity.enabled = true;
        networkTransform.enabled = true;
        networkTransformChild.enabled = true;

        currentCarDriver.OnEnterTrigger += CurrentCarDriver_OnEnterTrigger;
    }


    public void Deinitialize()
    {
        currentCarDriver.Deinitialize();
        Destroy(currentCarDriver);
        currentCarDriver = null;
        networkIdentity.enabled = false;
        networkTransform.enabled = false;
        networkTransformChild.enabled = false;

        currentCarDriver.OnEnterTrigger -= CurrentCarDriver_OnEnterTrigger;
    }


    public void Respawn()
    {
        currentCarDriver.carTrail.DestroyTrail();
        currentCarDriver.carTrail.Initialize(currentCarDriver.carTrail.transform);
        currentCarDriver.MainRigidBody.position = spawnPosition;
        currentCarDriver.MainRigidBody.rotation = Quaternion.identity;
        currentCarDriver.MainRigidBody.velocity = Vector3.zero;
    }


    public override void OnStartLocalPlayer()
    {
        currentCarDriver.IsLocalPlayer = true;
    }

    public override void OnStartClient()
    {
        Initialize();
    }



    private void CurrentCarDriver_OnEnterTrigger(TriggerType obj)
    {
        Respawn();
    }

}
