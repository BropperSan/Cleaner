using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    // Синглтон, чтобы вызывать отовсюду
    public static LevelManager Instance;

    [Header("События при полной очистке")]
    public UnityEvent OnLevelCompleted; // Сюда в инспекторе перетащишь звуки, UI победы и т.д.

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void TriggerWin()
    {
        Debug.Log("Уровень пройден! Все чисто!");
        OnLevelCompleted?.Invoke();
    }
}