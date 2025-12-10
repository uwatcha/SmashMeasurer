//参考コード：https://github.com/CreativeAI2024/CreativeAI2024/blob/develop/Assets/Scripts/Base/DontDestroySingleton.cs
//上記の開発者：https://github.com/kanata0510

using UnityEngine;

public class DontDestroySingleton<T> : MonoBehaviour where T : Component
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindFirstObjectByType(typeof(T));
                if (instance == null)
                {
                    SetupInstance();
                }
            }
            return instance;
        }
    }
    public virtual void Awake()
    {
        Logger.Log($"DontDestroySingleton({typeof(T).Name}) Awaked");
        RemoveDuplicates();
    }
    
    private static void SetupInstance()
    {
        instance = (T)FindFirstObjectByType(typeof(T));
        if (instance == null)
        {
            GameObject gameObj = new GameObject();
            gameObj.name = typeof(T).Name;
            instance = gameObj.AddComponent<T>();
            DontDestroyOnLoad(gameObj);
        }
    }
    
    private void RemoveDuplicates()
    {
        if (instance == null)
        {
            instance = this as T;
            Logger.Log($"DontDestronized({typeof(T).Name})");
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}