using UnityEngine;
using UnityEngine.UI;

public class NoiseManager : MonoBehaviour
{
    public static NoiseManager Instance;

    public CarHandler car;
    public Image noiseBar;

    public float idleNoise = 10f;
    public float gasNoise = 40f;
    public float brushNoise = 20f;
    public float vacuumNoise = 30f;

    public float smoothSpeed = 5f;

    [HideInInspector] public float currentNoiseLevel = 0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Update()
    {
        if (car == null) return;

        float targetNoise = 0f;


        targetNoise += idleNoise + (car.CurrentThrottle * gasNoise);

        if (car.IsBrushingActive) targetNoise += brushNoise;

        if (car.IsVacuumingActive) targetNoise += vacuumNoise;

        targetNoise = Mathf.Clamp(targetNoise, 0f, 100f);


        currentNoiseLevel = Mathf.Lerp(currentNoiseLevel, targetNoise, Time.deltaTime * smoothSpeed);

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (noiseBar != null)
        {
            float fill = currentNoiseLevel / 100f;
            noiseBar.fillAmount = fill;

            noiseBar.color = Color.Lerp(Color.green, Color.red, fill);
        }
    }
}