using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CafeManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Text scoreText;
    public Text recipeBookText;
    public Transform cupContainer;

    [Header("Station Buttons")]
    public Button coffeeButton;
    public Button milkButton;
    public Button hotWaterButton;
    public Button coldWaterButton;
    public Button iceButton;
    public Button chocolateButton;
    public Button creamButton;
    public Button vanillaButton;
    public Button caramelButton;

    [Header("Disabled Color Settings")]
    [SerializeField] private Color normalDisabledColor = new Color(0.5f, 0.0f, 0.0f, 0.5f); // İşlem esnasındaki geçici gri
    [SerializeField] private Color lockedDisabledColor = new Color(0.1f, 0.1f, 0.1f, 0.9f);  // Satın alınmamış koyu renk

    [Header("Manager References")]
    public CustomerManager customerManager;

    [Header("Prefab Template")]
    public GameObject liquidLayerPrefab;

    [Header("Cup Content Counters")]
    public List<Color> currentLayers = new List<Color>();
    public int coffeeCount = 0;
    public int milkCount = 0;
    public int hotWaterCount = 0;
    public int coldWaterCount = 0;
    public int iceCount = 0;
    public int chocolateCount = 0;
    public int creamCount = 0;
    public int vanillaCount = 0;
    public int caramelCount = 0;
    public int maxIngredients = 6;

    [Header("Base Cooldown Settings")]
    public float baseCoffeeCooldown = 1.8f;
    public float baseMilkCooldown = 1.8f;
    public float baseSyrupCooldown = 1.5f;
    public float waterCooldown = 1.0f;
    public float iceCooldown = 0.8f;
    public float toppingCooldown = 0.8f;

    [Header("Game Logic")]
    public int score = 0;
    private bool isProcessing = false;

    void Start()
    {
        score = SaveManager.LoadCoins();
        UpdateRecipeBook();
        UpdateUI();
        
        // Buton renklerini kilit durumuna göre tanımla ve erişimleri ayarla
        UpdateIngredientButtonColors();
        SetAllStationButtonsInteractable(true);
    }

    // Butonların kapalı/pasif renk tonlarını belirler
    void UpdateIngredientButtonColors()
    {
        SetButtonDisabledColor(iceButton, SaveManager.IsIngredientUnlocked("Ice"));
        SetButtonDisabledColor(chocolateButton, SaveManager.IsIngredientUnlocked("Chocolate"));
        SetButtonDisabledColor(caramelButton, SaveManager.IsIngredientUnlocked("Caramel"));
        SetButtonDisabledColor(vanillaButton, SaveManager.IsIngredientUnlocked("Vanilla"));
        SetButtonDisabledColor(creamButton, SaveManager.IsIngredientUnlocked("Cream"));
    }

    void SetButtonDisabledColor(Button btn, bool isUnlocked)
    {
        if (btn == null) return;
        ColorBlock cb = btn.colors;
        cb.disabledColor = isUnlocked ? normalDisabledColor : lockedDisabledColor;
        btn.colors = cb;
    }

    public void AddScore(int amount)
    {
        score += amount;
        SaveManager.SaveCoins(score);
        UpdateUI();
    }

    // --- MAKİNE SEVİYESİNE GÖRE BEKLEME SÜRESİ HESAPLAMA ---
    float GetCalculatedCooldown(string machineType, float baseCooldown)
    {
        int level = SaveManager.LoadMachineLevel(machineType);
        float speedMultiplier = 1f - ((level - 1) * 0.15f); 
        return Mathf.Max(0.3f, baseCooldown * speedMultiplier);
    }

    public void AddCoffee()
    {
        if (!isProcessing && GetTotalIngredients() < maxIngredients)
        {
            float speed = GetCalculatedCooldown("Coffee", baseCoffeeCooldown);
            StartCoroutine(ProcessIngredientRoutine(
                speed,
                new Color(0.3f, 0.15f, 0.05f),
                () => coffeeCount++
            ));
        }
    }

    public void AddMilk()
    {
        if (!isProcessing && GetTotalIngredients() < maxIngredients)
        {
            float speed = GetCalculatedCooldown("Milk", baseMilkCooldown);
            StartCoroutine(ProcessIngredientRoutine(
                speed,
                new Color(0.95f, 0.93f, 0.88f),
                () => milkCount++
            ));
        }
    }

    public void AddHotWater()
    {
        if (!isProcessing && GetTotalIngredients() < maxIngredients)
        {
            StartCoroutine(ProcessIngredientRoutine(
                waterCooldown,
                new Color(0.8f, 0.2f, 0.2f, 0.6f),
                () => hotWaterCount++
            ));
        }
    }

    public void AddColdWater()
    {
        if (!isProcessing && GetTotalIngredients() < maxIngredients)
        {
            StartCoroutine(ProcessIngredientRoutine(
                waterCooldown,
                new Color(0.4f, 0.7f, 1f, 0.7f),
                () => coldWaterCount++
            ));
        }
    }

    public void AddIce()
    {
        if (!isProcessing && GetTotalIngredients() < maxIngredients)
        {
            StartCoroutine(ProcessIngredientRoutine(
                iceCooldown,
                new Color(0.8f, 0.95f, 1f, 0.6f),
                () => iceCount++
            ));
        }
    }

    public void AddChocolate()
    {
        if (!isProcessing && GetTotalIngredients() < maxIngredients)
        {
            float speed = GetCalculatedCooldown("Syrup", baseSyrupCooldown);
            StartCoroutine(ProcessIngredientRoutine(
                speed,
                new Color(0.2f, 0.1f, 0.05f),
                () => chocolateCount++
            ));
        }
    }

    public void AddCream()
    {
        if (!isProcessing && GetTotalIngredients() < maxIngredients)
        {
            StartCoroutine(ProcessIngredientRoutine(
                toppingCooldown,
                new Color(1f, 0.98f, 0.95f),
                () => creamCount++
            ));
        }
    }

    public void AddVanilla()
    {
        if (!isProcessing && GetTotalIngredients() < maxIngredients)
        {
            float speed = GetCalculatedCooldown("Syrup", baseSyrupCooldown);
            StartCoroutine(ProcessIngredientRoutine(
                speed,
                new Color(0.95f, 0.85f, 0.5f),
                () => vanillaCount++
            ));
        }
    }

    public void AddCaramel()
    {
        if (!isProcessing && GetTotalIngredients() < maxIngredients)
        {
            float speed = GetCalculatedCooldown("Syrup", baseSyrupCooldown);
            StartCoroutine(ProcessIngredientRoutine(
                speed,
                new Color(0.75f, 0.4f, 0.1f),
                () => caramelCount++
            ));
        }
    }

    IEnumerator ProcessIngredientRoutine(float processTime, Color layerColor, System.Action addCountLogic)
    {
        isProcessing = true;
        SetAllStationButtonsInteractable(false);

        yield return new WaitForSeconds(processTime);

        addCountLogic.Invoke();
        AddLayerVisual(layerColor);

        isProcessing = false;
        SetAllStationButtonsInteractable(true);
    }

    void SetAllStationButtonsInteractable(bool state)
    {
        // Temel malzemeler
        if (coffeeButton != null) coffeeButton.interactable = state;
        if (milkButton != null) milkButton.interactable = state;
        if (hotWaterButton != null) hotWaterButton.interactable = state;
        if (coldWaterButton != null) coldWaterButton.interactable = state;

        // Özel malzemeler (Açık mı VE işlem devam etmiyor mu kontrolü)
        if (iceButton != null) 
            iceButton.interactable = state && SaveManager.IsIngredientUnlocked("Ice");
            
        if (chocolateButton != null) 
            chocolateButton.interactable = state && SaveManager.IsIngredientUnlocked("Chocolate");
            
        if (caramelButton != null) 
            caramelButton.interactable = state && SaveManager.IsIngredientUnlocked("Caramel");
            
        if (vanillaButton != null) 
            vanillaButton.interactable = state && SaveManager.IsIngredientUnlocked("Vanilla");
            
        if (creamButton != null) 
            creamButton.interactable = state && SaveManager.IsIngredientUnlocked("Cream");
    }

    int GetTotalIngredients()
    {
        return coffeeCount + milkCount + hotWaterCount + coldWaterCount + iceCount + chocolateCount + creamCount + vanillaCount + caramelCount;
    }

    void AddLayerVisual(Color layerColor)
    {
        currentLayers.Add(layerColor);
        GameObject newLayer = Instantiate(liquidLayerPrefab, cupContainer);
        Image layerImage = newLayer.GetComponent<Image>();
        layerImage.color = layerColor;
    }

    public void ServeDrink()
    {
        if (isProcessing) return;

        if (customerManager != null && customerManager.customerSlot != null && customerManager.customerSlot.gameObject.activeSelf)
        {
            string currentCustomerOrder = customerManager.customerSlot.currentOrder;
            string preparedDrink = GetPreparedDrinkName();

            if (preparedDrink == currentCustomerOrder)
            {
                Debug.Log("Doğru İçecek!");

                int price = GetDrinkPrice(preparedDrink);
                int cost = GetDrinkCost(preparedDrink);

                AddScore(price);

                DayManager dayManager = FindObjectOfType<DayManager>();
                if (dayManager != null)
                {
                    dayManager.RegisterSale(price, cost);
                }

                customerManager.customerSlot.CustomerLeftSatisfied();
                ResetCup();
                UpdateUI();
            }
            else
            {
                Debug.Log("Yanlış İçecek!");
                score -= 40;
                if (score < 0) score = 0;
                SaveManager.SaveCoins(score);
                ResetCup();
                UpdateUI();
            }
        }
    }

    // --- ÇEKİRDEK KALİTESİNE GÖRE PRİCE HESABI ---
    int GetDrinkPrice(string drinkName)
    {
        int basePrice = 0;

        switch (drinkName)
        {
            case "Strong Espresso": basePrice = 30; break;
            case "Sweet Latte": basePrice = 45; break;
            case "Hot Americano": basePrice = 35; break;
            case "Cold Brew": basePrice = 40; break;
            case "Iced Latte": basePrice = 50; break;
            case "Mocha": basePrice = 55; break;
            case "Caramel Macchiato": basePrice = 60; break;
            case "Vanilla Latte": basePrice = 55; break;
            case "Creamy Mocha": basePrice = 70; break;
            default: basePrice = 0; break;
        }

        int beanQuality = SaveManager.LoadBeanQuality();
        if (beanQuality == 1) basePrice = Mathf.RoundToInt(basePrice * 1.2f);
        else if (beanQuality == 2) basePrice = Mathf.RoundToInt(basePrice * 1.5f);

        return basePrice;
    }

    int GetDrinkCost(string drinkName)
    {
        switch (drinkName)
        {
            case "Strong Espresso": return 8;
            case "Sweet Latte": return 12;
            case "Hot Americano": return 9;
            case "Cold Brew": return 10;
            case "Iced Latte": return 14;
            case "Mocha": return 16;
            case "Caramel Macchiato": return 18;
            case "Vanilla Latte": return 16;
            case "Creamy Mocha": return 22;
            default: return 0;
        }
    }

    string GetPreparedDrinkName()
    {
        if (coffeeCount == 2 && milkCount == 0 && hotWaterCount == 0 && coldWaterCount == 0 && iceCount == 0 && chocolateCount == 0 && creamCount == 0 && vanillaCount == 0 && caramelCount == 0) 
            return "Strong Espresso";

        if (coffeeCount == 1 && milkCount == 2 && hotWaterCount == 0 && coldWaterCount == 0 && iceCount == 0 && chocolateCount == 0 && creamCount == 0 && vanillaCount == 0 && caramelCount == 0) 
            return "Sweet Latte";

        if (coffeeCount == 1 && hotWaterCount == 2 && milkCount == 0 && coldWaterCount == 0 && iceCount == 0 && chocolateCount == 0 && creamCount == 0 && vanillaCount == 0 && caramelCount == 0) 
            return "Hot Americano";

        if (coffeeCount == 1 && coldWaterCount == 1 && iceCount == 1 && milkCount == 0 && hotWaterCount == 0 && chocolateCount == 0 && creamCount == 0 && vanillaCount == 0 && caramelCount == 0) 
            return "Cold Brew";

        if (coffeeCount == 1 && milkCount == 1 && iceCount == 1 && hotWaterCount == 0 && coldWaterCount == 0 && chocolateCount == 0 && creamCount == 0 && vanillaCount == 0 && caramelCount == 0) 
            return "Iced Latte";

        if (coffeeCount == 1 && milkCount == 1 && chocolateCount == 1 && creamCount == 1 && hotWaterCount == 0 && coldWaterCount == 0 && iceCount == 0 && vanillaCount == 0 && caramelCount == 0) 
            return "Creamy Mocha";

        if (coffeeCount == 1 && milkCount == 1 && chocolateCount == 1 && hotWaterCount == 0 && coldWaterCount == 0 && iceCount == 0 && creamCount == 0 && vanillaCount == 0 && caramelCount == 0) 
            return "Mocha";

        if (coffeeCount == 1 && milkCount == 1 && caramelCount == 1 && hotWaterCount == 0 && coldWaterCount == 0 && iceCount == 0 && chocolateCount == 0 && creamCount == 0 && vanillaCount == 0) 
            return "Caramel Macchiato";

        if (coffeeCount == 1 && milkCount == 1 && vanillaCount == 1 && hotWaterCount == 0 && coldWaterCount == 0 && iceCount == 0 && chocolateCount == 0 && creamCount == 0 && caramelCount == 0) 
            return "Vanilla Latte";

        return "Unknown Drink";
    }

    public void ResetCup()
    {
        coffeeCount = 0;
        milkCount = 0;
        hotWaterCount = 0;
        coldWaterCount = 0;
        iceCount = 0;
        chocolateCount = 0;
        creamCount = 0;
        vanillaCount = 0;
        caramelCount = 0;
        currentLayers.Clear();

        for (int i = cupContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(cupContainer.GetChild(i).gameObject);
        }
    }

    void UpdateRecipeBook()
    {
        if (recipeBookText != null)
        {
            recipeBookText.text = "<b>--- RECIPE BOOK ---</b>\n" +
                                 "• Strong Espresso: 2 Coffee\n" +
                                 "• Sweet Latte: 1 Coffee + 2 Milk\n" +
                                 "• Hot Americano: 1 Coffee + 2 Hot Water\n" +
                                 "• Cold Brew: 1 Coffee + 1 Cold Water + 1 Ice\n" +
                                 "• Iced Latte: 1 Coffee + 1 Milk + 1 Ice\n" +
                                 "• Mocha: 1 Coffee + 1 Milk + 1 Chocolate\n" +
                                 "• Caramel Macchiato: 1 Coffee + 1 Milk + 1 Caramel\n" +
                                 "• Vanilla Latte: 1 Coffee + 1 Milk + 1 Vanilla\n" +
                                 "• Creamy Mocha: 1 Coffee + 1 Milk + 1 Chocolate + 1 Cream";
        }
    }

    public void UpdateUI()
    {
        if (scoreText != null) scoreText.text = "Score: " + score;
    }

    public void TrashCup()
    {
        if (GetTotalIngredients() > 0)
        {
            Debug.Log("Bardak çöpe döküldü!");
            ResetCup();
            UpdateUI();
        }
    }
}