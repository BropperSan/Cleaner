using _2DOF;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class MainMenuController : MonoBehaviour
{
    public string nextSceneName;
    public GameObject menuPanel;
    public CarHandler carController;
    public GameObject winPanel;
    public GameObject losePanel;


    public Transform garageDoor;
    public Vector3 doorMoveOffset = new Vector3(0, 0.022f, 0);
    public float startDelay = 2.0f;
    public float doorDuration = 5.5f;


    public AudioSource doorSource;
    public AudioClip buttonClickSound;
    public AudioClip doorOpenSound;
    public AudioClip doorCloseSound;
    private Vector3 _closedPos;
    private Vector3 _openPos;

    private AudioSource _uiSource;

    private void Awake()
    {
        _uiSource = gameObject.AddComponent<AudioSource>();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        AudioListener.volume = 1.0f;

        if (carController) carController.enabled = false;
        if (winPanel) winPanel.SetActive(false);

        if (garageDoor)
        {
            _closedPos = garageDoor.localPosition;
            _openPos = _closedPos + doorMoveOffset;
        }
    }

    private void Start()
    {
        if (!menuPanel.activeSelf)
        {
            StartCoroutine(StartSequence());
        }
    }

    public void OnStartGameClicked()
    {
        if (buttonClickSound) _uiSource.PlayOneShot(buttonClickSound);

        StartCoroutine(StartSequence());
    }

    public void OnExitGameClicked()
    {
        if (buttonClickSound) _uiSource.PlayOneShot(buttonClickSound);

        if (carController != null)
        {
            var telemetry = carController.GetComponent<CarTelemetryHandler>();

            if (telemetry != null)
            {
                telemetry.ForceReset();
            }
        }
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private IEnumerator StartSequence()
    {
        menuPanel.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        yield return new WaitForSeconds(startDelay);

        if (doorSource && doorOpenSound)
        {
            doorSource.clip = doorOpenSound;
            doorSource.Play();
        }

        if (garageDoor)
        {
            Vector3 startPos = garageDoor.localPosition;
            Vector3 endPos = startPos + doorMoveOffset;
            float elapsed = 0f;

            while (elapsed < doorDuration)
            {
                garageDoor.localPosition = Vector3.Lerp(startPos, endPos, elapsed / doorDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            garageDoor.localPosition = endPos;
        }

        if (carController)
        {
            carController.enabled = true;
            carController.GetComponentInParent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
    }

    public void CloseGarageDoor()
    {
        StartCoroutine(CloseDoorRoutine());
    }

    private IEnumerator CloseDoorRoutine()
    {

        if (doorSource)
        {
            doorSource.clip = doorCloseSound ? doorCloseSound : doorOpenSound;
            doorSource.Play();
        }

        if (garageDoor)
        {
            float elapsed = 0f;
            while (elapsed < doorDuration)
            {
                garageDoor.localPosition = Vector3.Lerp(_openPos, _closedPos, elapsed / doorDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            garageDoor.localPosition = _closedPos;
        }
    }

    public void TriggerWinSequence()
    {
        StartCoroutine(WinRoutine());
    }

    private IEnumerator WinRoutine()
    {
        CloseGarageDoor();

        yield return new WaitForSeconds(doorDuration + 2.0f);

        if (carController)
        {
            carController.enabled = false;
            carController.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        }

        float startVol = AudioListener.volume;
        float fadeDuration = 2.0f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            AudioListener.volume = Mathf.Lerp(startVol, 0f, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        AudioListener.volume = 0f;

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            Debug.Log($"Загрузка уровня: {nextSceneName}");
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            if (winPanel) winPanel.SetActive(true);
        }
    }
    public void TriggerLoseSequence()
    {
        StartCoroutine(LoseRoutine());
    }

    private IEnumerator LoseRoutine()
    {
        Debug.Log("Поражение! Глушим системы...");
        if (carController)
        {
            carController.enabled = false;
            carController.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            carController.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }

        float startVol = AudioListener.volume;
        float fadeDuration = 2.0f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            AudioListener.volume = Mathf.Lerp(startVol, 0f, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        AudioListener.volume = 0f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (losePanel) losePanel.SetActive(true);
    }
}