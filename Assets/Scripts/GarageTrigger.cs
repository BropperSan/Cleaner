using UnityEngine;

public class GarageTrigger : MonoBehaviour
{
    public MainMenuController menuController;

    private bool _hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_hasTriggered) return;

        if (other.GetComponentInParent<CarHandler>())
        {
            _hasTriggered = true;

            if (menuController != null)
            {
                menuController.CloseGarageDoor();
            }

            gameObject.SetActive(false);
        }
    }
}