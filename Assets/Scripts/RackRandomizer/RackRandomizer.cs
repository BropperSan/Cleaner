using UnityEngine;

public class RackRandomizer : MonoBehaviour
{
    private Transform[] shelfsCenter;
    private void Awake()
    {
        shelfsCenter = GetComponentsInChildren<Transform>();
    }
    void Start()
    {
        if (shelfsCenter != null)
        {
            for (int i = 1; i < shelfsCenter.Length; i++)
            {
                GameObject prop = Resources.Load<GameObject>("Props/RackProp" + Random.Range(1, 21));
                GameObject instance = Instantiate(prop, shelfsCenter[i].position, Quaternion.identity, shelfsCenter[i]);
                instance.transform.localPosition = prop.transform.position;
                instance.transform.rotation = prop.transform.rotation;
            }
        }
    }
}
