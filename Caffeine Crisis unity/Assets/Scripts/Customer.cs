using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Customer : MonoBehaviour, IDropHandler
{
    public string currentOrder;
    public float maxPatience = 15f;
    private float currentPatience;

    [Header("UI References")]
    public Image customerImage;
    public GameObject orderBubble;
    public TextMeshProUGUI orderText;
    public Slider patienceSlider;
    public GameObject coinObject; // Masada belirecek para görseli/butonu

    public CafeManager cafeManager;
    private bool isWaiting = false;

    public void SetupCustomer(Sprite sprite, string recipeName)
    {
        customerImage.sprite = sprite;
        currentOrder = recipeName;
        orderText.text = recipeName;
        
        currentPatience = maxPatience;
        patienceSlider.maxValue = maxPatience;
        patienceSlider.value = maxPatience;

        // Müşteri ilk geldiğinde para KAPALI olmalı
        

        gameObject.SetActive(true);
        isWaiting = true;
    }

    void Update()
    {
        if (!isWaiting) return;

        currentPatience -= Time.deltaTime;
        patienceSlider.value = currentPatience;

        if (currentPatience <= 0)
        {
            CustomerLeftUnsatisfied();
        }
    }

    // Bardak müşterinin üzerine bırakıldığında otomatik çalışır
    public void OnDrop(PointerEventData eventData)
    {
        DraggableCup cup = eventData.pointerDrag?.GetComponent<DraggableCup>();
        if (cup != null && isWaiting)
        {
            cafeManager.ServeDrink();
        }
    }

    public void CustomerLeftSatisfied()
    {
        isWaiting = false;
        gameObject.SetActive(false); // Müşteri görseli kaybolur
        
        // Sadece doğru servis yapıldığında para görünür
        if (coinObject != null)
        {
            coinObject.SetActive(true);
        }

        DayManager dayManager = FindObjectOfType<DayManager>();
        if (dayManager != null)
        {
            dayManager.CheckIfDayCanFullyEnd();
        }
    }

    void CustomerLeftUnsatisfied()
    {
        isWaiting = false;
        gameObject.SetActive(false);
        
        // Müşteri kızıp giderse para çıkmasın
        if (coinObject != null)
        {
            coinObject.SetActive(false);
        }

        DayManager dayManager = FindObjectOfType<DayManager>();
        if (dayManager != null)
        {
            dayManager.CheckIfDayCanFullyEnd();
        }
    }
}