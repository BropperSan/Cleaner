using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BloodSpawner : MonoBehaviour
{
    private BoxCollider[] colliders;
    private Queue<KeyValuePair<Vector3, GameObject>> bloodSplits = new();
    [SerializeField] public int wave;

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
        bloodSplits.Clear();
        colliders = GetComponentsInChildren<BoxCollider>();
    }

    private void Start()
    {
        GenerateBloodQueue(wave);
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
    public Vector3 GetRandomPoint(BoxCollider col, GameObject bloodSplit)
    {
        if (col != null)
        {
            BoxCollider bloodCol = bloodSplit.GetComponent<BoxCollider>();
            float xOffset = bloodCol.size.x * bloodSplit.transform.localScale.x / 2;
            float zOffset = bloodCol.size.z * bloodSplit.transform.localScale.z / 2;
            Vector3 localPoint = new Vector3(
            Random.Range(col.bounds.min.x + xOffset, col.bounds.max.x - xOffset),
            col.bounds.max.y * 4,
            Random.Range(col.bounds.min.z + zOffset, col.bounds.max.z - zOffset)
            );

            return localPoint;
        }
        return Vector3.zero;
    }

    private void Spawn()
    {
        if (bloodSplits.Count != 0)
        {
            KeyValuePair<Vector3, GameObject> bloodSplit = bloodSplits.Dequeue();
            Instantiate(bloodSplit.Value, bloodSplit.Key, Quaternion.identity);
        }
    }

    private void GenerateBloodQueue(int wave)
    {
        for (int i = 0; i < wave; i++)
        {
            GameObject bloodSplit = Resources.Load<GameObject>("Blood/blood" + Random.Range(1, 7));
            bloodSplits.Enqueue(new KeyValuePair<Vector3, GameObject>(GetRandomPoint(GetRandomCollider(), bloodSplit), bloodSplit));
        }
    }
}
