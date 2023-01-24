using UnityEngine;
using UnityEngine.EventSystems;
using VRSDK;

namespace VRShooterKit.SimpleUI
{
    public class VR_ControllerUI : MonoBehaviour
    {
        [SerializeField] private VR_Controller controller = null;
        [SerializeField] private VR_InputButton interactButton;
        [SerializeField] private Transform interactTransform = null;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private float interactDistance = 50.0f;
        [SerializeField] private LineRenderer lineRender = null;
        [SerializeField] private GameObject pointerTip = null;

        private GameObject currentSelection = null;
        private Vector3 lastHitPoint = Vector3.zero;
        private bool isPointerDown = false;

        private PointerEventData PointerEventData
        {
            get
            {
                return new PointerEventData(EventSystem.current);
            }
        }

        private void Update()
        {
            UpdateCurrentSelection();
            UpdateLineRender();
            UpdatePointerUI();
            CheckForInput();
        }

        private void CheckForInput()
        {
            if (!isPointerDown && currentSelection != null && controller.Input.GetButton(interactButton))
            {
                isPointerDown = true;
                InvokeOnPointerClick(currentSelection);
                InvokeOnPointerClick2(currentSelection);
            }

            else if (isPointerDown && currentSelection != null && !controller.Input.GetButton(interactButton))
            {
                isPointerDown = false;
                InvokeOnPointerUp(currentSelection);
            }
        }

        private void UpdatePointerUI()
        {
            if (pointerTip == null)
            {
                return;
            }

            if (currentSelection == null)
            {
                pointerTip.SetActive(false);
            }
            else
            {
                pointerTip.SetActive(true);
                pointerTip.transform.position = lastHitPoint;
            }
        }

        private void UpdateLineRender()
        {
            lineRender.positionCount = 2;
            lineRender.SetPosition(0, interactTransform.position);
            lineRender.SetPosition(1, GetHitPoint());
        }

        private Vector3 GetHitPoint()
        {
            if (currentSelection == null)
            {
                return interactTransform.position + (interactTransform.forward * interactDistance);
            }

            return lastHitPoint;
        }

        private void UpdateCurrentSelection()
        {
            if (Physics.Raycast(interactTransform.position, interactTransform.forward, out var hitInfo , interactDistance,
                layerMask.value))
            {
                lastHitPoint = hitInfo.point;
                
                if (currentSelection != null && currentSelection != hitInfo.collider.gameObject)
                {
                    InvokeOnPointerExit(currentSelection);
                }

                if (currentSelection == null || currentSelection != hitInfo.collider.gameObject)
                {
                    currentSelection = hitInfo.collider.gameObject;
                    InvokeOnPointerEnter(currentSelection);
                }
            }
            else
            {
                if (currentSelection != null)
                {
                    InvokeOnPointerExit(currentSelection);
                    currentSelection = null;
                }
            }
        }

        private void InvokeOnPointerEnter(GameObject go)
        {
            IPointerEnterHandler[] pointerEnterHandlerArray = go.GetComponents<IPointerEnterHandler>();

            if (pointerEnterHandlerArray != null && pointerEnterHandlerArray.Length > 0)
            {
                foreach (var pointerEnterHandler in pointerEnterHandlerArray)
                {
                    pointerEnterHandler.OnPointerEnter(PointerEventData);
                }
            }
        }
        
        private void InvokeOnPointerExit(GameObject go)
        {
            IPointerExitHandler[] pointerExitHandlerArray = go.GetComponents<IPointerExitHandler>();

            if (pointerExitHandlerArray != null && pointerExitHandlerArray.Length > 0)
            {
                foreach (var pointerEnterHandler in pointerExitHandlerArray)
                {
                    pointerEnterHandler.OnPointerExit(PointerEventData);
                }
            }
        }

        private void InvokeOnPointerClick(GameObject go)
        {
            IPointerDownHandler[] pointerExitHandlerArray = go.GetComponents<IPointerDownHandler>();

            if (pointerExitHandlerArray != null && pointerExitHandlerArray.Length > 0)
            {
                foreach (var pointerEnterHandler in pointerExitHandlerArray)
                {
                    pointerEnterHandler.OnPointerDown(PointerEventData);
                }
            }
        }
        
        private void InvokeOnPointerClick2(GameObject go)
        {
            IPointerClickHandler[] pointerExitHandlerArray = go.GetComponents<IPointerClickHandler>();

            if (pointerExitHandlerArray != null && pointerExitHandlerArray.Length > 0)
            {
                foreach (var pointerEnterHandler in pointerExitHandlerArray)
                {
                    pointerEnterHandler.OnPointerClick(PointerEventData);
                }
            }
        }
        
        private void InvokeOnSelect(GameObject go)
        {
            ISelectHandler[] pointerExitHandlerArray = go.GetComponents<ISelectHandler>();

            if (pointerExitHandlerArray != null && pointerExitHandlerArray.Length > 0)
            {
                foreach (var pointerEnterHandler in pointerExitHandlerArray)
                {
                    pointerEnterHandler.OnSelect(PointerEventData);
                }
            }
        }
        
        private void InvokeOnPointerUp(GameObject go)
        {
            IPointerUpHandler[] pointerExitHandlerArray = go.GetComponents<IPointerUpHandler>();

            if (pointerExitHandlerArray != null && pointerExitHandlerArray.Length > 0)
            {
                foreach (var pointerEnterHandler in pointerExitHandlerArray)
                {
                    pointerEnterHandler.OnPointerUp(PointerEventData);
                }
            }
        }

    }
}

