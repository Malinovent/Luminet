using System;
using UnityEngine;

namespace Mali.Utils
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        public static bool _isShuttingDown = false;

        public static bool Exists => _instance != null;

        public static T Instance
        {
            get
            {
                if (_isShuttingDown)
                {
                    //Debug.LogWarning($"Attempted to access {typeof(T).Name} after it's being destroyed. Returning null.");
                    return null;
                }

                if (_instance == null)
                {
                    // Try to find an existing instance
                    _instance = FindFirstObjectByType<T>();

                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = typeof(T).ToString() + " (Singleton)";
                        Debug.Log($"SINGLETON: Creating a new instance of {typeof(T).Name}.");
                        //DontDestroyOnLoad(singleton);
                    }
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            _isShuttingDown = false;
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                //Debug.LogWarning($"A duplicate of {gameObject.name} was found and destroyed.");
                return;
            }

            if (_instance == null)
            {
                _instance = this as T;
                //DontDestroyOnLoad(gameObject);
            }
        }

        private void OnApplicationQuit()
        {
            //Debug.LogWarning("Application is quitting.");
            _isShuttingDown = true;
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _isShuttingDown = true;
                _instance = null;
            }
            
        }
    }
}