using UnityEngine;

namespace VRShooterKit.Multiplayer
{
    public static class GameObjectExtension
    {

        public static void DontDestroyOnLoad(this GameObject go, bool enable)
        {
            if (enable)
            {
                Object.DontDestroyOnLoad(go);
            }
            else
            {
                if (go.GetComponent<RemoveDontDestroyOnLoad>() == null)
                {
                    go.AddComponent<RemoveDontDestroyOnLoad>();
                }
            }
        }
    }
}

