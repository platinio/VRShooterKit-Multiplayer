using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace VRShooterKit.Multiplayer
{
    public class LoadSceneButton : MonoBehaviour
    {
        [SerializeField] private string sceneName = null;

        private void Start()
        {
            var button = GetComponent<Button>();

            if (button != null)
            {
                button.onClick.AddListener(OnClick);
            }
        }

        private void OnClick()
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}

