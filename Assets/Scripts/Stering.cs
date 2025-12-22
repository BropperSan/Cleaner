using LogitechG29.Sample.Input;
using UnityEngine;
using UnityEngine.UI;

public class SteeringUIController : MonoBehaviour
{
    public InputControllerReader g29Input;
    public Button[] menuButtons;

    public float navigationCooldown = 0.25f;

    private int _currentIndex = 0;
    private float _lastInputTime;
    private bool _isSouthPressed = false;


    private void OnEnable()
    {
        if (menuButtons.Length > 0)
        {
            _currentIndex = 0;
            SelectButton(_currentIndex);
        }
    }

    private void Update()
    {

        if (g29Input == null || menuButtons.Length == 0) return;


        float dPadY = g29Input.HatSwitch.y;

        if (Time.time > _lastInputTime + navigationCooldown)
        {
            if (dPadY > 0.5f)
            {
                ChangeSelection(-1);
                _lastInputTime = Time.time;
            }
            else if (dPadY < -0.5f)
            {
                ChangeSelection(1);
                _lastInputTime = Time.time;
            }
        }

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