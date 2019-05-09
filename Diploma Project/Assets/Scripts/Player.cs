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
    bool shouldRespawn = false;
    bool shouldDecreasePoints = false;

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

    private void OnEnable()
    {
        SimpleGui.OnUp += SimpleGui_OnUp;
    }

    private void OnDisable()
    {
        SimpleGui.OnUp += SimpleGui_OnUp;
    }

    private void SimpleGui_OnUp(GuiButtonTypeTEMP a)
    {
        if (a == GuiButtonTypeTEMP.Reset)
        {
            CurrentCarDriver_OnEnterTriggerEvent(TriggerType.Trail);
        }
        if (a == GuiButtonTypeTEMP.Disconnect)
        {
            GameManager.Instance.NetworkManager.StopClient();
            GameManager.Instance.NetworkManager.StopHost();
        }
    }

    private void Update()
    {
        if (IsLocalPlayer)
        {
            if (shouldRespawn)
            {
                Respawn();
            }

            if (shouldDecreasePoints)
            {
                playerPoints--;
                OnPlayerPointsChanged(playerPoints);
            }
        }

        float deltaTime = Time.deltaTime;
        currentCarDriver.CustomUpdate(deltaTime);
    }

    public void UpdateAllData()
    {
        CmdSetIdOnClients(playerID);
        CmdSetPointsOnClients(playerPoints);
    }
    

    private void OnDestroy()
    {
        OnPlayerDestroy?.Invoke(this);
    }

    public void Initialize()
    {
        shouldRespawn = false;
        shouldDecreasePoints = false;
        spawnPosition = transform.position;
        currentCarDriver = Instantiate<CarDriver>(prefab);
        currentCarDriver.transform.SetParent(transform);
        currentCarDriver.transform.localPosition = Vector3.zero;
        networkTransformChild.target = currentCarDriver.MovablePart;
        currentCarDriver.Initialize(this);
        networkIdentity.enabled = true;
        networkTransform.enabled = true;
        networkTransformChild.enabled = true;

        currentCarDriver.OnEnterTriggerEvent += CurrentCarDriver_OnEnterTriggerEvent;
    }


    public void Deinitialize()
    {
        currentCarDriver.Deinitialize();
        Destroy(currentCarDriver);
        currentCarDriver = null;
        networkIdentity.enabled = false;
        networkTransform.enabled = false;
        networkTransformChild.enabled = false;

        currentCarDriver.OnEnterTriggerEvent -= CurrentCarDriver_OnEnterTriggerEvent;
    }


    public void Respawn(bool isFromIntenet = false)
    {
        shouldRespawn = false;
        shouldDecreasePoints = false;
        currentCarDriver.MainRigidBody.transform.position = spawnPosition;
        currentCarDriver.MainRigidBody.transform.rotation = Quaternion.identity;
        currentCarDriver.MainRigidBody.velocity = Vector3.zero;
        currentCarDriver.MainRigidBody.angularDrag = 0f;
        currentCarDriver.MainRigidBody.angularVelocity = Vector3.zero;
        

        currentCarDriver.carTrail.DestroyTrail();
        currentCarDriver.carTrail.Initialize(currentCarDriver.emmitTrailTransfom, this);
        if (!isFromIntenet)
        {
            CmdOnPlayerRestarted();
        }
    }


    public override void OnStartLocalPlayer()
    {
        playerID = GameManager.Instance.UserData.user_id;
        OnIdSeted?.Invoke(this, playerID);
        UpdateAllData();
        currentCarDriver.IsLocalPlayer = true;
    }


    public override void OnStartClient()
    {
        playerPoints = initialLifes;
        OnPlayerPointsChanged(playerPoints);
    }


    private void CurrentCarDriver_OnEnterTriggerEvent(TriggerType obj)
    {
        if (IsLocalPlayer)
        {
            if (TriggerType.Trail == obj)
            {
                shouldRespawn = true;
                shouldDecreasePoints = true;
            }
        }
    }

    public static event Action<Player, int> OnIdSeted;



    [SyncVar] public int playerID = -1;
    [SyncVar] public int playerPoints = -1;
    

    void OnPlayerPointsChanged(int newValue)
    {
        CmdSetPointsOnClients(newValue);
    }

    
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


    [Command]
    private void CmdOnPlayerRestarted()
    {
        RpcOnPlayerRespawned();
    }




    [ClientRpc]
    private void RpcSetIdOnClients(int idValue)
    {
        if (!IsLocalPlayer)
        {
            Debug.Log("Update ID " + idValue);
            playerID = idValue;
            OnIdSeted?.Invoke(this, playerID);
        }
    }
    [ClientRpc]
    private void RpcSetPointsOnClients(int points)
    {
        if (!IsLocalPlayer)
        {
            playerPoints = points;
        }
    }


    [ClientRpc]
    private void RpcOnPlayerRespawned()
    {
        if (!IsLocalPlayer)
        {
            Respawn(true);
        }
    }

}
