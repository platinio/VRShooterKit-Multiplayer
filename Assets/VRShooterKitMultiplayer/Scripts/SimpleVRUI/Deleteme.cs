using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Deleteme : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image image;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = Color.red;
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = Color.white;
    }
}
