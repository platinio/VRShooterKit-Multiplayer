using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VRShooterKit.Multiplayer
{
    public class NumericInput : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI valueLabel = null;
        [SerializeField] private Button increaseValueButton = null;
        [SerializeField] private Button decreaseValueButton = null;
        [SerializeField] private int startValue = 5;
        [SerializeField] private int minValue = 2;
        [SerializeField] private int maxValue = 10;

        private int currentValue = 0;

        public int Value => currentValue;

        private void Start()
        {
            ModifyValue(startValue);
            
            increaseValueButton.onClick.AddListener(delegate { ModifyValue(1); });
            decreaseValueButton.onClick.AddListener(() => { ModifyValue(-1); });
        }

        private void ModifyValue(int value)
        {
            currentValue = Mathf.Clamp(currentValue + value, minValue, maxValue);
            valueLabel.text = currentValue.ToString();
        }

    }
}

