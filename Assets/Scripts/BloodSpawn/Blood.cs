using UnityEngine;

public class Blood : MonoBehaviour
{
    private BloodSpawner _spawner;
    private Renderer _renderer;
    private Color _color;
    private bool _isFullyCleaned = false;

    private float cleanSpeed = 0.5f;

    public void Initialize(BloodSpawner spawner)
    {
        _spawner = spawner;
    }

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        _color = _renderer.material.color;
    }

    private void OnTriggerStay(Collider other)
    {
        CarHandler car = other.GetComponentInParent<CarHandler>();
        if (car != null && car.IsBrushingActive)
        {
            Clean(Time.deltaTime);
        }
    }

    private void Clean(float time)
    {
        if (_isFullyCleaned) return;

        _color.a -= cleanSpeed * time;
        _renderer.material.color = _color;

        if (_color.a <= 0f)
        {
            _isFullyCleaned = true;

            if (_spawner != null)
            {
                _spawner.OnBloodDespawned(this.gameObject);
            }

            Destroy(gameObject);
        }
    }
}