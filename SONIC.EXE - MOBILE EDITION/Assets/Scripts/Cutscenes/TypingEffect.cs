using UnityEngine;
using TMPro;
using System.Collections;


public class TypingEffect : MonoBehaviour
{

    public TextMeshProUGUI textComponent;
    [TextArea]
    public string FullText;
    public float typingSpeed = 0.01f;

    public AudioClip characterSound;
    public float characterFrequancy = 2;

    private AudioSource audioSource;

    private Coroutine typingCoroutine;


    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void OnEnable()
    {
        StartTyping();
    }

    public void StartTyping()
    {
        textComponent.text = "";
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        for (int i = 0; i < FullText.Length; i++)
        {
            textComponent.text += FullText[i];

            if (characterSound != null && i % characterFrequancy == 0)
            {
                audioSource.PlayOneShot(characterSound);
            }
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
