using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace VRShooterKit.Multiplayer
{
    public class VR_Key : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private TextMeshProUGUI text = null;

        public event Action<string> OnClick = null;
        
        public void OnPointerDown(PointerEventData eventData)
        {
            OnClick?.Invoke(text.text);
        }
    }
}

