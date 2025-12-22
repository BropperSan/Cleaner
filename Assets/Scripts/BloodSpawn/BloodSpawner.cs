using System.Collections.Generic;
using UnityEngine;

public class BloodSpawner : MonoBehaviour
{
    private List<BoxCollider> spawnZones = new List<BoxCollider>();

    private Queue<KeyValuePair<Vector3, GameObject>> bloodDataQueue = new();

    private List<GameObject> activeBloodList = new();

    [SerializeField] public int bloodAmount = 10;
    [SerializeField]  public string zoneTag = "BloodZone";
    bool isGenerated = false;
    private int _totalBloodAtStart;

    private void Awake()
    {
        isGenerated = false;
    }

    private void Start()
    {   
        if (this.GetComponentInParent<LocationGeneration>() == null)
        {
            Debug.Log("Я полез в старт, хотя не должен был");
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
    }

    private void Update()
    {
        ProcessSpawnQueue();
    }


    public void InitializeSpawner()
    {
        isGenerated = false;
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
        _totalBloodAtStart = bloodDataQueue.Count;
        isGenerated = true;
    }

    private void ProcessSpawnQueue()
    {
        if (bloodDataQueue.Count > 0 && isGenerated)
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
        if (col == null) return Vector3.zero;

        float xPadding = 0f;
        float zPadding = 0f;

        if (bloodSplit != null)
        {
            BoxCollider bloodCol = bloodSplit.GetComponent<BoxCollider>();
            if (bloodCol != null)
            {
                xPadding = (bloodCol.size.x * bloodSplit.transform.localScale.x) / 2;
                zPadding = (bloodCol.size.z * bloodSplit.transform.localScale.z) / 2;
            }
        }

        float halfSizeX = (col.size.x / 2) - xPadding;
        float halfSizeZ = (col.size.z / 2) - zPadding;

        if (halfSizeX < 0) halfSizeX = 0;
        if (halfSizeZ < 0) halfSizeZ = 0;

        Vector3 randomLocalPoint = new Vector3(
            Random.Range(-halfSizeX, halfSizeX),
            col.size.y / 2,
            Random.Range(-halfSizeZ, halfSizeZ)
        );

        randomLocalPoint += col.center;

        Vector3 worldPos = col.transform.TransformPoint(randomLocalPoint);

        worldPos.y += 0.01f;

        return worldPos;
    }

    public float GetCleanProgress()
    {
        if (_totalBloodAtStart == 0) return 1f;

        int remaining = bloodDataQueue.Count + activeBloodList.Count;

        return 1f - ((float)remaining / _totalBloodAtStart);
    }
}