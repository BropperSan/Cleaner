using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{

    public CarHandler car;
    public BloodSpawner spawner;

    public Image healthBar;
    public Image progressBar;

    public float smoothSpeed = 5f;
    public Gradient healthColor;

    private void Update()
    {
        UpdateHealth();
        UpdateProgress();
    }

    private void UpdateHealth()
    {
        if (car == null || healthBar == null) return;

        float targetFill = car.GetHealthRatio();

        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, targetFill, Time.deltaTime * smoothSpeed);

        healthBar.color = healthColor.Evaluate(healthBar.fillAmount);
    }

    private void UpdateProgress()
    {
        if (spawner == null || progressBar == null) return;

        float targetProgress = spawner.GetCleanProgress();

        progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, targetProgress, Time.deltaTime * smoothSpeed);
    }
}