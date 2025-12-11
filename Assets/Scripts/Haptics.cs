using UnityEngine;
using Bhaptics.SDK2;

public class Haptics: MonoBehaviour
{
    public static Haptics Instance;

    public string engineEvent = "engine";
    public string vacuumEvent = "vacuum";
    public string brushEvent = "brush";
    public string impactEvent = "impact";
    public string pawEvent = "paws";
    public string mirrorEvent = "mirrors";
    public string flashEvent = "light";
    public float engineIntensityMult = 1f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        PlayEngineLoop(0.3f);
    }

    private void OnDisable()
    {
        BhapticsLibrary.StopAll();
    }

    public void PlayEngineLoop(float intensity)
    {

        BhapticsLibrary.PlayLoop(engineEvent, intensity * engineIntensityMult);
        
    }

    public void PlayImpact(float force)
    {
        float hapticForce = Mathf.Clamp(force / 1000f, 0.5f, 5f);
        BhapticsLibrary.PlayLoop(impactEvent, hapticForce);
    }

    public void ToggleVacuum(bool isOn)
    {
        if (isOn)
            BhapticsLibrary.PlayLoop(vacuumEvent, 1.0f);


        else
            BhapticsLibrary.StopByEventId(vacuumEvent);

    }

    public void ToggleBrushes(bool isOn)
    {
        if (isOn)
            BhapticsLibrary.PlayLoop(brushEvent, 0.8f);
        else
            BhapticsLibrary.StopByEventId(brushEvent);
    }

    public void SetMechanismState(string key, bool isMoving)
    {
        if (isMoving)
            BhapticsLibrary.PlayLoop(key, 1.0f);

        else
            BhapticsLibrary.StopByEventId(key);
    }
}