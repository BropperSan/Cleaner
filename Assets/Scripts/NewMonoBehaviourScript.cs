using LogitechG29.Sample.Input;
using System.Collections;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public InputControllerReader g29Input;
    public float motoreForce = 100f;
    public float brakeForce = 1000f;
    public float maxSteerAngle = 30f;
    public float maxSpeed = 12f;
    public Vector3 centerOfMassOffset = new Vector3(0, -0.5f, 0);
    public Vector3 pawRotationAxis = new Vector3(0, 0, 90);
    public Vector3 mirrorFoldAxis = new Vector3(0, 0, 45);
    public Vector3 brushLocalAxis = new Vector3(0, 1, 0);
    public Vector3 flashRotationAxis = new Vector3(-30, 0, 0);

    public float pawRotSpeed = 100f;
    public float mirrorRotSpeed = 100f;
    public float brushSpinSpeed = 500f;
    public float flashRotSpeed = 200f;
    public float vibrationStrength = 2.0f;

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
    private float currentBrakeForce;
    private bool isBraking;
    private Rigidbody _rb;

    public Transform leftPaw;
    public Transform leftPawHead;
    public Transform rightPaw;
    public Transform rightPawHead;
    public Transform leftFlashLight;
    public Light leftLightSource;
    public Transform rightFlashLight;
    public Light rightLightSource;
    public Transform leftMirror;
    public Transform rightMirror;
    public Transform leftVacuum;
    public Transform rightVacuum;

    private Quaternion lPawStart, lPawEnd, rPawStart, rPawEnd;
    private bool arePawsOpen = false;
    private Coroutine pawCoroutine;

    private Quaternion lMirStart, lMirEnd, rMirStart, rMirEnd;
    private bool areMirrorsFolded = false;
    private Coroutine mirrorCoroutine;

    private Quaternion lFlashStart, lFlashEnd, rFlashStart, rFlashEnd;
    private Coroutine flashCoroutine;
    private bool areFlashLightsUp = false;
    private bool isLightOn = false;

    private bool isBrushing = false;
    public bool IsBrushingActive => isBrushing;

    private Quaternion lVacStart, rVacStart;
    private bool isVacuuming = false;

    public int driveDirection = 0;
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.centerOfMass = centerOfMassOffset;
        GetComponent<Rigidbody>().centerOfMass = centerOfMassOffset;
        if (leftPaw && rightPaw)
        {
            lPawStart = leftPaw.localRotation;
            rPawStart = rightPaw.localRotation;
            lPawEnd = lPawStart * Quaternion.Euler(pawRotationAxis);
            rPawEnd = rPawStart * Quaternion.Euler(-pawRotationAxis);
        }

        if (leftMirror && rightMirror)
        {
            lMirStart = leftMirror.localRotation;
            rMirStart = rightMirror.localRotation;
            lMirEnd = lMirStart * Quaternion.Euler(mirrorFoldAxis);
            rMirEnd = rMirStart * Quaternion.Euler(-mirrorFoldAxis);
        }
        if (leftFlashLight && rightFlashLight)
        {
            lFlashStart = leftFlashLight.localRotation;
            rFlashStart = rightFlashLight.localRotation;
            lFlashEnd = lFlashStart * Quaternion.Euler(flashRotationAxis);
            rFlashEnd = rFlashStart * Quaternion.Euler(flashRotationAxis);
        }

        if (leftLightSource) leftLightSource.enabled = false;
        if (rightLightSource) rightLightSource.enabled = false;

        if (leftVacuum && rightVacuum)
        {
            lVacStart = leftVacuum.localRotation;
            rVacStart = rightVacuum.localRotation;
        }

        if (leftLightSource) leftLightSource.enabled = false;
        if (rightLightSource) rightLightSource.enabled = false;

        if (g29Input != null)
        {
            g29Input.OnLeftBumperCallback += value => { if (value) ToggleUpMechanism(); };

            g29Input.OnRightBumperCallback += value => { if (value) ToggleLightBulbs(); };

            g29Input.OnSouthButtonCallback += value => { if (value) TogglePaws(); };

            g29Input.OnWestButtonCallback += value => { if (value) ToggleMirrors(); };

            g29Input.OnNorthButtonCallback += value => { if (value) ToggleBrushes(); };

            g29Input.OnEastButtonCallback += value => { if (value) ToggleVacuum(); };
        }
    }

    private void FixedUpdate()
    {
        HandleMotor();
        HandleSteering();
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

    private void Update()
    {
        GetInput();
        UpdateWheels();
        HandleGearShifting();

        float engineIntens = 0.2f + Mathf.Abs(verticalInput) * 0.8f;

        if (Haptics.Instance != null)
        {
            Haptics.Instance.PlayEngineLoop(engineIntens);
        }

        if (Input.GetKeyDown(KeyCode.E)) TogglePaws();

        if (Input.GetKeyDown(KeyCode.Q)) ToggleMirrors();

        if (Input.GetKeyDown(KeyCode.T)) ToggleBrushes();

        if (Input.GetKeyDown(KeyCode.G)) ToggleUpMechanism();

        if (Input.GetKeyDown(KeyCode.F)) ToggleLightBulbs();

        if (Input.GetKeyDown(KeyCode.V)) ToggleVacuum();

        if (AudioManager.Instance)
        {
            float speedFactor = _rb.linearVelocity.magnitude / maxSpeed;
            float rpm = Mathf.Clamp01(speedFactor + Mathf.Abs(verticalInput) * 0.3f);
            AudioManager.Instance.UpdateEngine(rpm);
        }

        HandleBrushesRotation();
        HandleVacuumVibration();

    }
    private void GetInput()
    {
        if (g29Input != null)
        {

            horizontalInput = g29Input.Steering;

            verticalInput = g29Input.Throttle;

            isBraking = g29Input.Brake > 0.1f;
        }
        else
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
            isBraking = Input.GetKey(KeyCode.Space);
        }
    }

    private void HandleGearShifting()
    {
        if (g29Input == null) return;

        if (g29Input.Shifter7)
        {
            driveDirection = -1;
        }
        else if (g29Input.Shifter1 || g29Input.Shifter2 || g29Input.Shifter3 ||
                 g29Input.Shifter4 || g29Input.Shifter5 || g29Input.Shifter6)
        {
            driveDirection = 1;
        }
        else
        {
            driveDirection = 0;
        }
    }

    private void HandleMotor()
    {
        float finalTorque = verticalInput * motoreForce * driveDirection;

        frontLeftWheelCollider.motorTorque = finalTorque;
        frontRightWheelCollider.motorTorque = finalTorque;
        rearLeftWheelCollider.motorTorque = finalTorque;
        rearRightWheelCollider.motorTorque = finalTorque;

        currentBrakeForce = isBraking ? brakeForce : 0f;
        ApplyBraking();
    }

    private void ApplyBraking()
    {
        frontLeftWheelCollider.brakeTorque = currentBrakeForce;
        frontRightWheelCollider.brakeTorque = currentBrakeForce;
        rearLeftWheelCollider.brakeTorque = currentBrakeForce;
        rearRightWheelCollider.brakeTorque = currentBrakeForce;
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

    private void HandleVacuumVibration()
    {
        if (!isVacuuming) return;
        if (!leftVacuum || !rightVacuum) return;

        float x = Random.Range(-vibrationStrength, vibrationStrength);
        float y = Random.Range(-vibrationStrength, vibrationStrength);
        float z = Random.Range(-vibrationStrength, vibrationStrength);

        Quaternion shake = Quaternion.Euler(x, y, z);

        leftVacuum.localRotation = lVacStart * shake;

        float x2 = Random.Range(-vibrationStrength, vibrationStrength);
        float y2 = Random.Range(-vibrationStrength, vibrationStrength);
        float z2 = Random.Range(-vibrationStrength, vibrationStrength);

        rightVacuum.localRotation = rVacStart * Quaternion.Euler(x2, y2, z2);
    }

    private void ResetVacuumRotation()
    {
        if (leftVacuum) leftVacuum.localRotation = lVacStart;
        if (rightVacuum) rightVacuum.localRotation = rVacStart;
    }


    private void ToggleVacuum()
    {
        isVacuuming = !isVacuuming;
        if (!isVacuuming) ResetVacuumRotation();

        if (Haptics.Instance) Haptics.Instance.ToggleVacuum(isVacuuming);

        if (AudioManager.Instance) AudioManager.Instance.SetToolSound(isVacuuming, true);
    }

    private void ToggleBrushes()
    {
        isBrushing = !isBrushing;

        if (Haptics.Instance) Haptics.Instance.ToggleBrushes(isBrushing);

        if (AudioManager.Instance) AudioManager.Instance.SetToolSound(isBrushing, false);
    }

    public void ToggleUpMechanism()
    {
        if (!leftFlashLight || !rightFlashLight) return;
        areFlashLightsUp = !areFlashLightsUp;
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        string hapticKey = Haptics.Instance ? Haptics.Instance.flashEvent : "";
        flashCoroutine = StartCoroutine(AnimateParts(leftFlashLight, rightFlashLight, 
            areFlashLightsUp ? lFlashEnd : lFlashStart,
            areFlashLightsUp ? rFlashEnd : rFlashStart, 
            flashRotSpeed, hapticKey, (isOn) => AudioManager.Instance?.SetMechSound(isOn, 2)));
    }

    public void TogglePaws()
    {
        if (!leftPaw || !rightPaw) return;
        arePawsOpen = !arePawsOpen;
        if (pawCoroutine != null) StopCoroutine(pawCoroutine);
        string hapticKey = Haptics.Instance ? Haptics.Instance.pawEvent : "";
        pawCoroutine = StartCoroutine(AnimateParts(leftPaw, rightPaw,
            arePawsOpen ? lPawEnd : lPawStart,
            arePawsOpen ? rPawEnd : rPawStart,
            pawRotSpeed, hapticKey, (isOn) => AudioManager.Instance?.SetMechSound(isOn, 0)));
    }

    public void ToggleLightBulbs()
    {
        if (AudioManager.Instance) AudioManager.Instance.PlaySwitch();
        isLightOn = !isLightOn;
        if (leftLightSource) leftLightSource.enabled = isLightOn;
        if (rightLightSource) rightLightSource.enabled = isLightOn;
    }

    public void ToggleMirrors()
    {
        if (!leftMirror || !rightMirror) return;
        areMirrorsFolded = !areMirrorsFolded;
        if (mirrorCoroutine != null) StopCoroutine(mirrorCoroutine);
        string hapticKey = Haptics.Instance ? Haptics.Instance.mirrorEvent : "";
        mirrorCoroutine = StartCoroutine(AnimateParts(leftMirror, rightMirror, 
            areMirrorsFolded ? lMirEnd : lMirStart, 
            areMirrorsFolded? rMirEnd : rMirStart,
            mirrorRotSpeed, hapticKey, (isOn) => AudioManager.Instance?.SetMechSound(isOn, 1)));
    }

    private void HandleBrushesRotation()
    {
        if (!isBrushing) return;

        if (leftPawHead != null)
        {
            leftPawHead.Rotate(brushLocalAxis * brushSpinSpeed * Time.deltaTime, Space.Self);
        }

        if (rightPawHead != null)
        {
            rightPawHead.Rotate(-brushLocalAxis * brushSpinSpeed * Time.deltaTime, Space.Self);
        }
    }

    private IEnumerator AnimateParts(Transform partL, Transform partR, Quaternion targetL, Quaternion targetR, float speed, string hapticEventKey, System.Action<bool> soundAction)
    {
        if (Haptics.Instance != null && hapticEventKey != "")
        {
            Haptics.Instance.SetMechanismState(hapticEventKey, true);
        }
        if (soundAction != null) soundAction(true);

        while (Quaternion.Angle(partL.localRotation, targetL) > 0.1f ||
               Quaternion.Angle(partR.localRotation, targetR) > 0.1f)
        {
            partL.localRotation = Quaternion.RotateTowards(partL.localRotation, targetL, speed * Time.deltaTime);
            partR.localRotation = Quaternion.RotateTowards(partR.localRotation, targetR, speed * Time.deltaTime);
            yield return null;
        }
        partL.localRotation = targetL;
        partR.localRotation = targetR;

        if (Haptics.Instance != null && hapticEventKey != "")
        {
            Haptics.Instance.SetMechanismState(hapticEventKey, false);
        }
        if (soundAction != null) soundAction(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        float impactForce = collision.impulse.magnitude;

        if (impactForce > 1000f)
        {
            if (Haptics.Instance != null)
            {
                Haptics.Instance.PlayImpact(impactForce);
                if (AudioManager.Instance) AudioManager.Instance.PlayImpact(impactForce);
            }
        }
    }
}
