using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour
    where T : MonoBehaviour, new()
{
    private static T _singleton = null;

    public virtual void Awake()
    {
        if (_singleton == null)
        {
            _singleton = this as T;
            GameObject.DontDestroyOnLoad(_singleton);
        }
    }

    public static T Instance
    {
        get
        {
            if (_singleton != null) return _singleton;
            var go = new GameObject(typeof(T).ToString());
            GameObject.DontDestroyOnLoad(go);
            _singleton = go.AddComponent<T>();
            return _singleton;
        }
    }

    public static void Close() {
        var cName = typeof(T).ToString();
        var cObject = GameObject.Find(cName);
        if (cObject != null) {
            GameObject.Destroy(cObject);
        }

        _singleton = null;
    }
}