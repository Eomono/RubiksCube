using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T instance;
    
    public static T Instance
    {
        get
        {
            if (instance != null) return instance;
            
            instance = FindObjectOfType<T> ();
            if (instance != null) return instance;
            
            instance = (new GameObject(typeof(T).Name)).AddComponent<T>();
            return instance;
        }
    }
 
    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy (gameObject);
    }
}