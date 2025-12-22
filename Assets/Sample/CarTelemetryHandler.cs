using System.Collections;
using _2DOF;
using UnityEngine;

public class CarTelemetryHandler : MonoBehaviour
{
    private const float WAIT_TIME = SendingData.WAIT_TIME / 1000f;

    [SerializeField] private Transform vehicleTransform;
    [SerializeField] private Rigidbody carRigidbody;

    [SerializeField] private bool useLocalVelocity = true;

    [Range(0.5f, 100f)]
    [SerializeField] private float angleMultiplier = 1.0f;

    [Range(0.5f, 100f)]
    [SerializeField] private float velocityMultiplier = 1.0f;

    private ObjectTelemetryData _telemetryDataData;
    private SendingData _sendingData;

    private WaitForSeconds _waitObj;

    private void Awake()
    {
        if (vehicleTransform == null) vehicleTransform = transform;
        if (carRigidbody == null) carRigidbody = GetComponent<Rigidbody>();

        _sendingData = new SendingData();
        _telemetryDataData = _sendingData.ObjectTelemetryData;

        _waitObj = new WaitForSeconds(WAIT_TIME);
    }

    private void OnEnable()
    {
        if (_sendingData == null) return;

        _sendingData.SendingStart();
        StartCoroutine(TelemetryRoutine());
    }

    private void OnDisable()
    {
        if (_sendingData == null) return;

        _sendingData.SendingStop();
        StopCoroutine(TelemetryRoutine());
    }

    private IEnumerator TelemetryRoutine()
    {
        while (true)
        {
            if (_telemetryDataData == null)
            {
                yield return new WaitForSeconds(1f);
                continue;
            }

            UpdateAngles();
            UpdateVelocity();

            yield return _waitObj;
        }
    }

    private void UpdateVelocity()
    {
        Vector3 vel = carRigidbody.linearVelocity;

        if (useLocalVelocity)
        {
            vel = vehicleTransform.InverseTransformDirection(vel);
        }

        _telemetryDataData.Velocity = vel * velocityMultiplier;
    }

    private void UpdateAngles()
    {
        Vector3 rawAngles = vehicleTransform.eulerAngles;

        Vector3 normalizedAngles = new Vector3(
            Mathf.DeltaAngle(0, rawAngles.x) * angleMultiplier,
            Mathf.DeltaAngle(0, rawAngles.y),
            Mathf.DeltaAngle(0, rawAngles.z) * angleMultiplier
        );

        _telemetryDataData.Angles = normalizedAngles;
    }
    public void ForceReset()
    {
        if (_telemetryDataData != null)
        {
            _telemetryDataData.Reset();
            Debug.Log("Телеметрия сброшена в ноль перед выходом.");
        }
    }
}