using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public Customer customerSlot;
    public Sprite defaultCustomerSprite;

    [Header("Base Spawn Settings")]
    public float baseMinSpawnTime = 8f;
    public float baseMaxSpawnTime = 12f;

    void Start()
    {
        StartCoroutine(SpawnCustomerRoutine());
    }

    IEnumerator SpawnCustomerRoutine()
    {
        while (true)
        {
            // 1. Tabela ve Maskot Durumuna Göre Bekleme Süresini Hesapla
            float minTime = baseMinSpawnTime;
            float maxTime = baseMaxSpawnTime;

            if (SaveManager.HasSignboard()) // Tabela alındıysa süreyi düşür (10s -> 7s mantığı)
            {
                minTime -= 3f;
                maxTime -= 3f;
            }

            if (SaveManager.HasMascot()) // Maskot alındıysa ekstra 2s daha düşür (7s -> 5s mantığı)
            {
                minTime -= 2f;
                maxTime -= 2f;
            }

            // Sınırların altına düşmesini engelle
            minTime = Mathf.Max(2f, minTime);
            maxTime = Mathf.Max(4f, maxTime);

            float waitTime = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(waitTime);

            // 2. Müşteri Oluşturma ve Geçerli Tarif Seçimi
            if (customerSlot != null && !customerSlot.gameObject.activeSelf)
            {
                string validRecipe = GetValidRandomRecipe();
                
                if (!string.IsNullOrEmpty(validRecipe))
                {
                    customerSlot.SetupCustomer(defaultCustomerSprite, validRecipe);
                }
            }
        }
    }

    // Sadece malzemeleri AÇIK olan tariflerden rastgele seçer
    string GetValidRandomRecipe()
    {
        List<string> validRecipes = new List<string>();

        // Temel Tarifler (Kilit Yok)
        validRecipes.Add("Strong Espresso");
        validRecipes.Add("Sweet Latte");
        validRecipes.Add("Hot Americano");

        // Malzemeye Bağlı Tarifler
        if (SaveManager.IsIngredientUnlocked("Ice"))
        {
            validRecipes.Add("Cold Brew");
            validRecipes.Add("Iced Latte");
        }

        if (SaveManager.IsIngredientUnlocked("Chocolate"))
        {
            validRecipes.Add("Mocha");
        }

        if (SaveManager.IsIngredientUnlocked("Caramel"))
        {
            validRecipes.Add("Caramel Macchiato");
        }

        if (SaveManager.IsIngredientUnlocked("Vanilla"))
        {
            validRecipes.Add("Vanilla Latte");
        }

        if (SaveManager.IsIngredientUnlocked("Chocolate") && SaveManager.IsIngredientUnlocked("Cream"))
        {
            validRecipes.Add("Creamy Mocha");
        }

        // Açık olan tariflerden rastgele birini seç
        if (validRecipes.Count > 0)
        {
            return validRecipes[Random.Range(0, validRecipes.Count)];
        }

        return "Strong Espresso"; // Varsayılan yedek
    }
}