using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioClip engineClip;
    public AudioClip heavyMechClip;
    public AudioClip lightMechClip;
    public AudioClip flashMechClip;
    public AudioClip vacuumClip;
    public AudioClip brushClip;
    public AudioClip switchClip;
    public AudioClip impactClip;

    public float minPitch = 0.8f;
    public float maxPitch = 2.0f;

    public AudioSource engineSource;

    public AudioSource pawsSourceL;
    public AudioSource pawsSourceR;
    public AudioSource mirrorsSourceL;
    public AudioSource mirrorsSourceR;
    public AudioSource flashSourceL;
    public AudioSource flashSourceR;

    public AudioSource vacuumSourceL;
    public AudioSource vacuumSourceR;
    public AudioSource brushSourceL;
    public AudioSource brushSourceR;
    public AudioSource sfxSource;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

    }

    private void Start()
    {
        if (engineSource) { engineSource.clip = engineClip; engineSource.Play(); engineSource.loop = true; }

        if (pawsSourceL) { pawsSourceL.clip = heavyMechClip; pawsSourceL.loop = true; }
        if (pawsSourceR) { pawsSourceR.clip = heavyMechClip; pawsSourceR.loop = true; }

        if (mirrorsSourceL) { mirrorsSourceL.clip = lightMechClip; mirrorsSourceL.loop = true; }
        if (mirrorsSourceR) { mirrorsSourceR.clip = lightMechClip; mirrorsSourceR.loop = true; }

        if (flashSourceL) { flashSourceL.clip = flashMechClip; flashSourceL.loop = true; }
        if (flashSourceR) { flashSourceR.clip = flashMechClip; flashSourceR.loop = true; }

        if (vacuumSourceL) { vacuumSourceL.clip = vacuumClip; vacuumSourceL.loop = true; }
        if (vacuumSourceR) { vacuumSourceR.clip = vacuumClip; vacuumSourceR.loop = true; }

        if (brushSourceL) { brushSourceL.clip = brushClip; brushSourceL.loop = true; }
        if (brushSourceR) { brushSourceR.clip = brushClip; brushSourceR.loop = true; }
    }

    public void UpdateEngine(float ratio)
    {
        if (engineSource && engineSource.isPlaying)
        {
            float targetPitch = Mathf.Lerp(minPitch, maxPitch, ratio);
            engineSource.pitch = Mathf.Lerp(engineSource.pitch, targetPitch, Time.deltaTime * 5f);
        }
    }

    public void SetMechSound(bool isMoving, int type)
    {

        if (type == 0)
        {
            if (isMoving)
            {
                if (pawsSourceL && !pawsSourceL.isPlaying) pawsSourceL.Play();
                if (pawsSourceR && !pawsSourceR.isPlaying) pawsSourceR.Play();
            }
            else
            {
                if (pawsSourceL) pawsSourceL.Stop();
                if (pawsSourceR) pawsSourceR.Stop();
            }
        }
        else if (type == 1)
        {
            if (isMoving)
            {
                if (mirrorsSourceL && !mirrorsSourceL.isPlaying) mirrorsSourceL.Play();
                if (mirrorsSourceR && !mirrorsSourceR.isPlaying) mirrorsSourceR.Play();
            }
            else
            {
                if (mirrorsSourceL) mirrorsSourceL.Stop();
                if (mirrorsSourceR) mirrorsSourceR.Stop();
            }
        }
        else if (type == 2)
        {
            if (isMoving)
            {
                if (flashSourceL && !flashSourceL.isPlaying) flashSourceL.Play();
                if (flashSourceR && !flashSourceR.isPlaying) flashSourceR.Play();
            }
            else
            {
                if (flashSourceL) flashSourceL.Stop();
                if (flashSourceR) flashSourceR.Stop();
            }
        }
    }

    public void SetToolSound(bool isOn, bool isVacuum)
    {
        if (isVacuum)
        {
            TogglePair(isOn, vacuumSourceL, vacuumSourceR);
        }
        else
        {
            TogglePair(isOn, brushSourceL, brushSourceR);
        }
    }

    private void TogglePair(bool play, AudioSource L, AudioSource R)
    {
        if (play)
        {
            if (L && !L.isPlaying) L.Play();
            if (R && !R.isPlaying) R.Play();
        }
        else
        {
            if (L) L.Stop();
            if (R) R.Stop();
        }
    }

    public void PlaySwitch()
    {
        if (sfxSource && switchClip) sfxSource.PlayOneShot(switchClip);
    }

    public void PlayImpact(float force)
    {
        if (sfxSource && impactClip && force > 2000f)
        {
            float vol = Mathf.Clamp01(force / 5000f);
            sfxSource.PlayOneShot(impactClip, vol);
        }
    }
}