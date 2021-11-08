using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Shared
{
    public static class Helpers
    {
        public static IEnumerator DoAfterMilliseconds(int milliseconds, Action action)
        {
            yield return new WaitForSeconds(1f / milliseconds);
            action();
        }

        public static IEnumerator DoAfterMilliseconds<T>(int milliseconds, T param, Action<T> action)
        {
            yield return new WaitForSeconds(milliseconds / 1000f);
            action(param);
        }

        public static bool IsDontDestroyOnLoad(GameObject go)
        {
            return go.scene.buildIndex == -1;
        }

        public static IEnumerator DoNextFrame(Action action)
        {
            yield return 0;
            action();
        }

        public static IEnumerator DoNextPhysicsUpdate<T>(T param, Action<T> action)
        {
            yield return new WaitForFixedUpdate();
            action(param);
        }

        public static void AssertScriptFieldIsAssignedOrQuit<TScript, T>(TScript scriptObject, Expression<Func<TScript, T>> expression)
            where T : class
        {
            try
            {
                var assert = expression.Compile()(scriptObject);
                Assert.IsNotNull(assert);
            }
            catch (AssertionException)
            {
                if (expression.Body is MemberExpression memberExpression)
                {
                    var memberName = memberExpression.Member.Name;
                    Debug.LogError($"{typeof(TScript).Name}.{memberName} is not assigned");   
                }
                else
                {
                    Debug.LogError($"{nameof(AssertScriptFieldIsAssignedOrQuit)} assertion failed");
                }
                Quit();
            }
        }

        public static void AssertScriptFieldIsAssignedOrQuit<T>(T assert, string nameofScript, string nameofField)
            where T : class
        {
            try
            {
                Assert.IsNotNull(assert);
            }
            catch (AssertionException)
            {
                Debug.LogError($"{nameofScript}.{nameofField} is not assigned");
                Quit();
            }
        }

        public static void AssertIsNotNullOrQuit<T>(T assert, string message)
            where T : class
        {
            try
            {
                Assert.IsNotNull(assert, message);
            }
            catch (AssertionException e)
            {
                Debug.LogError(e.Message);
                Quit();
            }
        }

        public static void AssertIsTrueOrQuit(bool condition, string message)
        {
            try
            {
                Assert.IsTrue(condition, message);
            }
            catch (AssertionException e)
            {
                Debug.LogError(e);
                Quit();
            }
        }

        public static TComponent AssertGameObjectHasComponent<TComponent>(GameObject gameObject)
            where TComponent : Component
        {
            var component = gameObject.GetComponent<TComponent>();
            AssertIsNotNullOrQuit(component, $"GameObject {gameObject.name} does not have {typeof(TComponent).Name} component");
            return component;
        }
        
        public static void Quit()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public static IReadOnlyCollection<T> FindWithInterface<T>()
        {
            var found = new List<T>();
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                
                found.AddRange(SceneManager
                    .GetSceneAt(i)
                    .GetRootGameObjects()
                    .SelectMany(x => x.GetComponentsInChildren<T>(includeInactive: true)));
            }

            return found;
        }

        public static void DispatchEvent<T>(Action<T> func, bool allowNoHandlers = false)
            where T : IEventSystemHandler
        {
            var foundHandlers = FindWithInterface<T>();
            AssertIsTrueOrQuit(foundHandlers.Any() || allowNoHandlers, $"No handlers found for event {typeof(T).Name}");
            foreach (var eventHandler in foundHandlers)
            {
                func(eventHandler);
            }
        }

        public static void EnsureAllEventTargetsHaveImplementations()
        {
            var handlers = new List<Type>();
            var behaviours = new List<Type>();
            var allTypes = typeof(Helpers).Assembly.GetTypes();

            var monoBehaviourType = typeof(MonoBehaviour);
            var eventHandlerType = typeof(IEventSystemHandler);

            foreach (var type in allTypes)
            {
                if (type.IsSubclassOf(monoBehaviourType))
                {
                    behaviours.Add(type);
                    continue;
                }

                if (type.IsInterface && type.GetInterfaces().Any(x => x == eventHandlerType))
                {
                    handlers.Add(type);
                }
            }

            var missingImplementations = new List<Type>();
            
            foreach (var eventSystemHandler in handlers)
            {
                if (!behaviours.Any(x => eventSystemHandler.IsAssignableFrom(x)))
                {
                    missingImplementations.Add(eventSystemHandler);
                }
            }

            AssertIsTrueOrQuit(!missingImplementations.Any(),
                $"The following event handlers are missing implementations: \n{string.Join(",", missingImplementations.Select(x => x.Name))}");
        }
    }
}