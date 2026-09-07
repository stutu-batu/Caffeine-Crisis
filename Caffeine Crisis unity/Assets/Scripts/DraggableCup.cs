using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableCup : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 originalPosition;
    private CanvasGroup canvasGroup;
    public float dropDistanceThreshold = 180f;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = transform.position;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        CafeManager cafeManager = FindObjectOfType<CafeManager>();

        if (cafeManager != null)
        {
            if (cafeManager.customerManager != null && cafeManager.customerManager.customerSlot != null && cafeManager.customerManager.customerSlot.gameObject.activeSelf)
            {
                Transform customerTransform = cafeManager.customerManager.customerSlot.transform;
                float distanceToCustomer = Vector2.Distance(transform.position, customerTransform.position);

                if (distanceToCustomer <= dropDistanceThreshold)
                {
                    cafeManager.ServeDrink();
                    ResetPosition();
                    return;
                }
            }
        }

        ResetPosition();
    }

    public void ResetPosition()
    {
        transform.position = originalPosition;
    }
}