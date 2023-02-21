using UnityEngine;

namespace GameCode.Utilities.Singleton
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));
                    if (_instance == null)
                    {
                        SetupInstance();
                    }
                }
                return _instance;
            }
        }
        protected virtual void Awake()
        {
            RemoveDuplicates();
        }
        private static void SetupInstance()
        {
            _instance = (T)FindObjectOfType(typeof(T));
            if (_instance == null)
            {
                GameObject gameObj = new GameObject();
                gameObj.name = typeof(T).Name;
                _instance = gameObj.AddComponent<T>();
                DontDestroyOnLoad(gameObj);
            }
        }
        private void RemoveDuplicates()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}