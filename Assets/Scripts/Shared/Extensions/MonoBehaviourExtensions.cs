using System;
using System.Collections;
using UnityEngine;

namespace Shared.Extensions
{
    public static class MonoBehaviourExtensions
    {
        public static void DoNextFrame<T>(this T behaviour, Action<T> action)
            where T : MonoBehaviour
        {
            behaviour.StartCoroutine(DoNextFrameInternal(behaviour, action));
        }

        private static IEnumerator DoNextFrameInternal<T>(T behaviour, Action<T> action)
        {
            yield return 0;
            action(behaviour);
        }
    }
}