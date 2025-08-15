using UnityEngine;

public class LevelButtonController : MonoBehaviour
{


    [Header("Real level buttons")]
     public GameObject[] levelButtons;

     [Header("Locked level buttons")]
    public GameObject[] levelUnknownButtons;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        int unlockedLevel = SaveManager.getUnlockedLevel();

        for (int i = 0; i < levelButtons.Length; i++)
        {
            bool isUnlocked = (i + 1) <= unlockedLevel;

            if (levelButtons[i] != null)
                levelButtons[i].SetActive(isUnlocked);

            if (levelUnknownButtons[i] != null)
                levelUnknownButtons[i].SetActive(!isUnlocked);
       }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
