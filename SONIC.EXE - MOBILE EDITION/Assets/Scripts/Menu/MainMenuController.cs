using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using JetBrains.Annotations;
using Unity.Services.Core;
using com.unity3d.mediation;

public class MainMenuController : MonoBehaviour
{
    public Button continueButton;

    public Button newGameButton;

    public Button settings;

    public Button[] levelButtons;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        int unlockedLevel = SaveManager.getUnlockedLevel();

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1;

            levelButtons[i].interactable = levelIndex < unlockedLevel;

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
        settings.onClick.AddListener(Settings);

        continueButton.interactable = PlayerPrefs.HasKey("SavedGame");
        
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
        SceneManager.LoadScene("SampleScene");

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
