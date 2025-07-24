using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public string nextSceneName;
    void Start()
    {
        
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
    void Update()
    {
        
    }
}
