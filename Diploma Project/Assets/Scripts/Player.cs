using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;


public class Player : CustomNetworkBehaviour
{
    #region Fields

    const int initialLifes = 4;
    public static event Action<Player> OnPlayerCreated;
    public static event Action<Player> OnPlayerDestroy;

    [SerializeField] NetworkIdentity networkIdentity;
    [SerializeField] NetworkTransform networkTransform;
    [SerializeField] NetworkTransformChild networkTransformChild;
    [SerializeField] CarDriver prefab;

    [SerializeField] CarDriver currentCarDriver;

    Vector3 spawnPosition;

    #endregion


    #region Properties

    public CarDriver CarDriver
    {
        get
        {
            return currentCarDriver;
        }
    }

    #endregion





    public override void OnDeserialize(NetworkReader reader, bool initialState)
    {
        base.OnDeserialize(reader, initialState);
    }


    private void Awake()
    {
        OnPlayerCreated?.Invoke(this);
        Initialize();
    }


    private void OnDestroy()
    {
        OnPlayerDestroy?.Invoke(this);
    }

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
        playerID = GameManager.Instance.UserData.user_id;
        OnPlayerIDChanged(playerID);
        currentCarDriver.IsLocalPlayer = true;
    }


    public override void OnStartClient()
    {
        OnPlayerIDChanged(playerID);

        playerPoints = initialLifes;
        OnPlayerPointsChanged(playerPoints);
    }


    private void CurrentCarDriver_OnEnterTrigger(TriggerType obj)
    {
        if (TriggerType.Trail == obj)
        {
            playerPoints--;
            OnPlayerPointsChanged(playerPoints);
            Respawn();
        }
    }
    


    [SyncVar(hook = "OnPlayerIDChanged")] public int playerID;
    [SyncVar(hook = "OnPlayerPointsChanged")] public int playerPoints;

    void OnPlayerIDChanged(int newValue)
    {
        CmdSetIdOnClients(newValue);
    }

    void OnPlayerPointsChanged(int newValue)
    {
        CmdSetPointsOnClients(newValue);
    }


    // function called on the server.
    [Command]
    private void CmdSetIdOnClients(int idValue)
    {
        RpcSetIdOnClients(idValue);
    }
    [Command]
    private void CmdSetPointsOnClients(int points)
    {
        RpcSetPointsOnClients(points);
    }

    [ClientRpc]
    private void RpcSetIdOnClients(int idValue)
    {
        playerID = idValue;
    }
    [ClientRpc]
    private void RpcSetPointsOnClients(int points)
    {
        playerPoints = points;
    }

}
