using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheelCollider;
    public WheelCollider rightWheelCollider;
    public GameObject leftWheelMesh;
    public GameObject rightWheelMesh;
    public bool motor;
    public bool steering;
}


public class CarDriver : CustomNetworkBehaviour, ITriggerable
{
    #region Helpers
    

    #endregion



    #region Fields
    
    [SerializeField] Transform centerOfMassTransform;
    [SerializeField] Rigidbody rigidbody;

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
    [SerializeField] CarTrail carTrail;
    [SerializeField] CarTrailListner carTrailListner;

    [SerializeField] GameObject cameraObject;
    Vector3 spawnPosition;
    float invisibleTimer;

    float periodSvrRpc = 0.02f; //как часто сервер шлёт обновление картинки клиентам, с.
    float timeSvrRpcLast = 0; //когда последний раз сервер слал обновление картинки

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


    void Awake()
    {
        spawnPosition = rigidbody.transform.position;
        carTrail.Initialize(carTrail.transform);
    }


    void OnEnable()
    {
        rigidbody.centerOfMass = centerOfMassTransform.localPosition;
        SimpleGui.OnUp += (a)=>
        {
            if (a == GuiButtonTypeTEMP.Reset)
            {
                Respawn();
            }
        };
    }
    public void ApplyLocalPositionToVisuals (AxleInfo axleInfo)
    {
        Vector3 position;
        Quaternion rotation;
        axleInfo.leftWheelCollider.GetWorldPose (out position, out rotation);
        axleInfo.leftWheelMesh.transform.position = position;
        axleInfo.rightWheelCollider.GetWorldPose (out position, out rotation);
        axleInfo.rightWheelMesh.transform.position = position;
        
        axleInfo.rightWheelMesh.transform.rotation = rotation * Quaternion.AngleAxis(-90, Vector3.forward) * Quaternion.AngleAxis(-90, Vector3.up);
        axleInfo.leftWheelMesh.transform.rotation = rotation * Quaternion.AngleAxis(-90, Vector3.forward) * Quaternion.AngleAxis(-90, Vector3.up);

    }


    public override void OnStartLocalPlayer()
    {
        cameraObject.SetActive(IsLocalPlayer);
    }


    void FixedUpdate ()
    {
        if (IsLocalPlayer)
        {
            float motor = -maxMotorTorque * InputAdapter.Instance.VerticalInput();// Input.GetAxis ("Vertical");
            float steering = maxSteeringAngle * InputAdapter.Instance.HorizontalInput();// Input.GetAxis ("Horizontal");
            for (int i = 0; i < axleInfos.Count; i++)
            {
                if (axleInfos [i].steering)
                {
                    Steering (axleInfos [i], steering);
                }
                if (axleInfos [i].motor)
                {
                    Acceleration (axleInfos [i], motor);
                }
                if (Input.GetKey (KeyCode.Space))
                {
                    Brake (axleInfos [i]);
                } 
                ApplyLocalPositionToVisuals (axleInfos [i]);
            }


            if (Input.GetKeyDown(KeyCode.R))
            {
                Respawn();
            }

            CmdUpdatePlayerPositionSettings();
            carTrailListner.CheckCollision();

            if (invisibleTimer >= invisibleTime)
            {
                if(rigidbody.velocity.magnitude < neededVelocityMagnitude)
                {
                    Respawn();
                }
            }
            invisibleTimer += Time.fixedDeltaTime;
        }
        else if (CurrentClientType == ClientType.Server)
        {

            if (timeSvrRpcLast + periodSvrRpc < Time.time)
            {
                RpcSetPlayerSettings(rigidbody.position, rigidbody.rotation.eulerAngles, rigidbody.velocity);
                timeSvrRpcLast = Time.time;
            }
        }
    }


    Vector3 currentPlayerPosition;
    Vector3 currentPlayerRotation;
    Vector3 currentPlayerVelocity;


    [Command(channel = 0)]
    void CmdUpdatePlayerPositionSettings()
    {
        if (CurrentClientType == ClientType.Server)
        {
            currentPlayerPosition = rigidbody.position;
            currentPlayerRotation = rigidbody.rotation.eulerAngles;
            currentPlayerVelocity = rigidbody.velocity;
        }
    }


    // [ClientRpc(channel = 0)]
    void RpcSetPlayerSettings(Vector3 position, Vector3 rotation, Vector3 velocity)
    {
        Debug.Log("RpcSetPlayerSettings");
        if (CurrentClientType == ClientType.Client)
        {
            Debug.Log("RpcSetPlayerSettings_Client");
            rigidbody.position = position;
            rigidbody.rotation =Quaternion.Euler(rotation);
            rigidbody.velocity = velocity;
        }
    }


    void Respawn()
    {
        invisibleTimer = 0f;
        carTrail.DestroyTrail();
        carTrail.Initialize(carTrail.transform);
        rigidbody.position = spawnPosition;
        rigidbody.rotation = Quaternion.identity;
        rigidbody.velocity = Vector3.zero;
    }




    #region LocalPlayerInfo

    private void Acceleration (AxleInfo axleInfo, float motor)
    {
        if (motor != 0f)
        {
            axleInfo.leftWheelCollider.brakeTorque = 0;
            axleInfo.rightWheelCollider.brakeTorque = 0;

            axleInfo.leftWheelCollider.motorTorque = (Mathf.Sign(motor)) * momentByRpm(axleInfo.leftWheelCollider.rpm);//motor
            axleInfo.rightWheelCollider.motorTorque = (Mathf.Sign(motor)) * momentByRpm(axleInfo.rightWheelCollider.rpm);//motor
        } else
        {
            Deceleration (axleInfo);
        }
    }


    private void Deceleration (AxleInfo axleInfo)
    {
        axleInfo.leftWheelCollider.brakeTorque = decelerationForce;
        axleInfo.rightWheelCollider.brakeTorque = decelerationForce;
    }


    private void Steering (AxleInfo axleInfo, float steering)
    {
        axleInfo.leftWheelCollider.steerAngle = steering;
        axleInfo.rightWheelCollider.steerAngle = steering;
    }


    private void Brake (AxleInfo axleInfo)
    {
        axleInfo.leftWheelCollider.brakeTorque = brakeTorque;
        axleInfo.rightWheelCollider.brakeTorque = brakeTorque;
    }

    #endregion


    
    #region Debug GUI

    // float time = 0f;
    // string result = "";
    // float outHeight = 0f;
    
    // void OnGUI () {

    //     if (time > 0.2f)
    //     {
    //         result = "";
    //         float oneHeight = 50f;
    //         outHeight = 0f;
    //         // Make a background box
    //         axleInfos.ForEach((item) =>
    //         {
    //             result += $"{item.leftWheelCollider.name}:{(int)item.leftWheelCollider.rpm,6} \n";
    //             result += $"{item.rightWheelCollider.name}:{(int)item.rightWheelCollider.rpm,6} \n";
    //             outHeight += 2f * oneHeight;
    //         });
    //         time = 0f;
    //     }
    //     GUI.Box(new Rect(10,10,1000,outHeight), result);
        
    //     time += Time.deltaTime;
    // }

    #endregion



    #region ITriggerable interface

    public void OnTriggerEnter(TriggerType triggerType, ITrigger triggerObject)
    {
        switch(triggerType)
        {
            case TriggerType.Trail:
                Respawn();
            break;

            default:
            break;
        }
    }

    #endregion

}