using System;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VRShooterKit.Multiplayer
{
    public class CreateRoomUI : MonoBehaviourPunCallbacks
    {
        [SerializeField] private VR_Keyboard keyboard = null;
        [SerializeField] private TMP_InputField roomNameInput = null;
        [SerializeField] private NumericInput maxPlayerCount = null;
        [SerializeField] private NumericInput gameTime = null;
        [SerializeField] private Button submitButton = null;
        [SerializeField] private Button backButton = null;

        private const string KDeleteKeyValue = "⌂";

        private void Start()
        {
            if (keyboard != null)
            {
                keyboard.OnKeyInput += OnKeyboardInput;
            }

            if (submitButton != null)
            {
                submitButton.onClick.AddListener(Submit);
            }

            if (backButton != null)
            {
                backButton.onClick.AddListener(BackButtonClick);
            }
        }

        private void BackButtonClick()
        {
            gameObject.SetActive(false);
            LobbyController.instance.ShowLobbyUI();
        }

        private void OnKeyboardInput(string text)
        {
            if (KDeleteKeyValue == text)
            {
                roomNameInput.text = roomNameInput.text.Substring(0, roomNameInput.text.Length - 1);
            }
            else
            {
                roomNameInput.text += text;
            }
        }

        private void Submit()
        { 
            RoomController.instance.CreateRoom(roomNameInput.text, maxPlayerCount.Value, gameTime.Value);
        }

        public override void OnCreatedRoom()
        {
            LobbyController.instance.ShowRoomUI();
        }
    }
}

