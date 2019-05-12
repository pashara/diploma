using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkReader = UnityEngine.Networking.NetworkReader;
using NetworkTransformChild = UnityEngine.Networking.NetworkTransformChild;
using System;


public partial class Player : CustomNetworkBehaviour
{
    
    public static event Action<Player, int> OnIdSeted;
    #region Fields

    const int initialLifes = 4;
    public static event Action<Player> OnPlayerCreated;
    public static event Action<Player> OnPlayerDestroy;
    public static event Action<Player, Player> OnPlayerKilled;
    public static event Action<int> OnTimerChanged;
    [SerializeField] NetworkTransformChild networkTransformChild;

    [SerializeField] CarDriver prefab;

    [SerializeField] CarDriver currentCarDriver;
    [SerializeField] int seconds = 360;

    Vector3 spawnPosition;
    bool shouldRespawn = false;
    bool shouldDecreasePoints = false;

    float timer;
    int colorIndex = -1;


    #endregion


    #region Properties
    
    public int PlayerID
    {
        get;
        set;
    } = -1;


    public int PlayerPoints
    {
        get;
        set;
    } = -1;


    public CarDriver CarDriver
    {
        get
        {
            return currentCarDriver;
        }
    }


    public int LastKilledByPlayerId
    {
        get;
        private set;
    }


    public int ColorIndex
    {
        get
        {
            return colorIndex;
        }
        set
        {
            if (colorIndex != value)
            {
                colorIndex = value;
                currentCarDriver?.carTrail.UpdateColor();
            }
        }
    }

    #endregion




    private void Awake()
    {
        OnPlayerCreated?.Invoke(this);
        Initialize();
    }

    public override void OnDeserialize(NetworkReader reader, bool initialState)
    {
        base.OnDeserialize(reader, initialState);
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
            CurrentCarDriver_OnEnterTriggerEvent(TriggerType.Trail, null);
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
            if (shouldDecreasePoints)
            {
                PlayerPoints--;
                OnPlayerPointsChanged(PlayerPoints);
            }

            if (shouldRespawn)
            {
                Respawn();
            }
        }

        float deltaTime = Time.deltaTime;
        currentCarDriver?.CustomUpdate(deltaTime);
        
        if (isServer)
        {
            timer -= Time.deltaTime;
            int inSeconds = Mathf.Max((int)0, Mathf.CeilToInt(timer));
            RpcUpdateTimer(inSeconds);
            OnTimerChanged?.Invoke(inSeconds);
        }
    }
    

    

    private void OnDestroy()
    {
        OnPlayerDestroy?.Invoke(this);
    }


    public void Initialize()
    {
        timer = seconds;
        LastKilledByPlayerId = -1;
        shouldRespawn = false;
        shouldDecreasePoints = false;
        spawnPosition = transform.position;
        currentCarDriver = Instantiate<CarDriver>(prefab);
        currentCarDriver.IsLocalPlayer = IsLocalPlayer;
        currentCarDriver.transform.SetParent(transform);
        currentCarDriver.transform.localPosition = Vector3.zero;
        networkTransformChild.target = currentCarDriver.MovablePart;
        currentCarDriver.Initialize(this);

        currentCarDriver.OnEnterTriggerEvent += CurrentCarDriver_OnEnterTriggerEvent;

        OnPlayerCreated?.Invoke(this);
    }


    public void Deinitialize()
    {
        currentCarDriver.Deinitialize();
        Destroy(currentCarDriver);
        currentCarDriver = null;

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
        Player killer = MyNetworkManager.Instance.Players.Find((item) => item.instance.PlayerID == LastKilledByPlayerId)?.instance;
        if (!isFromIntenet)
        {
            CmdOnPlayerRestarted(LastKilledByPlayerId);
        }
        OnPlayerKilled?.Invoke(killer, this);
    }


    public override void OnStartLocalPlayer()
    {
        PlayerID = GameManager.Instance.UserData.user_id;
        currentCarDriver.IsLocalPlayer = IsLocalPlayer;
        OnIdSeted?.Invoke(this, PlayerID);
        UpdateAllDataFromClient();
    }


    public override void OnStartClient()
    {
        PlayerPoints = initialLifes;
        OnPlayerPointsChanged(PlayerPoints);
    }


    private void CurrentCarDriver_OnEnterTriggerEvent(TriggerType obj, ITrigger trigger)
    {
        if (IsLocalPlayer)
        {
            if (TriggerType.Trail == obj)
            {
                if (!shouldRespawn)
                {
                    shouldRespawn = true;
                    shouldDecreasePoints = true;
                    LastKilledByPlayerId = -1;

                    if (trigger != null)
                    {
                        var opponentPlayer = trigger.GameObject.GetComponent<Player>();
                        if (opponentPlayer != null)
                        {
                            LastKilledByPlayerId = opponentPlayer.PlayerID;
                        }
                    }
                }
            }
        }
    }


    
}
