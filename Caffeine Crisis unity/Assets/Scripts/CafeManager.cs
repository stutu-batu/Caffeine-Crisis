using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [Header("Cooldown Overlays")]
    public Image coffeeCooldownOverlay;
    public Image milkCooldownOverlay;
    public Image hotWaterCooldownOverlay;
    public Image coldWaterCooldownOverlay;
    public Image iceCooldownOverlay;
    public Image chocolateCooldownOverlay;
    public Image creamCooldownOverlay;
    public Image vanillaCooldownOverlay;
    public Image caramelCooldownOverlay;

    [Header("Disabled Color Settings")]
    [SerializeField] private Color normalDisabledColor = new Color(0.5f, 0.0f, 0.0f, 0.5f);
    [SerializeField] private Color lockedDisabledColor = new Color(0.1f, 0.1f, 0.1f, 0.9f);

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
    public bool isProcessing = false;

    [Header("Tutorial UI References")]
    public GameObject tutorialPanel;
    public TextMeshProUGUI tutorialText;
    public Button tutorialNextButton;

    [Header("Tutorial State")]
    public bool isTutorialActive = false;
    public int tutorialStep = 0;

    [Header("Tutorial Hand Reference")]
    public TutorialHand tutorialHand;

    void Start()
    {
        score = SaveManager.LoadCoins();
        UpdateRecipeBook();
        UpdateUI();
        
        UpdateIngredientButtonColors();
        SetAllStationButtonsInteractable(true);

        if (SaveManager.LoadDay() == 1) // Veya ilk gün kontrolü
        {
            StartTutorial();
        }
    }

    #region Tutorial Logic

    public void StartTutorial()
    {
        isTutorialActive = true;
        tutorialStep = 0;
        if (tutorialPanel != null) tutorialPanel.SetActive(true);

        // Eğitimin başında bekletmeden hemen bir müşteri getirtelim!
        if (customerManager != null && customerManager.customerSlot != null)
        {
            // Eğer masada müşteri yoksa anında doğur
            if (!customerManager.customerSlot.gameObject.activeSelf)
            {
                customerManager.customerSlot.SetupCustomer(customerManager.defaultCustomerSprite, "Strong Espresso");
            }
        }

        NextTutorialStep();
    }

    public void NextTutorialStep()
    {
        tutorialStep++;

        switch (tutorialStep)
        {
            case 1:
                // Aşama 1: Bardağı gösterme
                if (tutorialText != null)
                    tutorialText.text = "Welcome! This cup in the center is your main station. Everything you add will stack here layer by layer.";
                if (tutorialNextButton != null) tutorialNextButton.gameObject.SetActive(true);
                SetAllStationButtonsInteractable(false);

                if (tutorialHand != null && cupContainer != null)
                    tutorialHand.PointTo(cupContainer.GetComponent<RectTransform>());
                break;

            case 2:
                // Aşama 2: İstasyonları gösterme
                if (tutorialText != null)
                    tutorialText.text = "The buttons around are your machines. Don't spam them while pouring—wait for each layer to finish! Darker buttons are locked. The red buttons means it's cooldown. You can only use one at a time.";
                if (tutorialNextButton != null) tutorialNextButton.gameObject.SetActive(true);

                if (tutorialHand != null && coffeeButton != null)
                    tutorialHand.PointTo(coffeeButton.GetComponent<RectTransform>());
                break;

            case 3:
                // Aşama 3: Kahve Butonunu Gösterme (1. Tık)
                if (tutorialText != null)
                    tutorialText.text = "The customer wants a 'Strong Espresso' (2 Coffee). Press the Coffee button to start!";
                if (tutorialNextButton != null) tutorialNextButton.gameObject.SetActive(false);
                
                SetAllStationButtonsInteractable(false);
                if (coffeeButton != null) coffeeButton.interactable = true;

                if (tutorialHand != null && coffeeButton != null)
                    tutorialHand.PointTo(coffeeButton.GetComponent<RectTransform>());
                break;

            case 4:
                // Aşama 4: Kahve Butonunu Gösterme (2. Tık)
                if (tutorialText != null)
                    tutorialText.text = "Great! The first layer is filled. Press Coffee once more to finish the Strong Espresso recipe.";
                if (tutorialNextButton != null) tutorialNextButton.gameObject.SetActive(false);
                
                SetAllStationButtonsInteractable(false);
                if (coffeeButton != null) coffeeButton.interactable = true;

                if (tutorialHand != null && coffeeButton != null)
                    tutorialHand.PointTo(coffeeButton.GetComponent<RectTransform>());
                break;

            case 5:
                // Aşama 5: Bardağı Müşteriye Sürükleme Hareketi
                if (tutorialText != null)
                    tutorialText.text = "The drink is ready! Drag the cup and drop it onto the customer to serve.";
                SetAllStationButtonsInteractable(false);

                if (tutorialHand != null && cupContainer != null && customerManager != null && customerManager.customerSlot != null)
                {
                    RectTransform cupRect = cupContainer.GetComponent<RectTransform>();
                    RectTransform customerRect = customerManager.customerSlot.GetComponent<RectTransform>();
                    tutorialHand.DragTo(cupRect, customerRect);
                }
                break;

            case 6:
                // Aşama 6: Tamamlama
                if (tutorialText != null)
                    tutorialText.text = "Awesome job on your first sale! You're ready to run the cafe on your own. Oh and one more thing, if you click the 'Trash' text you gona clear the cup.";
                if (tutorialNextButton != null) tutorialNextButton.gameObject.SetActive(true);

                if (tutorialHand != null) tutorialHand.Hide();
                break;

            case 7:
                isTutorialActive = false;
                if (tutorialPanel != null) tutorialPanel.SetActive(false);
                if (tutorialHand != null) tutorialHand.Hide();
                SetAllStationButtonsInteractable(true);
                break;
        }
    }

    #endregion

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

    float GetCalculatedCooldown(string machineType, float baseCooldown)
    {
        int level = SaveManager.LoadMachineLevel(machineType);
        float speedMultiplier = 1f - ((level - 1) * 0.15f);
        return Mathf.Max(0.3f, baseCooldown * speedMultiplier);
    }

    #region Add Ingredients

    void PlayClick()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySFX(SoundManager.Instance.buttonClickClip);
    }

    public void AddCoffee()
    {
        if (!isProcessing && GetTotalIngredients() < maxIngredients)
        {
            PlayClick();
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX(SoundManager.Instance.coffeeMachineClip);

            float speed = GetCalculatedCooldown("Coffee", baseCoffeeCooldown);
            StartCoroutine(ProcessIngredientRoutine(
                speed,
                new Color(0.3f, 0.15f, 0.05f),
                () => coffeeCount++,
                coffeeCooldownOverlay
            ));
        }
    }

    public void AddMilk()
    {
        if (!isProcessing && GetTotalIngredients() < maxIngredients)
        {
            PlayClick();
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX(SoundManager.Instance.pourLiquidClip);

            float speed = GetCalculatedCooldown("Milk", baseMilkCooldown);
            StartCoroutine(ProcessIngredientRoutine(
                speed,
                new Color(0.95f, 0.93f, 0.88f),
                () => milkCount++,
                milkCooldownOverlay
            ));
        }
    }

    public void AddHotWater()
    {
        if (!isProcessing && GetTotalIngredients() < maxIngredients)
        {
            PlayClick();
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX(SoundManager.Instance.hotWaterClip);

            StartCoroutine(ProcessIngredientRoutine(
                waterCooldown,
                new Color(0.8f, 0.2f, 0.2f, 0.6f),
                () => hotWaterCount++,
                hotWaterCooldownOverlay
            ));
        }
    }

    public void AddColdWater()
    {
        if (!isProcessing && GetTotalIngredients() < maxIngredients)
        {
            PlayClick();
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX(SoundManager.Instance.coldWaterClip);

            StartCoroutine(ProcessIngredientRoutine(
                waterCooldown,
                new Color(0.4f, 0.7f, 1f, 0.7f),
                () => coldWaterCount++,
                coldWaterCooldownOverlay
            ));
        }
    }

    public void AddIce()
    {
        if (!isProcessing && GetTotalIngredients() < maxIngredients)
        {
            PlayClick();
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX(SoundManager.Instance.iceMachineClip);

            StartCoroutine(ProcessIngredientRoutine(
                iceCooldown,
                new Color(0.8f, 0.95f, 1f, 0.6f),
                () => iceCount++,
                iceCooldownOverlay
            ));
        }
    }

    public void AddChocolate()
    {
        if (!isProcessing && GetTotalIngredients() < maxIngredients)
        {
            PlayClick();
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX(SoundManager.Instance.pumpClip);

            float speed = GetCalculatedCooldown("Syrup", baseSyrupCooldown);
            StartCoroutine(ProcessIngredientRoutine(
                speed,
                new Color(0.2f, 0.1f, 0.05f),
                () => chocolateCount++,
                chocolateCooldownOverlay
            ));
        }
    }

    public void AddCream()
    {
        if (!isProcessing && GetTotalIngredients() < maxIngredients)
        {
            PlayClick();
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX(SoundManager.Instance.creamClip);

            StartCoroutine(ProcessIngredientRoutine(
                toppingCooldown,
                new Color(1f, 0.98f, 0.95f),
                () => creamCount++,
                creamCooldownOverlay
            ));
        }
    }

    public void AddVanilla()
    {
        if (!isProcessing && GetTotalIngredients() < maxIngredients)
        {
            PlayClick();
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX(SoundManager.Instance.pumpClip);

            float speed = GetCalculatedCooldown("Syrup", baseSyrupCooldown);
            StartCoroutine(ProcessIngredientRoutine(
                speed,
                new Color(0.95f, 0.85f, 0.5f),
                () => vanillaCount++,
                vanillaCooldownOverlay
            ));
        }
    }

    public void AddCaramel()
    {
        if (!isProcessing && GetTotalIngredients() < maxIngredients)
        {
            PlayClick();
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX(SoundManager.Instance.pumpClip);

            float speed = GetCalculatedCooldown("Syrup", baseSyrupCooldown);
            StartCoroutine(ProcessIngredientRoutine(
                speed,
                new Color(0.75f, 0.4f, 0.1f),
                () => caramelCount++,
                caramelCooldownOverlay
            ));
        }
    }

    #endregion

    // Malzeme işleme rutinimiz (Cooldown görseli dolumu)
    IEnumerator ProcessIngredientRoutine(float cooldownTime, Color layerColor, System.Action onComplete, Image cooldownOverlay = null)
    {
        isProcessing = true;

        if (cooldownOverlay != null)
        {
            cooldownOverlay.gameObject.SetActive(true);
            cooldownOverlay.fillAmount = 1f;
        }

        float elapsed = 0f;
        while (elapsed < cooldownTime)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / cooldownTime;

            if (cooldownOverlay != null)
            {
                cooldownOverlay.fillAmount = 1f - progress;
            }

            yield return null;
        }

        if (cooldownOverlay != null)
        {
            cooldownOverlay.fillAmount = 0f;
            cooldownOverlay.gameObject.SetActive(false);
        }

        onComplete?.Invoke();
        AddLayerVisual(layerColor);

        isProcessing = false;

        // --- TUTORIAL KONTROLÜ (EKLENEN KISIM) ---
        if (isTutorialActive)
        {
            // 3. Adım: İlk kahve basıldı -> 4. Adıma geç
            if (tutorialStep == 3)
            {
                NextTutorialStep();
            }
            // 4. Adım: İkinci kahve basıldı -> 5. Adıma (Sürükleme) geç
            else if (tutorialStep == 4)
            {
                NextTutorialStep();
            }
        }
    }

    void SetAllStationButtonsInteractable(bool state)
    {
        if (coffeeButton != null) coffeeButton.interactable = state;
        if (milkButton != null) milkButton.interactable = state;
        if (hotWaterButton != null) hotWaterButton.interactable = state;
        if (coldWaterButton != null) coldWaterButton.interactable = state;

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

    public int GetTotalIngredients()
    {
        return coffeeCount + milkCount + hotWaterCount + coldWaterCount + iceCount + chocolateCount + creamCount + vanillaCount + caramelCount;
    }

    void AddLayerVisual(Color layerColor)
    {
        currentLayers.Add(layerColor);
        GameObject newLayer = Instantiate(liquidLayerPrefab, cupContainer);
        Image layerImage = newLayer.GetComponent<Image>();
        if (layerImage != null) layerImage.color = layerColor;
    }

    public void ServeDrink()
    {
        if (isProcessing) return;
        if (GetTotalIngredients() == 0) return;

        if (customerManager != null && customerManager.customerSlot != null && customerManager.customerSlot.gameObject.activeSelf)
        {
            string currentCustomerOrder = customerManager.customerSlot.currentOrder;
            string preparedDrink = GetPreparedDrinkName();

            if (preparedDrink == currentCustomerOrder)
            {
                Debug.Log("Correct Drink!");

                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlaySFX(SoundManager.Instance.moneyPayClip);
                }

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

                if (isTutorialActive && tutorialStep == 5)
                {
                    NextTutorialStep();
                }
            }
            else
            {
                Debug.Log("Wrong Drink!");
                score -= 40;
                if (score < 0) score = 0;
                SaveManager.SaveCoins(score);
                ResetCup();
                UpdateUI();
            }
        }
    }

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
            Debug.Log("Cup emptied into trash!");
            ResetCup();
            UpdateUI();
        }
    }
}