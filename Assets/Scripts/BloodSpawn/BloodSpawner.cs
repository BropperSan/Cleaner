using UnityEngine;

public class BloodSpawner : MonoBehaviour
{
    private BoxCollider[] colliders;
    private BoxCollider randCol;
    private BoxCollider bloodCol;
    [SerializeField] public GameObject blood;

    private void OnEnable()
    {
        Blood.OnBloodWiped += Spawn;
    }

    private void OnDisable()
    {
        Blood.OnBloodWiped -= Spawn;
    }
    private void Awake()
    {
        bloodCol = blood.GetComponent<BoxCollider>();
        colliders = GetComponentsInChildren<BoxCollider>();
    }

    private void Update()
    {
        Spawn();
    }

    public BoxCollider GetRandomCollider()
    {
        if (colliders.Length == 0)
            return null;

        return colliders[Random.Range(0, colliders.Length)];
    }
    public Vector3 GetRandomPoint(BoxCollider col)
    {
        if (col != null)
        {
            Vector3 localPoint = new Vector3(
            Random.Range(col.bounds.min.x + bloodCol.size.x / 2, col.bounds.max.x - bloodCol.size.x / 2),
            col.bounds.max.y * 4,
            Random.Range(col.bounds.min.z + bloodCol.size.z / 2, col.bounds.max.z - bloodCol.size.z / 2)
            );

            return localPoint;
        }
        return Vector3.zero;
    }

    private void Spawn()
    {
        Vector3 point = GetRandomPoint(GetRandomCollider());
        Instantiate(blood, point, Quaternion.identity);
    }
}
