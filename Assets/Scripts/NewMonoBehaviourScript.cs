using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    Rigidbody m_Rigidbody;
    public float motoreForce = 100f;
    public float brakeForce = 1000f;
    public float maxSteerAngle = 30f;

    public WheelCollider frontLeftWheelCollider;
    public WheelCollider rearLeftWheelCollider;
    public WheelCollider frontRightWheelCollider;
    public WheelCollider rearRightWheelCollider;

    public Transform frontLeftWheelTransform;
    public Transform rearLeftWheelTransform;
    public Transform frontRightWheelTransform;
    public Transform rearRightWheelTransform;

    private float horizontalInput;
    private float verticalInput;
    private float currentSteerAngle;
    private float cureentBrakeForce;
    private bool isBraking;


    private void Update()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }
    private void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        isBraking = Input.GetKey(KeyCode.Space);
    }

    private void HandleMotor()
    {
        frontLeftWheelCollider.motorTorque = verticalInput * motoreForce;
        frontRightWheelCollider.motorTorque = verticalInput * motoreForce;

        cureentBrakeForce = isBraking ? brakeForce : 0f;
        ApplyBraking();
    }

    private void ApplyBraking()
    {
        frontLeftWheelCollider.brakeTorque = cureentBrakeForce;
        frontRightWheelCollider.brakeTorque = cureentBrakeForce;
        rearLeftWheelCollider.brakeTorque = cureentBrakeForce;
        rearRightWheelCollider.brakeTorque = cureentBrakeForce;
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
    }
}
