using System.Collections.Generic;
using UnityEngine;

public class BloodSpawner : MonoBehaviour
{
    private List<BoxCollider> spawnZones = new List<BoxCollider>();

    private Queue<KeyValuePair<Vector3, GameObject>> bloodDataQueue = new();

    private List<GameObject> activeBloodList = new();

    [SerializeField] public int bloodAmount = 10;
    [SerializeField]  public string zoneTag = "BloodZone";


    private void Start()
    {
        GameObject[] zoneObjects = GameObject.FindGameObjectsWithTag(zoneTag);

        spawnZones.Clear();
        foreach (var obj in zoneObjects)
        {
            BoxCollider col = obj.GetComponent<BoxCollider>();
            if (col != null)
            {
                spawnZones.Add(col);
            }
        }

        if (spawnZones.Count == 0)
        {
            Debug.LogError($"BloodSpawner: Не найдено объектов с тегом '{zoneTag}' и BoxCollider!");
            return;
        }

        Debug.Log($"Найдено зон спавна: {spawnZones.Count}. Генерируем кровь...");

        GenerateBloodQueue(bloodAmount);
    }

    private void Update()
    {
        ProcessSpawnQueue();
    }


    public void InitializeSpawner()
    {
        BoxCollider[] allColliders = GetComponentsInChildren<BoxCollider>();

        spawnZones.Clear();

        foreach (var col in allColliders)
        {
            if (col.CompareTag("BloodZone"))
            {
                spawnZones.Add(col);
            }

        }

        if (spawnZones.Count == 0)
        {
            Debug.LogError("BloodSpawner: Не найдено ни одной зоны спавна! Проверь имена объектов в префабах.");
            return;
        }

        Debug.Log($"BloodSpawner: Найдено {spawnZones.Count} зон для спавна.");

        GenerateBloodQueue(bloodAmount);
    }

    private void GenerateBloodQueue(int count)
    {
        bloodDataQueue.Clear();
        activeBloodList.Clear();

        for (int i = 0; i < count; i++)
        {
            GameObject prefab = Resources.Load<GameObject>("Blood/blood" + Random.Range(1, 8));

            if (prefab != null)
            {
                BoxCollider randomZone = spawnZones[Random.Range(0, spawnZones.Count)];
                Vector3 point = GetRandomPoint(randomZone, prefab);

                bloodDataQueue.Enqueue(new KeyValuePair<Vector3, GameObject>(point, prefab));
            }
        }
    }

    private void ProcessSpawnQueue()
    {
        if (bloodDataQueue.Count > 0)
        {
            KeyValuePair<Vector3, GameObject> data = bloodDataQueue.Dequeue();

            GameObject newBlood = Instantiate(data.Value, data.Key, Quaternion.identity);

            activeBloodList.Add(newBlood);

            Blood bloodScript = newBlood.GetComponent<Blood>();
            if (bloodScript != null)
            {
                bloodScript.Initialize(this);
            }
        }
    }

    public void OnBloodDespawned(GameObject bloodObj)
    {
        if (activeBloodList.Contains(bloodObj))
        {
            activeBloodList.Remove(bloodObj);
        }

        CheckWinCondition();
    }

    private void CheckWinCondition()
    {
        if (bloodDataQueue.Count == 0 && activeBloodList.Count == 0)
        {
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.TriggerWin();
            }
            else
            {
                Debug.Log("Победа! (Но LevelEventManager не найден на сцене)");
            }
        }
    }

    public Vector3 GetRandomPoint(BoxCollider col, GameObject bloodSplit)
    {
        if (col != null)
        {
            BoxCollider bloodCol = bloodSplit.GetComponent<BoxCollider>();
            if (bloodCol == null) return col.transform.position;

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
}