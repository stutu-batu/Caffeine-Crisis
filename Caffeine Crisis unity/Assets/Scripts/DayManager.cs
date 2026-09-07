using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DayManager : MonoBehaviour
{
    [Header("Time Settings")]
    public float dayDurationInSeconds = 60f; 
    private float timer = 0f;
    public bool isDayActive = true;

    [Header("UI Elements (TMP)")]
    public TMP_Text clockText;
    public TMP_Text currentDayText;

    [Header("Day End Summary UI (TMP)")]
    public GameObject dayEndPanel;
    public TMP_Text summaryDayTitleText;
    public TMP_Text earningsText;
    public TMP_Text rentExpenseText;
    public TMP_Text ingredientExpenseText;
    public TMP_Text netProfitText;
    public Button continueButton;

    [Header("Daily Accounting")]
    public int dailyEarnings = 0;
    public int dailyRent = 20;
    public int dailyIngredientCost = 0;

    [Header("References")]
    public CafeManager cafeManager;
    public CustomerManager customerManager;

    private int currentDay = 1;
    private bool isWaitingForLastCustomer = false;

    void Start()
    {
        currentDay = PlayerPrefs.GetInt("CurrentDay", 1);
        
        if (dayEndPanel != null) dayEndPanel.SetActive(false);
        if (currentDayText != null) currentDayText.text = "DAY: " + currentDay;

        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(OnContinueButtonClicked);
        }

        timer = 0f;
        isDayActive = true;
        isWaitingForLastCustomer = false;
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (!isDayActive) return;

        timer += Time.deltaTime;
        UpdateClockUI();

        if (timer >= dayDurationInSeconds && !isWaitingForLastCustomer)
        {
            TryEndDay();
        }
    }

    void UpdateClockUI()
    {
        float progress = Mathf.Clamp01(timer / dayDurationInSeconds);
        int startHour = 9;
        int endHour = 21;
        
        int hours = Mathf.FloorToInt(Mathf.Lerp(startHour, endHour, progress));

        if (clockText != null)
        {
            clockText.text = string.Format("{0:00}:00", hours);
        }
    }

    public void RegisterSale(int amount, int ingredientCost)
    {
        dailyEarnings += amount;
        dailyIngredientCost += ingredientCost;
    }

    void TryEndDay()
    {
        isWaitingForLastCustomer = true;

        if (customerManager != null && customerManager.customerSlot != null && customerManager.customerSlot.gameObject.activeSelf)
        {
            Debug.Log("Gün süresi bitti, son müşterinin gitmesi bekleniyor...");
        }
        else
        {
            FinalizeDay();
        }
    }

    public void CheckIfDayCanFullyEnd()
    {
        if (isWaitingForLastCustomer)
        {
            FinalizeDay();
        }
    }

    void FinalizeDay()
    {
        isDayActive = false;
        Time.timeScale = 0f;

        
        int totalExpenses = dailyRent + dailyIngredientCost;
        int netProfit = dailyEarnings - totalExpenses;

        if (cafeManager != null)
        {
            
            cafeManager.AddScore(-totalExpenses);
        }

        ShowSummary(netProfit);
    }

    void ShowSummary(int netProfit)
    {
        if (dayEndPanel != null)
        {
            dayEndPanel.SetActive(true);

            if (summaryDayTitleText != null) summaryDayTitleText.text = "Day: " + currentDay;
            if (earningsText != null) earningsText.text = "Earnings: " + dailyEarnings + " G";
            if (rentExpenseText != null) rentExpenseText.text = "Rent: -" + dailyRent + " G";
            if (ingredientExpenseText != null) ingredientExpenseText.text = "Ingredient: -" + dailyIngredientCost + " G";
            if (netProfitText != null) netProfitText.text = "Net Profit: " + netProfit + " G";
        }
    }

    public void OnContinueButtonClicked()
    {
        currentDay++;
        PlayerPrefs.SetInt("CurrentDay", currentDay);
        PlayerPrefs.Save();

        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}