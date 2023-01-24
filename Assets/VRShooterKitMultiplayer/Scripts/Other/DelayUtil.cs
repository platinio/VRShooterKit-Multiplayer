using System;
using System.Collections;
using UnityEngine;

namespace VRShooterKit
{
    public static class DelayUtil
    {
        public static IEnumerator DelayCall(float t, Action callback)
        {
            while (t > 0)
            {
                t -= Time.deltaTime;
                yield return null;
            }
            
            callback?.Invoke();
        }
    }
}

