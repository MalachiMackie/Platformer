using System;
using System.Collections;
using System.Linq.Expressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

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

        public static IEnumerator DoNextFrame<T>(T param, Action<T> action)
        {
            yield return 0;
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
#if DEBUG
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}