using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;


public class LocationGeneration : MonoBehaviour
{
    private int locationVariantsAmount = 4;
    private int maxWidth = 20;
    private int maxLength = 20;
    private int minLength = 10;
    private int minWidth = 10;
    private int minRectLength = 10;
    private int minRectWidth = 10;
    private int n;
    private int m;
    private GameObject[] locationVariants;
    private int[,] location;
    public GameObject enemyObject;

    private void Awake()
    {
        locationVariants = new GameObject[locationVariantsAmount];
        for (int i = 0; i < locationVariantsAmount; i++)
        {
            locationVariants[i] = Resources.Load<GameObject>("LocationPrefabs/WareHouseTile" + i);
        }

        n = Random.Range(minLength, maxLength);
        m = Random.Range(minWidth, maxWidth);
        location = new int[n, m];
        List<int> rects = new List<int>();
        List<int> racksLengths = new List<int>();


        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                location[i, j] = 8;
            }
        }

        // -1
        int k = n;
        while (true)
        {

            int length = Random.Range(minRectLength, (n + 1) / 2);
            if (length > k)
            {
                length = k;

            }
            k -= length;
            if (k == 0)
            {
                rects.Add(length);
                break;
            }
            rects.Add(length);

        }

        int offset = 0;
        int t = 0;
        while (offset != n)
        {
            int rectWidth = Random.Range(minRectWidth, m + 1);
            for (int i = 0; i < rects[t]; i++)
            {
                for (int j = 0; j < rectWidth; j++)
                {
                    location[i + offset, j] = -1;
                }
            }
            offset += rects[t];
            t++;
        }
        //

        // 1
        for (int i = 1; i < n - 1; i++)
        {
            for (int j = 1; j < m - 1; j++)
            {
                if ((location[i, j + 1] != 8) && (location[i + 1, j] != 8) && (location[i - 1, j] != 8))
                {
                    location[i, j] = 1;
                }
            }
        }
        //

        // 2
        List<string> racks = new List<string>();
        for (int i = 1; i < n - 1; i++)
        {
            int count = 0;
            for (int j = 0; j < m; j++)
            {
                if (location[i, j] == 1)
                {
                    count++;
                }
            }
            racks.Add(new string('a', count));
        }

        for (int i = 0; i < racks.Count; i++)
        {
            racksLengths.Clear();
            int len = racks[i].Length;
            int length;
            while (true)
            {

                if (len > 10)
                {
                    length = Random.Range(2, racks[i].Length / 3 + 1);
                }
                else
                {
                    length = Random.Range(2, 4);
                }
                if (length > len)
                {
                    length = len;

                }
                len -= length;
                if (len == 0)
                {
                    racksLengths.Add(length);
                    break;
                }
                racksLengths.Add(length);
            }
            char[] tmpChar = racks[i].ToCharArray();
            int offsetRack = 0;
            int num = 1;
            int chance = Random.Range(0, 100);
            foreach (int rlen in racksLengths)
            {
                for (int j = 0; j < rlen; ++j)
                {
                    if (num % 2 == chance % 2)
                    {
                        tmpChar[offsetRack] = 'b';
                    }
                    offsetRack++;
                }
                num++;
            }
            string tmpString = "";
            for (int j = 0; j < tmpChar.Length; j++)
            {
                tmpString += tmpChar[j];
            }
            racks[i] = tmpString;
        }

        int posi = 0;
        for (int i = 1; i < n - 1; i++)
        {
            int posj = 0;
            for (int j = 1; j < m - 1; j++)
            {
                if (location[i, j] == 1)
                {
                    if (racks[posi][posj] == 'b')
                    {
                        location[i, j] = 2;
                    }
                    posj++;
                }

            }
            posi++;
        }
        //

        // 3
        for (int i = 1; i < n - 1; i++)
        {
            for (int j = 1; j < m - 1; j++)
            {
                if (location[i, j] == 1 && location[i, j + 1] != 2 && location[i, j - 1] != 2)
                {
                    int chanse = Random.Range(0, 100);
                    if (chanse >= 65)
                    {
                        location[i, j] = 3;
                    }
                }
            }
        }
        //

        // 0
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                if (location[i, j] == 8)
                {
                    location[i, j] = 0;
                }
                if (location[i, j] == 1)
                {
                    int chanse = Random.Range(0, 100);
                    if (chanse >= 85)
                    {
                        location[i, j] = 0;
                    }
                }
            }
        }
        //

        for (int i = 0; i < n; ++i)
        {
            for (int j = 0; j < m; ++j)
            {
                if (location[i, j] == -1)
                {
                    location[i, j] = 1;
                }
            }
        }
    }

    private void Start()
    {
        GameObject tile;
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                tile = Instantiate(locationVariants[location[i, j]]);
                tile.transform.SetParent(transform);
                tile.transform.position = new Vector3(9 * i, 0, 9 * j);
                if (i == 0 && j == 0)
                {
                    tile.transform.GetChild(3).gameObject.SetActive(true);
                    tile.transform.GetChild(3).gameObject.layer = 9;
                    continue;
                }
                if (tile.tag != "Empty")
                {
                    if (i == 0)
                    {
                        tile.transform.GetChild(1).gameObject.SetActive(true);
                        tile.transform.GetChild(1).gameObject.layer = 9;
                    }
                    else
                    {
                        if (location[i - 1, j] == 0)
                        {
                            tile.transform.GetChild(1).gameObject.SetActive(true);
                            tile.transform.GetChild(1).gameObject.layer = 9;
                        }
                    }
                    if (i == n - 1)
                    {
                        tile.transform.GetChild(0).gameObject.SetActive(true);
                        tile.transform.GetChild(0).gameObject.layer = 9;
                    }
                    else
                    {
                        if (location[i + 1, j] == 0)
                        {
                            tile.transform.GetChild(0).gameObject.SetActive(true);
                            tile.transform.GetChild(0).gameObject.layer = 9;
                        }
                    }
                    if (j == 0)
                    {
                        tile.transform.GetChild(3).gameObject.SetActive(true);
                        tile.transform.GetChild(3).gameObject.layer = 9;
                    }
                    else
                    {
                        if (location[i, j - 1] == 0)
                        {
                            tile.transform.GetChild(3).gameObject.SetActive(true);
                            tile.transform.GetChild(3).gameObject.layer = 9;
                        }
                    }
                    if (j == m - 1)
                    {
                        tile.transform.GetChild(2).gameObject.SetActive(true);
                        tile.transform.GetChild(2).gameObject.layer = 9;
                    }
                    else
                    {
                        if (location[i, j + 1] == 0)
                        {
                            tile.transform.GetChild(2).gameObject.SetActive(true);
                            tile.transform.GetChild(2).gameObject.layer = 9;
                        }
                    }
                }
            }
        }
        NavMeshSurface surface = GetComponent<NavMeshSurface>();
        if (surface != null)
        {
            surface.BuildNavMesh();

            if (enemyObject != null)
            {
                Vector3 spawnPoint = new Vector3(n * 9 / 2, 0, m * 9 / 2);
                NavMeshHit hit;

                if (NavMesh.SamplePosition(spawnPoint, out hit, 10.0f, NavMesh.AllAreas))
                {
                    enemyObject.transform.position = hit.position;

                    enemyObject.SetActive(true);
                    Debug.Log("Враг успешно размещен на Навмеше!");
                }
                else
                {
                    Debug.LogError("Не удалось найти место для врага на Навмеше!");
                }
            }
        }
        BloodSpawner spawner = GetComponent<BloodSpawner>();
        if (spawner != null)
        {
            Debug.Log("Карта готова, запускаем кровь!");
            spawner.InitializeSpawner();
        }

    }

}
