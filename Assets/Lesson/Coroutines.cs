
using System.Collections;
using UnityEngine;

public sealed class Coroutines : MonoBehaviour
{
    private static Coroutines instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("[COROUTINE MANAGER]");
                _instance = go.AddComponent<Coroutines>();
                DontDestroyOnLoad(go);
            }

            return _instance;
        }
    }

    private static Coroutines _instance;

    public static Coroutine StartRoutine(IEnumerator enumerator)
    {
        return instance.StartCoroutine(enumerator);
    }

    public static void StopRoutine(Coroutine routine)
    {
        instance.StopCoroutine(routine);
    }

    public static void StopAllRoutine()
    {
        instance.StopAllCoroutines();
    }
}