using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RecipeBookManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject recipeBookPanel;
    public TextMeshProUGUI recipeTitleText;
    public TextMeshProUGUI recipeIngredientsText;
    public TextMeshProUGUI pageNumberText;

    [System.Serializable]
    public class RecipeData
    {
        public string recipeName;
        public string ingredients;
    }

    [Header("Recipes List")]
    public List<RecipeData> recipes = new List<RecipeData>();
    private int currentPageIndex = 0;

    void Start()
    {
        InitializeDefaultRecipes();
        if (recipeBookPanel != null) recipeBookPanel.SetActive(false);
    }

    void InitializeDefaultRecipes()
    {
        recipes.Clear();
        recipes.Add(new RecipeData { recipeName = "Strong Espresso", ingredients = "• 2x Coffee\n\nFiyat: 30 G" });
        recipes.Add(new RecipeData { recipeName = "Sweet Latte", ingredients = "• 1x Coffee\n• 2x Milk\n\nFiyat: 45 G" });
        recipes.Add(new RecipeData { recipeName = "Hot Americano", ingredients = "• 1x Coffee\n• 2x Hot Water\n\nFiyat: 35 G" });
        recipes.Add(new RecipeData { recipeName = "Cold Brew", ingredients = "• 1x Coffee\n• 1x Cold Water\n• 1x Ice\n\nFiyat: 40 G" });
        recipes.Add(new RecipeData { recipeName = "Iced Latte", ingredients = "• 1x Coffee\n• 1x Milk\n• 1x Ice\n\nFiyat: 50 G" });
        recipes.Add(new RecipeData { recipeName = "Mocha", ingredients = "• 1x Coffee\n• 1x Milk\n• 1x Chocolate\n\nFiyat: 55 G" });
        recipes.Add(new RecipeData { recipeName = "Caramel Macchiato", ingredients = "• 1x Coffee\n• 1x Milk\n• 1x Caramel\n\nFiyat: 60 G" });
        recipes.Add(new RecipeData { recipeName = "Vanilla Latte", ingredients = "• 1x Coffee\n• 1x Milk\n• 1x Vanilla\n\nFiyat: 55 G" });
        recipes.Add(new RecipeData { recipeName = "Creamy Mocha", ingredients = "• 1x Coffee\n• 1x Milk\n• 1x Chocolate\n• 1x Cream\n\nFiyat: 70 G" });
    }

    public void OpenRecipeBook()
    {
        if (recipeBookPanel != null)
        {
            recipeBookPanel.SetActive(true);
            currentPageIndex = 0;
            UpdatePageDisplay();
            Time.timeScale = 0f;
        }
    }

    public void CloseRecipeBook()
    {
        if (recipeBookPanel != null)
        {
            recipeBookPanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    public void NextPage()
    {
        if (currentPageIndex < recipes.Count - 1)
        {
            currentPageIndex++;
            UpdatePageDisplay();
        }
    }

    public void PreviousPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            UpdatePageDisplay();
        }
    }

    void UpdatePageDisplay()
    {
        if (recipes.Count == 0) return;

        RecipeData currentRecipe = recipes[currentPageIndex];

        if (recipeTitleText != null) recipeTitleText.text = currentRecipe.recipeName;
        if (recipeIngredientsText != null) recipeIngredientsText.text = currentRecipe.ingredients;
        if (pageNumberText != null) pageNumberText.text = $"{currentPageIndex + 1} / {recipes.Count}";
    }
}