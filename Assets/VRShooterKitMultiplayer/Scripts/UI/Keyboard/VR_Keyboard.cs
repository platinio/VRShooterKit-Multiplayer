using System;
using UnityEngine;

namespace VRShooterKit.Multiplayer
{
    public class VR_Keyboard : MonoBehaviour
    {
        private VR_Key[] keyArray;

        public event Action<string> OnKeyInput;

        private void Start()
        {
            keyArray = transform.GetComponentsInChildren<VR_Key>();

            foreach (var key in keyArray)
            {
                key.OnClick += OnKeyPressed;
            }
        }

        private void OnKeyPressed(string text)
        {
            OnKeyInput?.Invoke(text);
        }

    }
}

