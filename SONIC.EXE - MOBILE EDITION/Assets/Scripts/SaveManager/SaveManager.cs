using UnityEngine;
using UnityEngine.SceneManagement;

public static class SaveManager
{
    private const string LivesKey = "lives";
    private const string RingsKey = "rings";

    private const string SceneKey = "scene";

    private const string LevelUnlockKey = "levelUnlock";

    public static void Save(int lives, int rings)
    {
        PlayerPrefs.SetInt(LivesKey, lives);
        PlayerPrefs.SetInt(RingsKey, rings);
        PlayerPrefs.SetString(SceneKey, SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();

        Debug.Log("Game Saved..");

    }

    public static void Load(out int lives, out int rings, out string scene)
    {
        lives = PlayerPrefs.GetInt(LivesKey, 3);
        rings = PlayerPrefs.GetInt(RingsKey, 0);
        scene = PlayerPrefs.GetString(SceneKey, SceneManager.GetActiveScene().name);
    }
    public static bool HasSaveData()
    {
        return PlayerPrefs.HasKey(LivesKey) && PlayerPrefs.HasKey(SceneKey);
    }

    public static void ClearSave()
    {

        //add a confirmation button lol.
        PlayerPrefs.DeleteAll();
        Debug.Log("Save Data Cleared");
    }

    public static void UnlockNextLevel(int completedLevelIndex)
    {
        int currentUnlocked = PlayerPrefs.GetInt(LevelUnlockKey, 1);
        if (completedLevelIndex + 1 > currentUnlocked)
        {
            PlayerPrefs.SetInt(LevelUnlockKey, completedLevelIndex + 1);
            PlayerPrefs.Save();
            Debug.Log("Unlocked Level " + (completedLevelIndex + 1));
        }
    }

    public static int getUnlockedLevel()
    {
        return PlayerPrefs.GetInt(LevelUnlockKey, 1);
    }
}
