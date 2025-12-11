using System.Collections;
using UnityEngine;

public class EndingSequenceController : MonoBehaviour
{

    public AudioClip sirenClip;
    public AudioClip notificationClip;
    public AudioClip doorOpenSound;
    public AudioClip monsterScreamClip;


    public float initialDelay = 5.0f;
    public float delayBetweenSirenAndVoice = 2.0f;
    public float delayBeforeMonster = 1.0f;

    public Vector3 doorMoveOffset = new Vector3(0, 0.22f, 0);

    public float doorOpenDuration = 5.5f;

    public Transform emergencyDoor;
    private AudioSource _doorSource;
    private AudioSource _sirenSource;
    private AudioSource _notificationSource;
    private AudioSource _monsterSource;

    private void Awake()
    {
        _sirenSource = CreateSource("Siren_Source", true, 0.8f);
        _notificationSource = CreateSource("Voice_Source", false, 1.0f);
        _doorSource = CreateSource("Door_Source", false, 1.0f);
        _monsterSource = CreateSource("Monster_Source", true, 1.0f);
        _monsterSource.priority = 0;
    }

    private AudioSource CreateSource(string name, bool loop, float volume)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(transform);
        AudioSource source = go.AddComponent<AudioSource>();
        source.loop = loop;
        source.playOnAwake = false;
        source.volume = volume;
        return source;
    }


    public void StartEndingSequence()
    {
        StartCoroutine(SequenceRoutine());
    }

    private IEnumerator SequenceRoutine()
    {

        yield return new WaitForSeconds(initialDelay);

        if (sirenClip)
        {
            _sirenSource.clip = sirenClip;
            _sirenSource.volume = 0.8f;
            _sirenSource.Play();
        }

        if (emergencyDoor != null)
        {
            StartCoroutine(OpenDoorRoutine());
        }

        yield return new WaitForSeconds(delayBetweenSirenAndVoice);

        float voiceDuration = 0f;
        if (notificationClip)
        {
            _notificationSource.clip = notificationClip;
            _notificationSource.Play();

            voiceDuration = notificationClip.length;
        }

        yield return new WaitForSeconds(voiceDuration);

        yield return new WaitForSeconds(delayBeforeMonster);

        if (monsterScreamClip)
        {
            _monsterSource.clip = monsterScreamClip;
            _monsterSource.Play();
        }
    }

    private IEnumerator OpenDoorRoutine()
    {
        if (doorOpenSound)
        {
            _doorSource.clip = doorOpenSound;
            _doorSource.Play();
        }

        Vector3 startPos = emergencyDoor.localPosition;

        Vector3 endPos = startPos + doorMoveOffset;

        float elapsed = 0f;

        while (elapsed < doorOpenDuration)
        {
            emergencyDoor.localPosition = Vector3.Lerp(startPos, endPos, elapsed / doorOpenDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }


        emergencyDoor.localPosition = endPos;
    }
}