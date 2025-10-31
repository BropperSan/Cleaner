using UnityEngine;

public class LocationGeneration : MonoBehaviour
{
    private int locationVariantsAmount = 4;
    private int maxWidth = 15;
    private int maxLenght = 15;
    private int n;
    private int m;
    private GameObject[] locationVariants;
    private int[,] location;

    private void Awake()
    {
        locationVariants = new GameObject[locationVariantsAmount];
        for (int i = 0; i < locationVariantsAmount; i++)
        {
            locationVariants[i] = Resources.Load<GameObject>("LocationPrefabs/WareHouseTile" + i);
        }

        n = Random.Range(9, maxWidth);
        m = Random.Range(9, maxLenght);

        location = new int[n, m];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                if ((i == 0) || (j == 0) || (j == m - 1) || (i == n - 1))
                {
                    location[i,j] = 1;
                    continue;
                }
                location[i, j] = Random.Range(0, locationVariantsAmount);
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
                tile.transform.position = new Vector3(9 * i, 0, 9 * j);
            }
        }
    }
}
