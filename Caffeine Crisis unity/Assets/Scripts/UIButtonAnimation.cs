using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonAnimation : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        StartCoroutine(UIAnimator.ButtonClickEffect(transform));
    }
}