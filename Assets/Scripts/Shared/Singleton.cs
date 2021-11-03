using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Shared
{
    // because unity events aren't abstract/virtual methods, they can't be sealed
    // They need to be sealable so that I can ensure something runs on start.
    // this class is a way to make those functions sealable
    public abstract class BullshitClassToSealEventFunctions : MonoBehaviour
    {
        protected virtual void Start()
        {
            
        }
    }
    public abstract class Singleton<T> : BullshitClassToSealEventFunctions
        where T : Object
    {
        protected static T SingletonInstance;

        public static T Instance =>
            SingletonInstance ??= FindObjectOfType<T>()
                          ?? throw new InvalidOperationException($"No {typeof(T).Name} found in scene");

        protected sealed override void Start()
        {
            // this assert happens at start instead of on awake because this would not handle situations where the
            // script is DontDestroyOnLoad and it doesn't have enough time to get rid of other instances before the
            // assert
            Helpers.AssertIsTrueOrQuit(FindObjectsOfType<T>().Length == 1,
                $"Found more than one singleton instance of {typeof(T).Name} in the scene");
            OnSingletonStart();
        }

        protected virtual void OnSingletonStart()
        {
            
        }

        protected virtual void OnDestroy()
        {
            SingletonInstance = null;
        }
    }
}