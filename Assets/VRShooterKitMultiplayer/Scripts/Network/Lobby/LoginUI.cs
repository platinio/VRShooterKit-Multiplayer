using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VRShooterKit.Multiplayer
{
    public class LoginUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI userName = null;
        [SerializeField] private VR_Keyboard keyboard = null;
        [SerializeField] private int playerNameMaxSize = 15;
        [SerializeField] private Button submitButton = null;
        
        private const string KDeleteKeyValue = "⌂";
       
        private void Awake()
        {
            if (keyboard != null)
            {
                keyboard.OnKeyInput += OnKeyboardInput;
            }

            if (submitButton != null)
            {
                submitButton.onClick.AddListener(OnSubmitClick);
            }
        }

        private void OnKeyboardInput(string input)
        {
            if (KDeleteKeyValue == input)
            {
                userName.text = userName.text.Substring(0, userName.text.Length - 1);
            }
            else
            {
                if (userName.text.Length >= playerNameMaxSize)
                {
                    return;
                }

                userName.text += input;
            }
        }

        private void OnSubmitClick()
        {
            if (submitButton != null)
            {
                submitButton.onClick.RemoveAllListeners();
            }

            if (keyboard != null)
            {
                keyboard.OnKeyInput -= OnKeyboardInput;
            }
            
            PhotonNetwork.LocalPlayer.NickName = userName.text;

            if (LobbyController.instance == null)
            {
                Debug.LogError("[LoginUI] Lobby controller is missing!");
                return;
            }

            LobbyController.instance.ShowLobbyUI();
        }

    }

}

