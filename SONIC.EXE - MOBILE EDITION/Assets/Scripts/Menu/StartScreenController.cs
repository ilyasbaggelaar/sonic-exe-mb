using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenController : MonoBehaviour
{

    public UnityEngine.UI.Image logoImage;
    public float minDisplayTime = 2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(LoadMainMenu());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator LoadMainMenu() {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainMenu");
        asyncLoad.allowSceneActivation = false;

        float timer = 0f;
        while (timer < minDisplayTime || !asyncLoad.isDone)
        {
            timer += Time.deltaTime;

            // optional: update a loading bar.
            if (timer >= minDisplayTime && asyncLoad.progress >= 0.9f)
            {
                //fade out the logo
                yield return StartCoroutine(FadeOutLogo());

                asyncLoad.allowSceneActivation = true;

            }

            yield return null;
        }
    }

    IEnumerator FadeOutLogo() {
        float fadeDuration = 1f;
        float timer = 0f;
        Color originalColor = logoImage.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);

            logoImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
    }
}
