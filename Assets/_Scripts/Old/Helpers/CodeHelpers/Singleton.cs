using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    //Lazy Instantiation removed to maintain control over object lifecycle
    //public static T Instance
    //{
    //    get
    //    {
    //        if (instance == null)
    //        {
    //            instance = FindAnyObjectByType<T>();

    //            if (instance == null)
    //            {
    //                GameObject gameObject = new GameObject(typeof(T).Name);
    //                instance = gameObject.AddComponent<T>();
    //            }
    //        }

    //        return instance;
    //    }
    //}

    public virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
        }
        else
        {
            if (Instance != this)
                Destroy(gameObject);
        }
    }
}
