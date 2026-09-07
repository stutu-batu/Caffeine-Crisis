using UnityEngine;

public static class SaveManager
{
    // --- COINS & DAYS ---
    public static void SaveCoins(int amount)
    {
        PlayerPrefs.SetInt("TotalCoins", amount);
        PlayerPrefs.Save();
    }

    public static int LoadCoins()
    {
        return PlayerPrefs.GetInt("TotalCoins", 0);
    }

    public static void SaveDay(int day)
    {
        PlayerPrefs.SetInt("CurrentDay", day);
        PlayerPrefs.Save();
    }

    public static int LoadDay()
    {
        PlayerPrefs.GetInt("CurrentDay", 1);
        return PlayerPrefs.GetInt("CurrentDay", 1);
    }

    public static void ClearData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("All progress reset!");
    }

    // --- MACHINE & INGREDIENT LEVELS ---
    // General Machines (Coffee, Milk - Default Level 1)
    public static int LoadMachineLevel(string machineName) => PlayerPrefs.GetInt("Machine_" + machineName, 1);
    public static void SaveMachineLevel(string machineName, int level) => PlayerPrefs.SetInt("Machine_" + machineName, level);

    // Individual Ingredient Levels (Ice, Cream, Chocolate, Caramel, Vanilla)
    public static int LoadIngredientLevel(string ingredientName) => PlayerPrefs.GetInt("Level_" + ingredientName, 1);
    public static void SaveIngredientLevel(string ingredientName, int level) => PlayerPrefs.SetInt("Level_" + ingredientName, level);

    // --- INGREDIENT UNLOCKS ---
    public static bool IsIngredientUnlocked(string ingredientName)
    {
        // Basic ingredients are unlocked by default
        if (ingredientName == "Coffee" || ingredientName == "Milk" || ingredientName == "Hot Water") return true;
        return PlayerPrefs.GetInt("Ingredient_" + ingredientName, 0) == 1;
    }

    public static void UnlockIngredient(string ingredientName) => PlayerPrefs.SetInt("Ingredient_" + ingredientName, 1);

    // --- EXTERNAL UPGRADES ---
    public static bool HasSignboard() => PlayerPrefs.GetInt("HasSignboard", 0) == 1;
    public static void SetSignboard(bool bought) => PlayerPrefs.SetInt("HasSignboard", bought ? 1 : 0);

    public static bool HasMascot() => PlayerPrefs.GetInt("HasMascot", 0) == 1;
    public static void SetMascot(bool bought) => PlayerPrefs.SetInt("HasMascot", bought ? 1 : 0);

    // --- BEAN QUALITY ---
    // 0 = Standard, 1 = Quality, 2 = Gourmet
    public static int LoadBeanQuality() => PlayerPrefs.GetInt("BeanQuality", 0);
    public static void SaveBeanQuality(int quality) => PlayerPrefs.SetInt("BeanQuality", quality);
}