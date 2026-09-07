using UnityEngine;
using UnityEngine.EventSystems;

public class Coin : MonoBehaviour, IPointerClickHandler
{
    public int coinValue = 100;
    public CafeManager cafeManager;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (cafeManager != null)
        {
            cafeManager.score += coinValue;
            cafeManager.UpdateUI();
        }
        
        // Parayı topladık, nesneyi yok et veya gizle
        gameObject.SetActive(false);
    }
}