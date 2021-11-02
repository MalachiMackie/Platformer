using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Shared
{
    public abstract class Singleton<T> : MonoBehaviour
        where T : Object
    {
        protected static T SingletonInstance;

        public static T Instance =>
            SingletonInstance ??= FindObjectOfType<T>()
                          ?? throw new InvalidOperationException($"No {typeof(T).Name} found in scene");

        private void OnDestroy()
        {
            SingletonInstance = null;
        }
    }
}