using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using JetBrains.Annotations;
using Unity.Services.Core;
using com.unity3d.mediation;

using System.Collections;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenuCollection;
    public GameObject levelSelectCollection;

    public CanvasGroup mainMenuGroup;
    public CanvasGroup levelSelectGroup;

    public float transitionDuration = 0.5f;
    public Button continueButton;

    public Button back;

    public Button newGameButton;

    public Button levelSelect;

    public Button[] levelButtons;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        levelSelectCollection.SetActive(false);

        levelSelectGroup.alpha = 0;

        back.onClick.AddListener(BackToMainMenu);
        levelSelect.onClick.AddListener(ShowLevelSelect);

        int unlockedLevel = SaveManager.getUnlockedLevel();

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1;

            levelButtons[i].interactable = levelIndex <= unlockedLevel;

            int indexCopy = levelIndex;
            levelButtons[i].onClick.AddListener(() => LoadLevel(indexCopy));
        }

        SaveManager.Load(out int lives, out int rings, out string scene);

        if (lives <= 0)
        {
            continueButton.interactable = false;
            if (PlayerPrefs.HasKey("NextLifeTime"))
            {
                string storedTime = PlayerPrefs.GetString("NextLifeTime");
                System.DateTime nextLifeTime = System.DateTime.Parse(storedTime);
                if (System.DateTime.UtcNow >= nextLifeTime)
                {
                    PlayerController.lives = 1;
                    PlayerPrefs.DeleteKey("nextLifeTime");
                    continueButton.interactable = true;
                    SaveManager.Save(PlayerController.lives, rings);
                }
                else
                {
                    System.TimeSpan timeRemaining = nextLifeTime - System.DateTime.UtcNow;
                    Debug.Log("time remaining " + timeRemaining.ToString(@"mm\:ss"));
                }
            }
        }
        else
        {
            continueButton.interactable = true;
        }
        //Listeners to buttons
        continueButton.onClick.AddListener(ContinueGame);
        newGameButton.onClick.AddListener(NewGame);
        // settings.onClick.AddListener(Settings);

        continueButton.interactable = PlayerPrefs.HasKey("SavedGame");

    }

    public void ShowLevelSelect()
    {
        StartCoroutine(FadePanels(mainMenuGroup, levelSelectGroup));
        mainMenuCollection.SetActive(false);
        levelSelectCollection.SetActive(true);
    }

    public void BackToMainMenu()
    {
        StartCoroutine(FadePanels(levelSelectGroup, mainMenuGroup));
        levelSelectCollection.SetActive(false);
        mainMenuCollection.SetActive(true);
    }

    IEnumerator FadePanels(CanvasGroup from, CanvasGroup to)
    {
        float elapsed = 0f;

        from.blocksRaycasts = false;
        to.blocksRaycasts = false;

        to.alpha = 0;
        to.gameObject.SetActive(true);

        while (elapsed < transitionDuration)
        {
            float t = elapsed / transitionDuration;
            from.alpha = 1 - t;
            to.alpha = t;
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        from.alpha = 0;
        from.gameObject.SetActive(false);

        to.alpha = 1;
        to.blocksRaycasts = true;
    }

    void ContinueGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    void LoadLevel(int levelIndex)
    {
        string sceneName = "Level" + levelIndex;
        SceneManager.LoadScene(sceneName);
    }
    void NewGame()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("Level1");

    }

    void Settings()
    {
        //WIP
    }

    // Update is called once per frame
    void Update()
    {

    }
}
