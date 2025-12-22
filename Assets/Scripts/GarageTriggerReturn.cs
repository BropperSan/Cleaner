using System.Collections;
using UnityEngine;

public class GarageReturnTrigger : MonoBehaviour
{
    public MainMenuController menuController;
    public float activationDelay = 60f;

    private Collider _myCollider;
    private bool _hasTriggered = false;

    private void Awake()
    {
        _myCollider = GetComponent<Collider>();
    }

    private IEnumerator Start()
    {
        if (_myCollider) _myCollider.enabled = false;


        yield return new WaitForSeconds(activationDelay);

        if (_myCollider) _myCollider.enabled = true;
        Debug.Log("Триггер возвращения АКТИВЕН! Ждем машину домой.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hasTriggered) return;

        if (other.GetComponentInParent<CarHandler>())
        {
            Debug.Log("Машина вернулась на базу!");
            _hasTriggered = true;

            if (menuController != null)
            {
                menuController.TriggerWinSequence();
            }
        }
    }
}
