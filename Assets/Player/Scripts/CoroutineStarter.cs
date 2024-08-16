using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineStarter : MonoBehaviour
{
    private static CoroutineStarter Instance;
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static Coroutine Start(IEnumerator enumerator)
    {
        if (Instance == null)
        {
            return null;
        }
        return Instance.StartCoroutine(enumerator);
    }
}
