using System.Collections;
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
    public GameObject coinObject;

    [Header("Personality Settings")]
    public PersonalityType currentPersonality;

    [Header("Animation & Speed Settings")]
    [Tooltip("Müşterinin masaya giriş hızı (Saniye)")]
    public float enterDuration = 0.2f;

    [Tooltip("Müşterinin masadan çıkış hızı (Saniye)")]
    public float exitDuration = 0.2f;

    [Tooltip("Müşteri kızıp kaçarken çıkış hızı (Saniye)")]
    public float unsatisfiedExitDuration = 0.3f;

    [Tooltip("Yazının daktilo efektindeki harf yazma hızı (Saniye)")]
    public float typeLetterDelay = 0.025f;

    [Tooltip("Müşterinin ne kadar aşağıdan kayarak geleceği mesafe")]
    public float spawnYOffset = -600f;

    public CafeManager cafeManager;
    private bool isWaiting = false;
    private bool hasWarnedWaiting = false;

    // Animasyon İçin Pozisyon Kontrolleri
    private RectTransform rectTransform;
    private Vector2 targetPosition; // Masadaki asıl durması gereken yer
    private Vector2 startPosition;  // Ekranın altındaki başlangıç noktası
    private bool isPositionsSet = false;

    private Coroutine typeTextCoroutine;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (!isPositionsSet)
        {
            targetPosition = rectTransform.anchoredPosition;
            startPosition = targetPosition + new Vector2(0, spawnYOffset);
            isPositionsSet = true;
        }
    }

    public void SetupCustomer(Sprite sprite, string recipeName)
    {
        customerImage.sprite = sprite;
        currentOrder = recipeName;

        currentPersonality = (PersonalityType)Random.Range(0, System.Enum.GetValues(typeof(PersonalityType)).Length);
        ApplyPersonalityStats();

        currentPatience = maxPatience;
        if (patienceSlider != null)
        {
            patienceSlider.maxValue = maxPatience;
            patienceSlider.value = maxPatience;
        }

        if (coinObject != null) coinObject.SetActive(false);

        hasWarnedWaiting = false;
        gameObject.SetActive(true);
        isWaiting = true;

        StartCoroutine(UIAnimator.MoveFromTo(rectTransform, startPosition, targetPosition, enterDuration));

        string greeting = CustomerDialogues.GetRandomSpeech(currentPersonality, CustomerDialogues.GreetingDialogues, currentOrder);
        ShowSpeechAnimated(greeting);
    }

    private void ApplyPersonalityStats()
    {
        switch (currentPersonality)
        {
            case PersonalityType.Impatient: maxPatience = 10f; break;
            case PersonalityType.Sleepy: maxPatience = 15f; break;
            case PersonalityType.Generous: maxPatience = 25f; break;
            case PersonalityType.Gourmet: maxPatience = 18f; break;
            case PersonalityType.Friendly: default: maxPatience = 23f; break;
        }
    }

    void Update()
    {
        if (!isWaiting) return;
        if (cafeManager != null && cafeManager.isTutorialActive) return;

        currentPatience -= Time.deltaTime;

        if (patienceSlider != null) patienceSlider.value = currentPatience;

        if (!hasWarnedWaiting && currentPatience <= maxPatience / 2f)
        {
            hasWarnedWaiting = true;
            string waitLine = CustomerDialogues.GetRandomSpeech(currentPersonality, CustomerDialogues.WaitingDialogues);
            ShowSpeechAnimated(waitLine);
        }

        if (currentPatience <= 0)
        {
            CustomerLeftUnsatisfied();
        }
    }

    public void ShowSpeechAnimated(string text)
    {
        if (orderBubble != null)
        {
            orderBubble.SetActive(true);
            StartCoroutine(UIAnimator.PopIn(orderBubble.transform, 0.2f));
        }

        if (orderText != null)
        {
            if (typeTextCoroutine != null) StopCoroutine(typeTextCoroutine);
            typeTextCoroutine = StartCoroutine(UIAnimator.TypeText(orderText, text, typeLetterDelay));
        }
    }

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

        string happyLine = CustomerDialogues.GetRandomSpeech(currentPersonality, CustomerDialogues.SuccessDialogues);
        ShowSpeechAnimated(happyLine);

        if (coinObject != null) coinObject.SetActive(true);

        DayManager dayManager = FindObjectOfType<DayManager>();
        if (dayManager != null) dayManager.CheckIfDayCanFullyEnd();

        StartCoroutine(LeaveSequence());
    }

    private IEnumerator LeaveSequence()
    {
        yield return new WaitForSeconds(1.2f);

        yield return StartCoroutine(UIAnimator.MoveFromTo(rectTransform, targetPosition, startPosition, exitDuration));

        gameObject.SetActive(false);
    }

    // DayManager erişebilsin diye public yapıldı!
    public void CustomerLeftUnsatisfied()
    {
        isWaiting = false;

        if (coinObject != null) coinObject.SetActive(false);

        DayManager dayManager = FindObjectOfType<DayManager>();
        if (dayManager != null) dayManager.CheckIfDayCanFullyEnd();

        StartCoroutine(LeaveSequenceUnsatisfied());
    }

    private IEnumerator LeaveSequenceUnsatisfied()
    {
        yield return StartCoroutine(UIAnimator.MoveFromTo(rectTransform, targetPosition, startPosition, unsatisfiedExitDuration));
        gameObject.SetActive(false);
    }
}