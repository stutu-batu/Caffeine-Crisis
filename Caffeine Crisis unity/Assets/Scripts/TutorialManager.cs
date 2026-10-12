using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("UI References")]
    public GameObject tutorialPanel;        
    public TextMeshProUGUI instructionText; 
    public RectTransform handPointer;       

    [Header("Tutorial State")]
    public bool isTutorialActive = false;
    
    // Dinamik Oluşturulacak Adım Yapısı
    public class StepData
    {
        public string instructionMessage;
        public string targetGameObjectName; // Tıklanması gereken Obje / Buton Adı
    }

    private List<StepData> currentTutorialSteps = new List<StepData>();
    private int currentStepIndex = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // 1. Gün ve henüz eğitim tamamlanmadıysa aktifleşmeye hazırlan
        if (SaveManager.LoadDay() != 1 || PlayerPrefs.GetInt("TutorialCompleted", 0) == 1)
        {
            HideTutorial();
        }
    }

    // CafeManager siparişi oluşturduğunda bu metodu çağıracak
    public void BuildTutorialForOrder(List<string> requiredIngredients)
    {
        if (SaveManager.LoadDay() != 1 || PlayerPrefs.GetInt("TutorialCompleted", 0) == 1) return;

        currentTutorialSteps.Clear();

        // 1. ADIM: Her zaman Boş Bardak Al
        currentTutorialSteps.Add(new StepData { 
            instructionMessage = "Click on an EMPTY CUP to prepare the order!", 
            targetGameObjectName = "CupButton" 
        });

        // 2. ADIMLAR: Siparişteki Malzemelere Göre Dinamik Ekle
        foreach (string ingredient in requiredIngredients)
        {
            string btnName = GetButtonNameForIngredient(ingredient);
            string msg = $"Add {ingredient.ToUpper()} to the cup!";

            currentTutorialSteps.Add(new StepData { 
                instructionMessage = msg, 
                targetGameObjectName = btnName 
            });
        }

        // SON ADIM: Müşteriye Teslim Et
        currentTutorialSteps.Add(new StepData { 
            instructionMessage = "Great! Now click on the CUSTOMER to serve the drink!", 
            targetGameObjectName = "Customer" 
        });

        // Eğitimi Başlat
        StartCoroutine(StartTutorialRoutine());
    }

    private IEnumerator StartTutorialRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        isTutorialActive = true;
        if (tutorialPanel != null) tutorialPanel.SetActive(true);
        
        currentStepIndex = 0;
        ShowCurrentStep();
    }

    public void OnPlayerAction(string actionName)
    {
        if (!isTutorialActive || currentTutorialSteps.Count == 0) return;

        // Oyuncunun tıkladığı buton, o an beklenen adımla eşleşiyor mu?
        if (currentTutorialSteps[currentStepIndex].targetGameObjectName == actionName)
        {
            currentStepIndex++;

            if (currentStepIndex >= currentTutorialSteps.Count)
            {
                FinishTutorial();
            }
            else
            {
                ShowCurrentStep();
            }
        }
    }

    private void ShowCurrentStep()
    {
        StepData step = currentTutorialSteps[currentStepIndex];
        
        if (instructionText != null) 
            instructionText.text = step.instructionMessage;

        // El Görselini Hedef Butona Taşı
        GameObject targetObj = GameObject.Find(step.targetGameObjectName);
        if (targetObj != null && handPointer != null)
        {
            handPointer.gameObject.SetActive(true);
            handPointer.position = targetObj.transform.position + new Vector3(0, 50f, 0);
        }
    }

    // Malzeme isminden Sahnedeki Buton İsmini Dondüren Yardımcı Metod
    private string GetButtonNameForIngredient(string ingredient)
    {
        switch (ingredient)
        {
            case "Coffee": return "CoffeeButton";
            case "Milk": return "MilkButton";
            case "Hot Water": return "WaterButton";
            case "Ice": return "IceButton";
            case "Cream": return "CreamButton";
            case "Chocolate": return "ChocoButton";
            case "Caramel": return "CaramelButton";
            case "Vanilla": return "VanillaButton";
            default: return ingredient + "Button";
        }
    }

    private void FinishTutorial()
    {
        if (instructionText != null) 
            instructionText.text = "Well done! First order completed!";
            
        if (handPointer != null) 
            handPointer.gameObject.SetActive(false);

        PlayerPrefs.SetInt("TutorialCompleted", 1);
        PlayerPrefs.Save();
        
        StartCoroutine(HideTutorialWithDelay());
    }

    private IEnumerator HideTutorialWithDelay()
    {
        yield return new WaitForSeconds(2.0f);
        HideTutorial();
    }

    private void HideTutorial()
    {
        isTutorialActive = false;
        if (tutorialPanel != null) tutorialPanel.SetActive(false);
        if (handPointer != null) handPointer.gameObject.SetActive(false);
    }
}