using UnityEngine;
using TMPro;

public class Volume : MonoBehaviour
{
    public TextMeshProUGUI volumeText;

    public float step = 0.01f;

    private void Start()
    {
        float savedVol = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
        AudioListener.volume = savedVol;

        UpdateText();
    }

    public void Increase()
    {
        ChangeVolume(step);
    }

    public void Decrease()
    {
        ChangeVolume(-step);
    }

    private void ChangeVolume(float amount)
    {
        float newVol = AudioListener.volume + amount;

        newVol = Mathf.Clamp01(newVol);

        AudioListener.volume = newVol;

        PlayerPrefs.SetFloat("MasterVolume", newVol);
        PlayerPrefs.Save();

        UpdateText();
    }

    private void UpdateText()
    {
        if (volumeText != null)
        {
            int percent = Mathf.RoundToInt(AudioListener.volume * 100);

            volumeText.text = percent.ToString();
        }
    }
}