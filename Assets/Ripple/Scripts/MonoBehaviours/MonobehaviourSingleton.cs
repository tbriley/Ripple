using System;
using UnityEngine;

public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : UnityEngine.Object
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (null != _instance) return _instance;
            var os = FindObjectsOfType<T>();
            if (os.Length == 0)
                throw new Exception(string.Format("No instance of {0}", typeof(T)));
            if (os.Length > 1)
                Debug.LogWarning(string.Format("Multiple instances of {0}. The first registered one will be retured ({1})", typeof(T), os[0].name));
            _instance = os[0];
            return _instance;
        }
        protected set { _instance = value; }
    }

    public virtual void Awake()
    {
        Instance = this as T;
    }

    public virtual void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
