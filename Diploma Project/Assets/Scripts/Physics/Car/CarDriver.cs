using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

[Serializable]
public class AxleInfo
{
    public WheelCollider leftWheelCollider;
    public WheelCollider rightWheelCollider;
    public GameObject leftWheelMesh;
    public GameObject rightWheelMesh;
    public bool motor;
    public bool steering;
}


public class CarDriver : MonoBehaviour, ITriggerable
{
    #region Events

    public event Action<TriggerType, ITrigger> OnEnterTriggerEvent;

    #endregion



    #region Fields

    [SerializeField] Transform centerOfMassTransform;
    [SerializeField] Rigidbody currentRigidbody;


    [SerializeField] float neededVelocityMagnitude;
    [SerializeField] float invisibleTime;
    [SerializeField] float rpmMinVal;
    [SerializeField] float rpmMaxVal;
    [SerializeField] float momentMinVal;
    [SerializeField] float momentMaxVal;
    [SerializeField] AnimationCurve momentByRpmCurve;

    [SerializeField] List<AxleInfo> axleInfos;
    [SerializeField] float maxMotorTorque;
    [SerializeField] float maxSteeringAngle;
    [SerializeField] float brakeTorque;
    [SerializeField] float decelerationForce;
    [SerializeField] public CarTrail carTrail;

    [SerializeField] public Transform emmitTrailTransfom;
    [SerializeField] CarTrailListner carTrailListner;

    [SerializeField] GameObject cameraObject;
    Vector3 spawnPosition;
    float invisibleTimer;

    float periodSvrRpc = 0.02f; //как часто сервер шлёт обновление картинки клиентам, с.
    float timeSvrRpcLast = 0; //когда последний раз сервер слал обновление картинки

    bool isInitialized;
    Player owner;

    #endregion

    float momentByRpm(float rpm)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Go debug");
        }
        float sign = Mathf.Sign(rpm);
        float clampedValue = Mathf.Clamp(Mathf.Abs(rpm), rpmMinVal, rpmMaxVal);

        float factor = (Mathf.Approximately(rpmMaxVal, rpmMinVal)) ? 1f : (clampedValue - rpmMinVal) / (rpmMaxVal - rpmMinVal);
        float evalueatedFactor = momentByRpmCurve.Evaluate(factor);
        // Debug.Log(evalueatedFactor);
        return Mathf.Lerp(momentMinVal, momentMaxVal, evalueatedFactor);
    }


    bool isLocalPlayer;

    #region Properties

    public bool IsLocalPlayer
    {
        get
        {
            return isLocalPlayer;
        }
        set
        {
            isLocalPlayer = value;
            cameraObject.SetActive(isLocalPlayer);
        }
    }


    public Transform MovablePart
    {
        get
        {
            return currentRigidbody.transform;
        }
    }


    public Rigidbody MainRigidBody
    {
        get
        {
            return currentRigidbody;
        }
    }

    #endregion

    void Awake()
    {
        spawnPosition = currentRigidbody.transform.position;
    }

    void OnEnable()
    {
        currentRigidbody.centerOfMass = centerOfMassTransform.localPosition;
    }


    public void ApplyLocalPositionToVisuals(AxleInfo axleInfo)
    {
        Vector3 position;
        Quaternion rotation;
        axleInfo.leftWheelCollider.GetWorldPose(out position, out rotation);
        axleInfo.leftWheelMesh.transform.position = position;
        axleInfo.rightWheelCollider.GetWorldPose(out position, out rotation);
        axleInfo.rightWheelMesh.transform.position = position;

        axleInfo.rightWheelMesh.transform.rotation = rotation * Quaternion.AngleAxis(-90, Vector3.forward) * Quaternion.AngleAxis(-90, Vector3.up);
        axleInfo.leftWheelMesh.transform.rotation = rotation * Quaternion.AngleAxis(-90, Vector3.forward) * Quaternion.AngleAxis(-90, Vector3.up);

    }


    void FixedUpdate()
    {
        if (isInitialized)
        {
            if (IsLocalPlayer)
            {
                float motor = -maxMotorTorque * InputAdapter.Instance.VerticalInput();
                float steering = maxSteeringAngle * InputAdapter.Instance.HorizontalInput();
                for (int i = 0; i < axleInfos.Count; i++)
                {
                    if (axleInfos[i].steering)
                    {
                        Steering(axleInfos[i], steering);
                    }
                    if (axleInfos[i].motor)
                    {
                        Acceleration(axleInfos[i], motor);
                    }
                    if (Input.GetKey(KeyCode.Space))
                    {
                        Brake(axleInfos[i]);
                    }
                    ApplyLocalPositionToVisuals(axleInfos[i]);
                }
                

                carTrailListner.CheckCollision();
                
                invisibleTimer += Time.fixedDeltaTime;
            }
        }
    }


    


    #region Public methods

    public void Initialize(Player owner)
    {
        this.owner = owner;
        isInitialized = true;
        carTrail.Initialize(emmitTrailTransfom, owner);
    }
    


    public void Deinitialize()
    {
        carTrail.DestroyTrail();
        isInitialized = false;
    }


    public void CustomUpdate(float deltaTime)
    {
        carTrail.CustomUpdate(deltaTime);
    }
    #endregion

    #region LocalPlayerInfo

    private void Acceleration(AxleInfo axleInfo, float motor)
    {
        if (motor != 0f)
        {
            axleInfo.leftWheelCollider.brakeTorque = 0;
            axleInfo.rightWheelCollider.brakeTorque = 0;

            axleInfo.leftWheelCollider.motorTorque = (Mathf.Sign(motor)) * momentByRpm(axleInfo.leftWheelCollider.rpm); //motor
            axleInfo.rightWheelCollider.motorTorque = (Mathf.Sign(motor)) * momentByRpm(axleInfo.rightWheelCollider.rpm); //motor
        }
        else
        {
            Deceleration(axleInfo);
        }
    }

    private void Deceleration(AxleInfo axleInfo)
    {
        axleInfo.leftWheelCollider.brakeTorque = decelerationForce;
        axleInfo.rightWheelCollider.brakeTorque = decelerationForce;
    }

    private void Steering(AxleInfo axleInfo, float steering)
    {
        axleInfo.leftWheelCollider.steerAngle = steering;
        axleInfo.rightWheelCollider.steerAngle = steering;
    }

    private void Brake(AxleInfo axleInfo)
    {
        axleInfo.leftWheelCollider.brakeTorque = brakeTorque;
        axleInfo.rightWheelCollider.brakeTorque = brakeTorque;
    }

    #endregion


    #region ITriggerable interface

    public void OnEnterTrigger(TriggerType triggerType, ITrigger triggerObject)
    {
        if (IsLocalPlayer)
        {
            switch (triggerType)
            {
                case TriggerType.Trail:
                    OnEnterTriggerEvent?.Invoke(TriggerType.Trail, triggerObject);
                    break;

                default:
                    break;
            }
        }
    }

    #endregion

}