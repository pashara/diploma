using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


public class CarDriver : MonoBehaviour
{

    Vector3 spawnPosition;
    
    [SerializeField] Transform centerOfMassTransform;
    [SerializeField] Rigidbody rigidbody;

    [SerializeField] float rpmMinVal;
    [SerializeField] float rpmMaxVal;
    [SerializeField] float momentMinVal;
    [SerializeField] float momentMaxVal;
    [SerializeField] AnimationCurve momentByRpmCurve;

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
        Debug.Log(evalueatedFactor);
        return Mathf.Lerp(momentMinVal, momentMaxVal, evalueatedFactor);
    }
    public List<AxleInfo> axleInfos;
    public float maxMotorTorque;
    public float maxSteeringAngle;
    public float brakeTorque;
    public float decelerationForce;


    void Awake()
    {
        spawnPosition = rigidbody.transform.position;
    }


    void OnEnable()
    {
        rigidbody.centerOfMass = centerOfMassTransform.localPosition;
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
    void FixedUpdate ()
    {
        float motor = -maxMotorTorque * Input.GetAxis ("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis ("Horizontal");
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
            rigidbody.transform.position = spawnPosition;
            rigidbody.velocity = Vector3.zero;
            rigidbody.transform.localRotation = Quaternion.identity;

        }
    }
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


    float time = 0f;
    string result = "";
    float outHeight = 0f;
    
    void OnGUI () {

        if (time > 0.2f)
        {
            result = "";
            float oneHeight = 50f;
            outHeight = 0f;
            // Make a background box
            axleInfos.ForEach((item) =>
            {
                result += $"{item.leftWheelCollider.name}:{(int)item.leftWheelCollider.rpm,6} \n";
                result += $"{item.rightWheelCollider.name}:{(int)item.rightWheelCollider.rpm,6} \n";
                outHeight += 2f * oneHeight;
            });
            time = 0f;
        }
        GUI.Box(new Rect(10,10,1000,outHeight), result);
        
        time += Time.deltaTime;
    }

    Vector3? prevPosition;
    void Update()
    {
        Vector3 newPosition = centerOfMassTransform.position;
        // if(prevPosition != null)
        // {
        //     Debug.DrawLine(prevPosition.Value, newPosition, Color.red, 100f);
        // }
        
        prevPosition = newPosition;
    }
}