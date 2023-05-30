using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T>: MonoBehaviour where T: MonoSingleton<T>
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError(typeof(T) + " missing");
            return _instance;
        }
    }
    // Start is called before the first frame update
    void Awake()
    {
        if (FindObjectsOfType<T>().Length > 1) Destroy(gameObject);
        else
        {
            _instance = (T)this;
        }
    }

}
