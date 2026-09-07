using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Display")]
    public TextMeshProUGUI infoText;

    [Header("Panels")]
    public GameObject mainButtonsPanel; 
    public GameObject shopPanel;        

    void Start()
    {
        UpdateMenuUI();
        if (shopPanel != null) shopPanel.SetActive(false); 
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void HowToPlay()
    {
        SceneManager.LoadScene("HowToPlay");
    }

    public void OpenShop()
    {
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(false);
        if (shopPanel != null) shopPanel.SetActive(true);
    }

    public void CloseShop()
    {
        if (shopPanel != null) shopPanel.SetActive(false);
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(true);
        UpdateMenuUI(); 
    }

    public void ResetProgress()
    {
        SaveManager.ClearData();
        UpdateMenuUI();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void UpdateMenuUI()
    {
        if (infoText != null)
        {
            int savedDay = SaveManager.LoadDay();
            int savedCoins = SaveManager.LoadCoins();
            infoText.text = $"Day: {savedDay} | Coins: {savedCoins}";
        }
    }
}