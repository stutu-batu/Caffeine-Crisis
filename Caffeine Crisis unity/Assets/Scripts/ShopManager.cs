using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("UI General")]
    public TextMeshProUGUI coinsText;

    [Header("Machine Upgrade Texts")]
    public TextMeshProUGUI coffeeBtnText;
    public TextMeshProUGUI milkBtnText;

    [Header("Upgradeable Ingredient Texts")]
    public TextMeshProUGUI iceBtnText;
    public TextMeshProUGUI creamBtnText;
    public TextMeshProUGUI chocoBtnText;
    public TextMeshProUGUI caramelBtnText;
    public TextMeshProUGUI vanillaBtnText;

    [Header("Exterior & Bean Texts")]
    public TextMeshProUGUI signboardBtnText;
    public TextMeshProUGUI mascotBtnText;
    public TextMeshProUGUI beanBtnText;

    [Header("Prices - Machines")]
    public int[] coffeeCosts = new int[] { 0, 1000, 2000, 4000, 5000 };
    public int[] milkCosts = new int[] { 0, 800, 1800, 3500, 4500 };

    [Header("Prices - Ingredients (Index 0: Buy, Index 1-4: Upgrades)")]
    public int[] iceCosts = new int[] { 500, 800, 1500, 2500, 4000 };
    public int[] creamCosts = new int[] { 800, 1200, 2000, 3200, 4500 };
    public int[] chocoCosts = new int[] { 1000, 1500, 2500, 3800, 5000 };
    public int[] caramelCosts = new int[] { 2000, 2500, 3500, 4800, 6000 };
    public int[] vanillaCosts = new int[] { 2000, 2500, 3500, 4800, 6000 };

    [Header("Prices - Exterior & Beans")]
    public int signboardCost = 2000;
    public int mascotCost = 3500;
    public int qualityBeanCost = 1500;
    public int gourmetBeanCost = 4000;

    // Baz Süreler (Saniye)
    private const float BASE_COFFEE_TIME = 1.8f;
    private const float BASE_MILK_TIME = 1.8f;
    private const float BASE_ICE_TIME = 0.8f;
    private const float BASE_CREAM_TIME = 0.8f;
    private const float BASE_SYRUP_TIME = 1.5f;

    private void Start()
    {
        RefreshShopUI();
    }

    public void RefreshShopUI()
    {
        // 1. Coins Display
        if (coinsText != null) 
            coinsText.text = "Coins: " + SaveManager.LoadCoins();

        // 2. Machine Upgrades
        UpdateMachineUI("Coffee", coffeeBtnText, coffeeCosts, BASE_COFFEE_TIME);
        UpdateMachineUI("Milk", milkBtnText, milkCosts, BASE_MILK_TIME);

        // 3. Upgradeable Ingredients (Buy + Level Up on same button)
        UpdateUpgradeableIngredientUI("Ice", iceBtnText, iceCosts, BASE_ICE_TIME);
        UpdateUpgradeableIngredientUI("Cream", creamBtnText, creamCosts, BASE_CREAM_TIME);
        UpdateUpgradeableIngredientUI("Chocolate", chocoBtnText, chocoCosts, BASE_SYRUP_TIME);
        UpdateUpgradeableIngredientUI("Caramel", caramelBtnText, caramelCosts, BASE_SYRUP_TIME);
        UpdateUpgradeableIngredientUI("Vanilla", vanillaBtnText, vanillaCosts, BASE_SYRUP_TIME);

        // 4. Exterior Upgrades
        if (signboardBtnText != null)
            signboardBtnText.text = SaveManager.HasSignboard() ? "BOUGHT" : $"Signboard\n{signboardCost} G";
        if (mascotBtnText != null)
            mascotBtnText.text = SaveManager.HasMascot() ? "BOUGHT" : $"Mascot\n{mascotCost} G";

        // 5. Bean Quality
        if (beanBtnText != null)
        {
            int q = SaveManager.LoadBeanQuality();
            if (q == 0) beanBtnText.text = $"Quality Beans\n{qualityBeanCost} G";
            else if (q == 1) beanBtnText.text = $"Gourmet Beans\n{gourmetBeanCost} G";
            else beanBtnText.text = "MAX QUALITY";
        }
    }

    // Temel Makine UI Güncelleme (Coffee, Milk)
    private void UpdateMachineUI(string name, TextMeshProUGUI btnText, int[] costs, float baseTime)
    {
        if (btnText == null) return;
        int lvl = SaveManager.LoadMachineLevel(name);
        float currentSpeed = CalculateSpeed(baseTime, lvl);

        if (lvl >= 5)
            btnText.text = $"{name} Machine\nMAX Lvl ({currentSpeed:F1}s)";
        else
        {
            float nextSpeed = CalculateSpeed(baseTime, lvl + 1);
            btnText.text = $"{name} Lvl {lvl} -> {lvl + 1}\nSpeed: {currentSpeed:F1}s -> {nextSpeed:F1}s\n{costs[lvl]} G";
        }
    }

    // Satın Alınan & Seviye Atlatılan Malzemelerin UI Güncellemesi
    private void UpdateUpgradeableIngredientUI(string name, TextMeshProUGUI btnText, int[] costs, float baseTime)
    {
        if (btnText == null) return;

        bool isUnlocked = SaveManager.IsIngredientUnlocked(name);

        if (!isUnlocked)
        {
            // Henüz satın alınmamışsa
            btnText.text = $"Buy {name}\n{costs[0]} G";
        }
        else
        {
            // Satın alınmışsa seviyesini göster
            int lvl = SaveManager.LoadIngredientLevel(name);
            float currentSpeed = CalculateSpeed(baseTime, lvl);

            if (lvl >= 5)
            {
                btnText.text = $"{name}\nMAX Lvl ({currentSpeed:F1}s)";
            }
            else
            {
                float nextSpeed = CalculateSpeed(baseTime, lvl + 1);
                btnText.text = $"{name} Lvl {lvl} -> {lvl + 1}\nSpeed: {currentSpeed:F1}s -> {nextSpeed:F1}s\n{costs[lvl]} G";
            }
        }
    }

    private float CalculateSpeed(float baseTime, int level)
    {
        float speedMultiplier = 1f - ((level - 1) * 0.15f);
        return Mathf.Max(0.2f, baseTime * speedMultiplier);
    }

    // --- BUTTON ACTIONS ---

    public void UpgradeCoffeeMachine() => TryUpgradeMachine("Coffee", coffeeCosts);
    public void UpgradeMilkMachine() => TryUpgradeMachine("Milk", milkCosts);

    // Hem Satın Alma Hem Seviye Atlatma Metodu
    public void ProcessIngredientButton(string name)
    {
        bool isUnlocked = SaveManager.IsIngredientUnlocked(name);
        int[] costs = GetIngredientCosts(name);

        if (costs == null) return;

        if (!isUnlocked)
        {
            // 1. Durum: Satın Alma
            if (TryDeductCoins(costs[0]))
            {
                SaveManager.UnlockIngredient(name);
                SaveManager.SaveIngredientLevel(name, 1);
                RefreshShopUI();
            }
        }
        else
        {
            // 2. Durum: Seviye Yükseltme
            int lvl = SaveManager.LoadIngredientLevel(name);
            if (lvl >= 5) return;

            if (TryDeductCoins(costs[lvl]))
            {
                SaveManager.SaveIngredientLevel(name, lvl + 1);
                RefreshShopUI();
            }
        }
    }

    private int[] GetIngredientCosts(string name)
    {
        switch (name)
        {
            case "Ice": return iceCosts;
            case "Cream": return creamCosts;
            case "Chocolate": return chocoCosts;
            case "Caramel": return caramelCosts;
            case "Vanilla": return vanillaCosts;
            default: return null;
        }
    }

    private void TryUpgradeMachine(string name, int[] costs)
    {
        int lvl = SaveManager.LoadMachineLevel(name);
        if (lvl >= 5) return;
        if (TryDeductCoins(costs[lvl]))
        {
            SaveManager.SaveMachineLevel(name, lvl + 1);
            RefreshShopUI();
        }
    }

    public void BuySignboard()
    {
        if (!SaveManager.HasSignboard() && TryDeductCoins(signboardCost))
        {
            SaveManager.SetSignboard(true);
            RefreshShopUI();
        }
    }

    public void BuyMascot()
    {
        if (!SaveManager.HasMascot() && TryDeductCoins(mascotCost))
        {
            SaveManager.SetMascot(true);
            RefreshShopUI();
        }
    }

    public void BuyNextBeanQuality()
    {
        int q = SaveManager.LoadBeanQuality();
        if (q == 0 && TryDeductCoins(qualityBeanCost))
        {
            SaveManager.SaveBeanQuality(1);
            RefreshShopUI();
        }
        else if (q == 1 && TryDeductCoins(gourmetBeanCost))
        {
            SaveManager.SaveBeanQuality(2);
            RefreshShopUI();
        }
    }

    private bool TryDeductCoins(int cost)
    {
        int currentCoins = SaveManager.LoadCoins();
        if (currentCoins >= cost)
        {
            SaveManager.SaveCoins(currentCoins - cost);
            return true;
        }
        Debug.Log("Not enough coins!");
        return false;
    }
}