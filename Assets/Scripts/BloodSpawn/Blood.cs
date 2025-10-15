using System;
using UnityEngine;

public class Blood : MonoBehaviour
{
    public static event Action OnBloodWiped;

    //private void OnDestroy()
    //{
    //    OnBloodWiped?.Invoke();
    //}
}
