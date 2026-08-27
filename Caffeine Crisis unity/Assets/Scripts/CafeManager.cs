using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CafeManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Text scoreText;
    public Text orderText;
    public Text recipeBookText;
    public Transform cupContainer;
    public Image timerBarFill;

    [Header("Prefab Template")]
    public GameObject liquidLayerPrefab;

    [Header("Cup Content Counters")]
    public List<Color> currentLayers = new List<Color>();
    public int coffeeCount = 0;
    public int milkCount = 0;
    public int waterCount = 0;
    public int maxIngredients = 5;

    [Header("Game Logic")]
    public int score = 0;
    public int currentOrder = 1;

    [Header("Timer Settings")]
    public float maxTime = 15f;
    private float currentTime;
    private bool isTimerRunning = false;

    void Start()
    {
        UpdateRecipeBook();
        NewOrder();
        UpdateUI();
    }

    void Update()
    {
        if (isTimerRunning)
        {
            currentTime -= Time.deltaTime;
            timerBarFill.fillAmount = currentTime / maxTime;

            if (timerBarFill.fillAmount > 0.5f)
                timerBarFill.color = Color.green;
            else if (timerBarFill.fillAmount > 0.2f)
                timerBarFill.color = Color.yellow;
            else
                timerBarFill.color = Color.red;

            if (currentTime <= 0)
            {
                CustomerLeft();
            }
        }
    }

    //INGREDIENT ACTIONS

    public void AddCoffee()
    {
        if (GetTotalIngredients() < maxIngredients)
        {
            coffeeCount++;
            AddLayerVisual(new Color(0.3f, 0.15f, 0.05f)); // Dark Coffee
        }
    }

    public void AddMilk()
    {
        if (GetTotalIngredients() < maxIngredients)
        {
            milkCount++;
            AddLayerVisual(new Color(0.95f, 0.93f, 0.88f)); // Milk
        }
    }

    public void AddWater()
    {
        if (GetTotalIngredients() < maxIngredients)
        {
            waterCount++;
            AddLayerVisual(new Color(0.4f, 0.7f, 1f, 0.7f)); // Water
        }
    }

    int GetTotalIngredients()
    {
        return coffeeCount + milkCount + waterCount;
    }

    void AddLayerVisual(Color layerColor)
    {
        currentLayers.Add(layerColor);

        GameObject newLayer = Instantiate(liquidLayerPrefab, cupContainer);
        Image layerImage = newLayer.GetComponent<Image>();
        layerImage.color = layerColor;
    }

    //SERVE AND MIX

    public void ServeDrink()
    {
        if (GetTotalIngredients() == 0) return;
        StartCoroutine(MixAndServeRoutine());
    }

    IEnumerator MixAndServeRoutine()
    {
        isTimerRunning = false;

        foreach (Transform child in cupContainer)
        {
            Destroy(child.gameObject);
        }

        Color mixedColor = CalculateMixColor();
        GameObject mixedLayer = Instantiate(liquidLayerPrefab, cupContainer);
        Image mixedImage = mixedLayer.GetComponent<Image>();
        mixedImage.color = mixedColor;

        LayoutElement layout = mixedLayer.GetComponent<LayoutElement>();
        if (layout != null) layout.preferredHeight = 160;

        yield return new WaitForSeconds(0.5f);

        bool isCorrect = CheckRecipe();

        if (isCorrect)
        {
            int timeBonus = Mathf.RoundToInt(currentTime) * 10;
            score += 100 + timeBonus;
        }
        else
        {
            score -= 40;
        }

        ResetCup();
        NewOrder();
        UpdateUI();
    }

    void CustomerLeft()
    {
        score -= 50;
        ResetCup();
        NewOrder();
        UpdateUI();
    }

    Color CalculateMixColor()
    {
        float total = GetTotalIngredients();
        float r = (coffeeCount * 0.3f + milkCount * 0.95f + waterCount * 0.4f) / total;
        float g = (coffeeCount * 0.15f + milkCount * 0.93f + waterCount * 0.7f) / total;
        float b = (coffeeCount * 0.05f + milkCount * 0.88f + waterCount * 1.0f) / total;
        return new Color(r, g, b);
    }

    bool CheckRecipe()
    {
        // 1: Strong Espresso (2 Coffee)
        if (currentOrder == 1 && coffeeCount == 2 && milkCount == 0 && waterCount == 0) return true;
        // 2: Sweet Latte (1 Coffee, 2 Milk)
        if (currentOrder == 2 && coffeeCount == 1 && milkCount == 2 && waterCount == 0) return true;
        // 3: Smooth Americano (1 Coffee, 1 Milk, 2 Water)
        if (currentOrder == 3 && coffeeCount == 1 && milkCount == 1 && waterCount == 2) return true;

        return false;
    }

    public void ResetCup()
    {
        coffeeCount = 0;
        milkCount = 0;
        waterCount = 0;
        currentLayers.Clear();

        foreach (Transform child in cupContainer)
        {
            Destroy(child.gameObject);
        }
    }

    void NewOrder()
    {
        currentOrder = Random.Range(1, 4);
        currentTime = maxTime;
        isTimerRunning = true;
    }

    void UpdateRecipeBook()
    {
        if (recipeBookText != null)
        {
            recipeBookText.text = "<b>--- RECIPE BOOK ---</b>\n" +
                                 "• Strong Espresso: 2 Coffee\n" +
                                 "• Sweet Latte: 1 Coffee + 2 Milk\n" +
                                 "• Smooth Americano: 1 Coffee + 1 Milk + 2 Water";
        }
    }

    void UpdateUI()
    {
        if (scoreText != null) scoreText.text = "Score: " + score;

        if (orderText != null)
        {
            if (currentOrder == 1) orderText.text = "Order: Strong Espresso";
            else if (currentOrder == 2) orderText.text = "Order: Sweet Latte";
            else if (currentOrder == 3) orderText.text = "Order: Smooth Americano";
        }
    }
}