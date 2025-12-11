using LogitechG29.Sample.Input;
using UnityEngine;
using UnityEngine.UI;

public class SteeringUIController : MonoBehaviour
{
    [Header("Связи")]
    public InputControllerReader g29Input; // Ридер G29
    public Button[] menuButtons; // Кнопки этой панели

    [Header("Настройки")]
    public float navigationCooldown = 0.25f;

    private int _currentIndex = 0;
    private float _lastInputTime;
    private bool _isSouthPressed = false;

    // ВАЖНО: Используем OnEnable вместо Start
    // Это сработает, когда WinPanel включится в конце игры
    private void OnEnable()
    {
        if (menuButtons.Length > 0)
        {
            _currentIndex = 0; // Сброс на первую кнопку
            SelectButton(_currentIndex);
        }
    }

    private void Update()
    {
        // Если панель выключена или нет кнопок - ничего не делаем
        if (g29Input == null || menuButtons.Length == 0) return;

        // 1. НАВИГАЦИЯ (Крестовина)
        float dPadY = g29Input.HatSwitch.y;

        if (Time.time > _lastInputTime + navigationCooldown)
        {
            if (dPadY > 0.5f) // Вверх
            {
                ChangeSelection(-1);
                _lastInputTime = Time.time;
            }
            else if (dPadY < -0.5f) // Вниз
            {
                ChangeSelection(1);
                _lastInputTime = Time.time;
            }
        }

        // 2. ПОДТВЕРЖДЕНИЕ (Крестик/X)
        if (g29Input.SouthButton)
        {
            if (!_isSouthPressed)
            {
                _isSouthPressed = true;
                ClickCurrentButton();
            }
        }
        else
        {
            _isSouthPressed = false;
        }
    }

    private void ChangeSelection(int direction)
    {
        _currentIndex += direction;
        if (_currentIndex >= menuButtons.Length) _currentIndex = 0;
        if (_currentIndex < 0) _currentIndex = menuButtons.Length - 1;

        SelectButton(_currentIndex);
    }

    private void SelectButton(int index)
    {
        menuButtons[index].Select();
    }

    private void ClickCurrentButton()
    {
        menuButtons[_currentIndex].onClick.Invoke();
    }
}