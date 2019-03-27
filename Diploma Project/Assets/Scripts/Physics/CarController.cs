using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CarController : MonoBehaviour
{
    Vector3 spawnPosition;
    [SerializeField] Transform centerOfMassTransform;
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] WheelCollider FLWheel;
    [SerializeField] WheelCollider FRWheel;
    [SerializeField] WheelCollider BLWheel;
    [SerializeField] WheelCollider BRWheel;

    Vector3 prevLFVheel, prevLRVheel;

    [SerializeField] Transform FLWheelTransform;
    [SerializeField] Transform FRWheelTransform;
    [SerializeField] Transform BLWheelTransform;
    [SerializeField] Transform BRWheelTransform;

    [SerializeField] float maxSteerAngle;
    [SerializeField] float motorForce;
    private float m_horizontalInput;
    private float m_verticalInput;
    private float m_steeringAngle;


    void Awake()
    {
        spawnPosition = rigidbody.transform.position;
    }

    void OnEnable()
    {
        rigidbody.centerOfMass = centerOfMassTransform.localPosition;
    }


    
    // bool isSpacePressed = false;

    // public void GetInput()
    // {
    //     m_horizontalInput = Input.GetAxis("Horizontal");
    //     m_verticalInput = Input.GetAxis("Vertical");

    //     isSpacePressed = (Input.GetKey(KeyCode.Space));
    //     if (isSpacePressed)
    //     {
    //         m_verticalInput = 0f;
    //         // rigidbody.velocity = Vector3.zero;

    //         BLWheel.motorTorque = 0f;
    //         BRWheel.motorTorque = 0f;
            
    //         FLWheel.brakeTorque = 1000f;
    //         FRWheel.brakeTorque = 1000f;
    //         BLWheel.brakeTorque = 1000f;
    //         BRWheel.brakeTorque = 1000f;
    //     }
    //     else
    //     {
            
    //         FLWheel.brakeTorque = 0f;
    //         FRWheel.brakeTorque = 0f;
    //         BLWheel.brakeTorque = 0f;
    //         BRWheel.brakeTorque = 0f;
    //     }

    //     if (Input.GetKeyDown(KeyCode.R))
    //     {
    //         rigidbody.transform.position = spawnPosition;
    //         rigidbody.velocity = Vector3.zero;
    //         rigidbody.transform.localRotation = Quaternion.identity;

    //     }
    // }

    // private void Steer()
    // {
    //     m_steeringAngle = maxSteerAngle * m_horizontalInput;
    //     FLWheel.steerAngle = m_steeringAngle;
    //     FRWheel.steerAngle = m_steeringAngle;
    // }

    // private void Accelerate()
    // {
    //     if (!isSpacePressed)
    //     {
    //         BLWheel.motorTorque = -m_verticalInput * motorForce;
    //         BRWheel.motorTorque = -m_verticalInput * motorForce;
    //     }
    //     else
    //     {
    //         BLWheel.motorTorque = 0f;
    //         BRWheel.motorTorque = 0f;
    //     }
    // }

    // private void UpdateWheelPoses()
    // {
    //     UpdateWheelPose(FRWheel, FRWheelTransform);
    //     UpdateWheelPose(FLWheel, FLWheelTransform);
    //     UpdateWheelPose(BRWheel, BRWheelTransform);
    //     UpdateWheelPose(BLWheel, BLWheelTransform);
    // }

    // private void UpdateWheelPose(WheelCollider _collider, Transform _transform)
    // {
    //     Vector3 _pos = _transform.position;
    //     Quaternion _quat = _transform.rotation;

    //     _collider.GetWorldPose(out _pos, out _quat);

    //     _transform.position = _pos;
    //     _transform.rotation = _quat * Quaternion.AngleAxis(-90, Vector3.forward) * Quaternion.AngleAxis(-90, Vector3.up);
    // }
    // Vector3 currentPosition;
    // private void FixedUpdate()
    // {
    //     GetInput();
    //     Steer();
    //     Accelerate();
    //     UpdateWheelPoses();
        
    //     Debug.Log((rigidbody.transform.position - currentPosition).magnitude / Time.fixedDeltaTime);
        
    //     Debug.DrawLine(prevLFVheel, FLWheel.transform.position, Color.green, 100f);
    //     Debug.DrawLine(prevLRVheel, FRWheel.transform.position, Color.red, 100f);
        
    //     prevLFVheel = FLWheel.transform.position;
    //     prevLRVheel = FRWheel.transform.position;
    //     currentPosition = rigidbody.transform.position;





    // WheelHit hit = new WheelHit();
    // float travelL = 1.0f;
    // float travelR = 1.0f;
 
    // var groundedL = WheelL.GetGroundHit(hit);
    // if (groundedL)
    //     travelL = (-WheelL.transform.InverseTransformPoint(hit.point).y - WheelL.radius) / WheelL.suspensionDistance;
 
    // var groundedR = WheelR.GetGroundHit(hit);
    // if (groundedR)
    //     travelR = (-WheelR.transform.InverseTransformPoint(hit.point).y - WheelR.radius) / WheelR.suspensionDistance;
 
    // var antiRollForce = (travelL - travelR) * AntiRoll;
 
    // if (groundedL)
    //     rigidbody.AddForceAtPosition(WheelL.transform.up * -antiRollForce,
    //            WheelL.transform.position);  
    // if (groundedR)
    //     rigidbody.AddForceAtPosition(WheelR.transform.up * antiRollForce,
    //            WheelR.transform.position);  
    // }

}
