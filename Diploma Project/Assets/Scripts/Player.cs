using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;




public class Player : Photon.PunBehaviour
{
    #region Fields

    const int initialLifes = 4;
    public static event Action<Player> OnPlayerCreated;
    public static event Action<Player> OnPlayerDestroy;
    
    [SerializeField] CarDriver prefab;

    [SerializeField] CarDriver currentCarDriver;

    Vector3 spawnPosition;
    float syncDuration;

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


    void Awake()
    {
        Initialize();
        OnPlayerCreated?.Invoke(this);
    }


    void Update()
    {
        float detlaTime = Time.deltaTime;
        if (!photonView.isMine)
        {
            syncDuration += detlaTime * 15f;
            float factor = syncDuration;
            if (Vector3.Distance(oldPosition, newPosition) > 2f)
            {
                factor = 1f;
            }
            
            CarDriver.MainRigidBody.position = Vector3.Lerp(oldPosition, newPosition, factor);
            CarDriver.MainRigidBody.velocity = Vector3.Lerp(oldVelocity, newVelocity, factor);
            CarDriver.MainRigidBody.rotation = Quaternion.Lerp(oldRotation, newRotation, factor);
        }
    }


    void OnDestroy()
    {
        OnPlayerDestroy?.Invoke(this);
    }

    


    public void Initialize()
    {
        Debug.Log("Initialize");
        spawnPosition = transform.position;
        currentCarDriver = Instantiate<CarDriver>(prefab);
        currentCarDriver.transform.SetParent(transform);
        currentCarDriver.transform.localPosition = Vector3.zero;
        currentCarDriver.Initialize();

        currentCarDriver.OnEnterTrigger += CurrentCarDriver_OnEnterTrigger;


        if (photonView.isMine)
        {
            playerID = GameManager.Instance.UserData.user_id;
            currentCarDriver.IsLocalPlayer = true;
        }
    }


    public void Deinitialize()
    {
        currentCarDriver.Deinitialize();
        Destroy(currentCarDriver);
        currentCarDriver = null;

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


    bool isSync = false;
    Vector3 oldPosition = Vector3.zero;
    Vector3 newPosition = Vector3.zero;

    Vector3 oldVelocity = Vector3.zero;
    Vector3 newVelocity = Vector3.zero;

    Quaternion oldRotation = Quaternion.identity;
    Quaternion newRotation = Quaternion.identity;

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        Vector3 position = CarDriver.MainRigidBody.position;
        Quaternion rotation = CarDriver.MainRigidBody.rotation;
        Vector3 velocity = CarDriver.MainRigidBody.velocity;

        stream.Serialize(ref position);
        stream.Serialize(ref rotation);

        if (stream.isReading)
        {
            isSync = true;
            syncDuration = 0f;
            oldPosition = CarDriver.MainRigidBody.position;
            oldRotation = CarDriver.MainRigidBody.rotation;
            oldVelocity = CarDriver.MainRigidBody.velocity;

            newPosition = position;
            newRotation = rotation;
            newVelocity = velocity;
        }

    }


    private void CurrentCarDriver_OnEnterTrigger(TriggerType obj)
    {
        if (TriggerType.Trail == obj)
        {
            playerPoints--;
            //OnPlayerPointsChanged(playerPoints);
            Respawn();
        }
    }
    


    public int playerID;
    public int playerPoints;
    

}
