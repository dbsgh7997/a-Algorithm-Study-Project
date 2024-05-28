using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoBaseSingleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;
    public static T getInstance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<T>();

                if(_instance == null)
                {
                    GameObject container = new GameObject($"Singleton {typeof(T)}");
                    _instance = container.AddComponent<T>();
                }
            }

            return _instance;
        }
    }    
}
